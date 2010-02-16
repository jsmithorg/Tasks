using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Text.RegularExpressions;

namespace JSmith.MSBuild.Tasks.Git
{
    public class GitClient : ToolTask
    {
        #region Constants

		/// <summary>
		/// The default relative path of the Git installation.
		/// The value is <c>@"Git\bin"</c>.
		/// </summary>
		public const string DEFAULT_GIT_DIRECTORY = @"Git\bin";

		#endregion Constants

        #region Properties

        private string _workingDirectory;

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }

        public string Command { get; set; }

        [Output]
        public string Author { get; set; }

        [Output]
        public string AuthorEmail { get; set; }

        [Output]
        public string LastTag { get; set; }

        [Output]
        public string RevisionDate { get; set; }

        [Output]
        public string RevisionSinceLastTag { get; set; }

        [Output]
        public string Hash { get; set; }

        #endregion

        protected StringBuilder outputBuffer;

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Git"/> class.
		/// </summary>
        public GitClient()
		{
            outputBuffer = new StringBuilder();
		}

		#endregion Constructor

        private void ParseOutput()
        {
            Console.WriteLine("OUTPUT!!!!");
            Console.WriteLine("buffer: " + outputBuffer);
            RevisionSinceLastTag = outputBuffer.ToString();

        }//end method

        #region Task Overrides

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns>true if execution is successful, false if not.</returns>
        public override bool Execute()
        {
            bool bSuccess = base.Execute();

            if (bSuccess)
            {
                ParseOutput();
            }
            else
            {
                //ResetMemberVariables();
            }

            return bSuccess;
        }

        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder clb = new CommandLineBuilder();
            clb.AppendSwitch(Command);
            //clb.AppendSwitch("&&");
            //clb.AppendSwitch("\"" + GenerateFullPathToTool() + "\"");
            //clb.AppendSwitch("log");
            //clb.AppendSwitch("--no-color");
            //clb.AppendSwitch("-n 1");
            //clb.AppendSwitch("--date=iso");

            Console.WriteLine("PATH: " + clb.ToString());
            return clb.ToString();

            /*CommandLineBuilder builder = new CommandLineBuilder();
            builder.AppendSwitch("/nologo");
            if (DisableShadowCopy)
            {
                builder.AppendSwitch("/noshadow");
            }
            if (TestInNewThread)
            {
                builder.AppendSwitch("/thread");
            }
            builder.AppendFileNamesIfNotNull(_assemblies, " ");

            builder.AppendSwitchIfNotNull("/config=", _projectConfiguration);

            builder.AppendSwitchIfNotNull("/fixture=", _fixture);

            builder.AppendSwitchIfNotNull("/include=", _includeCategory);

            builder.AppendSwitchIfNotNull("/exclude=", _excludeCategory);

            builder.AppendSwitchIfNotNull("/transform=", _xsltTransformFile);

            builder.AppendSwitchIfNotNull("/xml=", _outputXmlFile);

            builder.AppendSwitchIfNotNull("/err=", _errorOutputFile);

            return builder.ToString();*/
        }

        private void CheckToolPath()
        {
            ToolPath = @"C:\Program Files\Git\bin";
            //Log.LogCommandLine("Checking TOOL PATH");

            /*string nunitPath = ToolPath == null ? String.Empty : ToolPath.Trim();
            if (String.IsNullOrEmpty(nunitPath))
            {
                nunitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                nunitPath = Path.Combine(nunitPath, DEFAULT_NUNIT_DIRECTORY);

                try
                {
                    using (RegistryKey buildKey = Registry.ClassesRoot.OpenSubKey(@"NUnitTestProject\shell\open\command"))
                    {
                        if (buildKey == null)
                        {
                            Log.LogError(Properties.Resources.NUnitNotFound);
                        }
                        else
                        {
                            nunitPath = buildKey.GetValue(null, nunitPath).ToString();
                            Regex nunitRegex = new Regex("(.+)nunit-gui\\.exe", RegexOptions.IgnoreCase);
                            Match pathMatch = nunitRegex.Match(nunitPath);
                            nunitPath = pathMatch.Groups[1].Value.Replace("\"", "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                }
                ToolPath = nunitPath;
            }*/
        }

        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>
        /// The fully qualified path to the executable file.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            CheckToolPath();
            return Path.Combine(ToolPath, ToolName);
        }

        /// <summary>
        /// Logs the starting point of the run to all registered loggers.
        /// </summary>
        /// <param name="message">A descriptive message to provide loggers, usually the command line and switches.</param>
        protected override void LogToolCommand(string message)
        {
            if (BuildEngine != null)
                Log.LogCommandLine(MessageImportance.Low, message);
            else
                Console.WriteLine(message);

        }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the executable file to run.</returns>
        protected override string ToolName
        {
            get { return @"git.exe"; }
        }

        /// <summary>
        /// Gets the <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.</returns>
        protected override MessageImportance StandardOutputLoggingImportance
        {
            get
            {
                return MessageImportance.Normal;
            }
        }

        /// <summary>
        /// Returns the directory in which to run the executable file.
        /// </summary>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(_workingDirectory) ? base.GetWorkingDirectory() : _workingDirectory;
        }

        /// <summary>
        /// Logs the events from text output.
        /// </summary>
        /// <param name="singleLine">The single line.</param>
        /// <param name="messageImportance">The message importance.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (messageImportance == MessageImportance.High)
            {
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            }
            outputBuffer.Append(singleLine + Environment.NewLine);
        }

        #endregion Task Overrides

    }//end class

}//end namespace