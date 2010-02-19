using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml.Linq;
using System.IO;

namespace JSmith.MSBuild.Tasks.Flex
{
    public class FlexApplicationProject : FlexCompiler
    {
        #region Constants

        public const string TempDirectoryName = "obj";
        public const string ProjectFileName = @".project";
        public const string ActionScriptPropertiesFileName = @".actionScriptProperties";
       
        #endregion

        #region Fields / Properties

        private string _projectRoot;
        protected string ProjectRoot
        {
            get
            {
                if (_projectRoot == null)
                {
                    int index = Project.ItemSpec.IndexOf("\\" + ProjectFileName);
                    _projectRoot = Project.ItemSpec.Substring(0, index);

                    TempDirectory = Path.Combine(_projectRoot, TempDirectoryName);

                }//end if

                return _projectRoot;

            }//end get

        }//end property

        public ITaskItem Project { get; set; }

        protected string ActionScriptPropertiesFile
        {
            get
            {
                return Path.Combine(ProjectRoot, ActionScriptPropertiesFileName);

            }//end get

        }//end property

        private string _projectName;
        private string _outputDirectory;
        private List<string> _sourceDirectories;
        private Dictionary<string, string> _modules;
        private Dictionary<string, bool> _optimizeModules;

        #endregion

        #region Constructor

        public FlexApplicationProject()
        {
            _sourceDirectories = new List<string>();
            _modules = new Dictionary<string, string>();
            _optimizeModules = new Dictionary<string, bool>();

        }//end constructor

        #endregion

        #region Flex Builder Project Parsing

        private void ParseProjectFile()
        {
            XDocument doc = XDocument.Load(Project.ItemSpec);

            _projectName = doc.Elements("projectDescription").Any() &&
                           doc.Element("projectDescription").Elements("name").Any() ?
                           doc.Element("projectDescription").Element("name").Value : String.Empty;

        }//end method

        private void ParseActionScriptPropertiesFile()
        {
            XDocument doc = XDocument.Load(ActionScriptPropertiesFile);

            XElement compilerNode = doc.Elements("actionScriptProperties").Any() &&
                                    doc.Element("actionScriptProperties").Elements("compiler").Any() ?
                                    doc.Element("actionScriptProperties").Element("compiler") : null;

            string mainSourceDirectory = compilerNode.Attributes("sourceFolderPath").Any() ? compilerNode.Attribute("sourceFolderPath").Value : String.Empty;
            _sourceDirectories.Add(mainSourceDirectory);

            if (doc.Element("actionScriptProperties").Attributes("mainApplicationPath").Any())
            {
                string projectRoot = Path.Combine(ProjectRoot, mainSourceDirectory);
                string entryPointPath = Path.Combine(projectRoot, doc.Element("actionScriptProperties").Attribute("mainApplicationPath").Value);
                EntryPoint = new TaskItem(entryPointPath);
            }
            else
            {
                throw new FlexConfigurationException("Flex application entry point not defined.");
            
            }//end if

            //grab additional source directories
            List<string> sourceDirectories = compilerNode.Elements("compilerSourcePath").Any() &&
                                             compilerNode.Element("compilerSourcePath").Elements("compilerSourcePathEntry").Any() &&
                                             compilerNode.Element("compilerSourcePath").Element("compilerSourcePathEntry").Attributes("path").Any() ?
                                             (from path in compilerNode.Element("compilerSourcePath").Elements("compilerSourcePathEntry")
                                              select path.Attribute("path").Value).ToList() : new List<string>();

            _sourceDirectories.AddRange(sourceDirectories);

            SourcePath = (from sd in _sourceDirectories
                          select new TaskItem(ProjectRoot + "\\" + sd + "\\")).ToArray();

            _outputDirectory = compilerNode.Attributes("outputFolderPath").Any() ? compilerNode.Attribute("outputFolderPath").Value : String.Empty;

            Output = new TaskItem(ProjectRoot + "\\" + _outputDirectory + "\\" + _projectName + ".swf");


            //libraries
            LibraryPath = compilerNode.Elements("libraryPath").Any() &&
                          compilerNode.Element("libraryPath").Elements("libraryPathEntry").Any() ?
                          (from lib in compilerNode.Element("libraryPath").Elements("libraryPathEntry")
                           where lib.Attribute("kind").Value == "3" && lib.Attribute("linkType").Value == "1"
                           select new TaskItem(FormatPath(lib.Attribute("path").Value))).ToArray() : null;

            IncludeLibraries = compilerNode.Elements("libraryPath").Any() &&
                               compilerNode.Element("libraryPath").Elements("libraryPathEntry").Any() ?
                               (from lib in compilerNode.Element("libraryPath").Elements("libraryPathEntry")
                                where lib.Attribute("kind").Value == "3" && lib.Attribute("linkType").Value == "2"
                                select new TaskItem(FormatPath(lib.Attribute("path").Value))).ToArray() : null;

            //modules
            _modules = doc.Element("actionScriptProperties").Elements("modules").Any() &&
                       doc.Element("actionScriptProperties").Element("modules").Elements("module").Any() ?
                       (from module in doc.Element("actionScriptProperties").Element("modules").Elements("module")
                        select module).ToDictionary(m => m.Attribute("sourcePath").Value, m => m.Attribute("destPath").Value) : new Dictionary<string, string>();

            _optimizeModules = doc.Element("actionScriptProperties").Elements("modules").Any() &&
                               doc.Element("actionScriptProperties").Element("modules").Elements("module").Any() ?
                               (from module in doc.Element("actionScriptProperties").Element("modules").Elements("module")
                                select module).ToDictionary(m => m.Attribute("sourcePath").Value, m => Boolean.Parse(m.Attribute("optimize").Value)) : new Dictionary<string, bool>();

        }//end method

        #endregion

        private string FormatPath(string path)
        {
            if (path.StartsWith("/"))
                return path.Substring(1);
            else
                return Path.Combine(ProjectRoot, path);

        }//end method

        #region Overrides 

        protected override string GenerateCommandLineCommands()
        {
            ParseProjectFile();
            ParseActionScriptPropertiesFile();
            
            return base.GenerateCommandLineCommands();

        }//end method

        public override bool Execute()
        {
            Log.LogMessage("Building Flex application");

            bool isSuccess = base.Execute();
            if (isSuccess)
                Log.LogMessage("Build succeeded");
            else
                Log.LogMessage("Build failed");

            bool isModuleBuildSuccess = BuildModules();

            return isSuccess && isModuleBuildSuccess;

        }//end method

        #endregion

        private bool BuildModules()
        {
            bool isSuccess = true;

            foreach (KeyValuePair<string, string> kvp in _modules)
            {
                FlexCompiler fm = new FlexCompiler();
                fm.BuildEngine = BuildEngine;
                fm.Version = Version;
                fm.EntryPoint = new TaskItem(Path.Combine(ProjectRoot, kvp.Key));
                fm.SourcePath = SourcePath;
                fm.Output = new TaskItem(Path.Combine(ProjectRoot, _outputDirectory) + "\\" + kvp.Value);
                fm.LibraryPath = LibraryPath;
                fm.IncludeLibraries = IncludeLibraries;
                fm.TempDirectory = TempDirectory;

                if (_optimizeModules[kvp.Key])
                    fm.LoadExterns = new List<ITaskItem> { new TaskItem(LinkReportPath) }.ToArray();

                bool moduleSuccess = fm.Execute();
                if (!moduleSuccess)
                    isSuccess = false;

            }//end foreach

            return isSuccess;

        }//end method

    }//end class

}//end namespace