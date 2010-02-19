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
    public class FlexProject : Task
    {
        #region Constants

        public const string ProjectFileName = @".project";
        public const string FlexPropertiesFileName = @".flexProperties";
        public const string FlexLibPropertiesFileName = @".flexLibProperties";
        public const string ActionScriptPropertiesFileName = @".actionScriptProperties";

        #endregion

        #region Fields / Properties

        public ITaskItem[] Projects { get; set; }
        public ITaskItem Version { get; set; }

        protected virtual MessageImportance DefaultMessageImportance
        {
            get { return MessageImportance.Normal; }

        }//end property

        private List<string> _buildOrder;
        private Dictionary<string, string> _projectFiles;
        private Dictionary<string, List<string>> _dependencies;

        #endregion

        #region Constructor

        public FlexProject()
        {
            _buildOrder = new List<string>();
            _projectFiles = new Dictionary<string, string>();
            _dependencies = new Dictionary<string, List<string>>();

        }//end constructor

        #endregion

        #region Overrides

        public override bool Execute()
        {
            try
            {
                //parse all the project files
                for (int i = 0; i < Projects.Length; i++)
                    ParseProjectFile(Projects[i].ItemSpec);

                //reorder based on dependencies
                DetermineBuildOrder();

                //build each project
                foreach (string project in _buildOrder)
                    BuildProject(_projectFiles[project]);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);

            }//end try

            return true;

        }//end method

        #endregion

        #region Flex Builder Project Parsing

        private void ParseProjectFile(string projectFile)
        {
            XDocument doc = XDocument.Load(projectFile);

            string projectName = doc.Elements("projectDescription").Any() &&
                                 doc.Element("projectDescription").Elements("name").Any() ?
                                 doc.Element("projectDescription").Element("name").Value : String.Empty;

            _projectFiles.Add(projectName, projectFile);

            //Console.WriteLine("Project: " + projectName + ": " + projectFile);

            List<string> dependencies = doc.Elements("projectDescription").Any() &&
                                        doc.Element("projectDescription").Elements("projects").Any() &&
                                        doc.Element("projectDescription").Element("projects").Elements("project").Any() ?
                                        (from project in doc.Element("projectDescription").Element("projects").Elements("project")
                                         select project.Value).ToList() : new List<string>();

            //Console.WriteLine("Project dependencies for " + projectName + ": " + dependencies.Count);
            //for (int i = 0; i < dependencies.Count; i++)
                //Console.WriteLine("Project dependency: " + dependencies[i]);

            _dependencies.Add(projectName, dependencies);

        }//end method

        #endregion

        private void DetermineBuildOrder()
        {
            foreach (KeyValuePair<string, string> kvp in _projectFiles)
            {
                //if the project name's not already in there, add it
                if (!_buildOrder.Contains(kvp.Key))
                    _buildOrder.Add(kvp.Key);

                int projectIndex = _buildOrder.IndexOf(kvp.Key);
                List<string> dependencies = _dependencies[kvp.Key];

                //loop through the dependencies and reorder projects as necessary
                foreach(string dependency in dependencies)
                {
                    if (!_projectFiles.ContainsKey(dependency))
                        throw new FlexConfigurationException("Could not find dependency '" + dependency + "' for project '" + kvp.Key + "'.");

                    //string dependency = dependencies[i];
                    int index = _buildOrder.IndexOf(dependency);

                    //if the index is not 0 or more, the dependency's not in the list (so add it)
                    //otherwise if it's in a position after our current project, remove
                    //and re-insert the dependent project above our current project
                    if (index < 0)
                    {
                        _buildOrder.Insert(projectIndex, dependency);
                    }
                    else if (index > projectIndex)
                    {
                        _buildOrder.Remove(dependency);
                        _buildOrder.Insert(projectIndex, dependency);

                    }//end if
                        
                }//end for

            }//end foreach

            //log it
            Log.LogMessage(DefaultMessageImportance, "Flex project build order:");
            for (int i = 0; i < _buildOrder.Count; i++)
                Log.LogMessage(DefaultMessageImportance, "  " + _buildOrder[i]);

        }//end method

        #region Build

        private bool BuildProject(string projectFile)
        {
            if (IsLibrary(projectFile))
                return BuildLibraryProject(projectFile);
            else if (IsFlexApplication(projectFile) || IsActionScriptApplication(projectFile))
                return BuildFlexApplicationProject(projectFile);
            else
                throw new FileNotFoundException("Could not find either a " + FlexPropertiesFileName + ", " + FlexLibPropertiesFileName + " or " + ActionScriptPropertiesFileName + " configuration file for project " + GetProjectRoot(projectFile));

        }//end method

        private bool BuildLibraryProject(string projectFile)
        {
            FlexLibraryProject flp = new FlexLibraryProject();
            flp.BuildEngine = BuildEngine;
            flp.Project = new TaskItem(projectFile);
            
            return flp.Execute();

        }//end method

        private bool BuildFlexApplicationProject(string projectFile)
        {
            FlexApplicationProject fap = new FlexApplicationProject();
            fap.BuildEngine = BuildEngine;
            fap.Project = new TaskItem(projectFile);
            fap.Version = Version;
            fap.GenerateLinkReport = true;

            return fap.Execute();

        }//end method

        #endregion

        #region Utilities

        private bool IsLibrary(string projectFile)
        {
            string projectRoot = GetProjectRoot(projectFile);
            string path = Path.Combine(projectRoot, FlexLibPropertiesFileName);

            return File.Exists(path);

        }//end method

        private bool IsFlexApplication(string projectFile)
        {
            string projectRoot = GetProjectRoot(projectFile);
            string path = Path.Combine(projectRoot, FlexPropertiesFileName);

            return File.Exists(path);

        }//end method

        private bool IsActionScriptApplication(string projectFile)
        {
            string projectRoot = GetProjectRoot(projectFile);
            string path = Path.Combine(projectRoot, ActionScriptPropertiesFileName);

            return File.Exists(path) && !IsLibrary(projectFile) && !IsFlexApplication(projectFile);

        }//end method

        private string GetProjectRoot(string projectFile)
        {
            int index = projectFile.IndexOf("\\" + ProjectFileName);
            return projectFile.Substring(0, index);

        }//end method

        #endregion

    }//end class

}//end namespace