using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace JSmith.MSBuild.Tasks.Flex
{
    public class FlexComponentCompiler : ToolTask
    {
        #region Constants

        public const string DefaultToolPath = @"C:\FlexSDK\3.3.0\bin";

        #endregion

        #region Fields / Properties

        public string WorkingDirectory { get; set; }

        public ITaskItem ComputeDigest { get; set; }
        public ITaskItem Directory { get; set; }
        public ITaskItem[] SourcePath { get; set; }
        public ITaskItem[] IncludeClasses { get; set; }
        public ITaskItem[] IncludeFile { get; set; }
        public ITaskItem IncludeLookupOnly { get; set; }
        public ITaskItem[] IncludeNamespaces { get; set; }
        public ITaskItem IncludeSources { get; set; }
        public ITaskItem[] IncludeStylesheet { get; set; }
        public ITaskItem Output { get; set; }

        #endregion

        #region Overrides

        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(WorkingDirectory) ? base.GetWorkingDirectory() : WorkingDirectory;

        }//end method

        protected override string ToolName
        {
            get { return "compc.exe"; }

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

            if (SourcePath != null)
                for (int i = 0; i < SourcePath.Length; i++)
                    clb.AppendSwitchIfNotNull("-source-path ", SourcePath[i]);

            clb.AppendSwitchIfNotNull("-compute-digest=", ComputeDigest);
            clb.AppendSwitchIfNotNull("-directory=", Directory);
            clb.AppendSwitchIfNotNull("-include-classes ", IncludeClasses, " ");

            if (IncludeFile != null)
                for (int i = 0; i < IncludeFile.Length; i++)
                    clb.AppendSwitchUnquotedIfNotNull("-include-file ", IncludeFile[i] + " " + IncludeFile[i]);

            clb.AppendSwitchIfNotNull("-include-lookup-only=", IncludeLookupOnly);
            clb.AppendSwitchIfNotNull("-include-namespaces ", IncludeNamespaces, " ");
            clb.AppendSwitchIfNotNull("-include-sources ", IncludeSources);
            clb.AppendSwitchIfNotNull("-include-stylesheet ", IncludeStylesheet, " ");
            clb.AppendSwitchIfNotNull("-output ", Output);

            Console.WriteLine("Command Line command: " + Environment.NewLine + clb);

            return clb.ToString();

        }//end method

        #region Logging

        protected override void LogToolCommand(string message)
        {
            if (BuildEngine != null)
                Log.LogCommandLine(MessageImportance.Low, message);
            else
                Console.WriteLine(message);

        }//end method

        protected override MessageImportance StandardOutputLoggingImportance
        {
            get { return MessageImportance.Normal; }

        }//end method

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (messageImportance == MessageImportance.High)
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            
            //outputBuffer.Append(singleLine + Environment.NewLine);

        }//end method

        #endregion

        #endregion

    }//end class

}//end namespace