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

        #endregion

        #region Constructor

        public FlexApplicationProject()
        {
            _sourceDirectories = new List<string>();

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

            return isSuccess;

        }//end method

        #endregion

    }//end class

}//end namespace