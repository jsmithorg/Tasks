using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace JSmith.MSBuild.Tasks.Flex
{
    public class FlexDocumentation : ToolTask
    {
        #region Constants

        public const string DefaultToolPath = @"C:\FlexSDK\3.3.0\bin";

        #endregion

        #region Fields / Properties

        public string WorkingDirectory { get; set; }

        public ITaskItem[] DocClasses { get; set; }
        public ITaskItem DocNamespaces { get; set; }
        public ITaskItem[] DocSources { get; set; }
        public ITaskItem[] ExcludeClasses { get; set; }
        public ITaskItem ExcludeDependencies { get; set; }
        public ITaskItem Footer { get; set; }
        public ITaskItem[] LibraryPath { get; set; }
        public ITaskItem LeftFramesetWidth { get; set; }
        public ITaskItem MainTitle { get; set; }
        public ITaskItem Output { get; set; }
        public ITaskItem[] Package { get; set; }
        public ITaskItem[] SourcePath { get; set; }
        public ITaskItem TemplatesPath { get; set; }
        public ITaskItem WindowTitle { get; set; }

        #endregion

        #region Overrides

        public override bool Execute()
        {
            bool isSuccess = base.Execute();

            return isSuccess;

        }//end method

        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(WorkingDirectory) ? base.GetWorkingDirectory() : WorkingDirectory;

        }//end method

        protected override string ToolName
        {
            get { return "asdoc.exe"; }

        }//end property

        protected override string GenerateFullPathToTool()
        {
            if (ToolPath == null)
                ToolPath = DefaultToolPath;

            return Path.Combine(ToolPath, ToolName);

        }//end method

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder clb = new CommandLineBuilder();

            clb.AppendSwitchIfNotNull("-doc-classes ", DocClasses, " ");
            //clb.AppendSwitchIfNotNull("-doc-namespaces ", DocNamespaces, " ");
            clb.AppendSwitchIfNotNull("-doc-sources ", DocSources, " ");
            clb.AppendSwitchIfNotNull("-exclude-classes ", ExcludeClasses, " ");
            clb.AppendSwitchIfNotNull("-exclude-dependencies=", ExcludeDependencies);
            clb.AppendSwitchIfNotNull("-footer ", Footer);

            if (LibraryPath != null)
                for (int i = 0; i < LibraryPath.Length; i++)
                    clb.AppendSwitchIfNotNull("-library-path+=", LibraryPath[i]);

            clb.AppendSwitchIfNotNull("-left-frameset-width ", LeftFramesetWidth);
            clb.AppendSwitchIfNotNull("-main-title ", MainTitle);
            clb.AppendSwitchIfNotNull("-output ", Output);

            if (Package != null)
                for (int i = 0; i < Package.Length; i++)
                    clb.AppendSwitchIfNotNull("-package ", Package[i]);

            //clb.AppendSwitchIfNotNull("-source-path ", SourcePath);
            if (SourcePath != null)
                for (int i = 0; i < SourcePath.Length; i++)
                    clb.AppendSwitchIfNotNull("-source-path ", SourcePath[i]);

            clb.AppendSwitchIfNotNull("-templates-path ", TemplatesPath);
            clb.AppendSwitchIfNotNull("-window-title ", WindowTitle);

            Console.WriteLine(GenerateFullPathToTool() + " " + clb);

            return clb.ToString();

        }//end method

        #endregion

    }//end class

}//end namespace