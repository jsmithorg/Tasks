using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace JSmith.MSBuild.Tasks.Flex
{
    public class FlexCompiler : ToolTask
    {
        #region Constants

        public const string DefaultToolPath = @"C:\FlexSDK\3.3.0\bin";
        public const string DefaultOutput = "Output.swf";
        //public const string TempDirectory = "obj";
        public const string VersionInfoFile = "Version.as";
        public const string VersionASClass =
@"/**
*   This file is machine-generated. Changes to this file may be overwritten.
*/
package 
{
	public class Version
	{
        private static const defaultFormat:String = ""{0}.{1}.{2}.{3}"";
		public static const major:int = {major};
		public static const minor:int = {minor};
		public static const build:int = {build};
		public static const revision:int = {revision};
		
        public static function toString(format:String = null):String
        {
            format = (format == null) ? defaultFormat : format;
            
            return format.replace(""{0}"", major).replace(""{1}"", minor).replace(""{2}"", build).replace(""{3}"", revision);
            //return major + ""."" + minor + ""."" + build + ""."" + revision;

        }//end method

	}//end class
	
}//end namespace
";
        #endregion

        #region Fields / Properties

        public string WorkingDirectory { get; set; }
        public string TempDirectory { get; set; }

        public ITaskItem Accessible { get; set; }
        public ITaskItem ActionScriptFileEncoding { get; set; }
        public ITaskItem AllowSourcePathOverlap { get; set; }
        public ITaskItem AS3 { get; set; }
        public ITaskItem ES { get; set; }
        public ITaskItem Benchmark { get; set; }
        public ITaskItem ContextRoot { get; set; }

        /* Metadata not yet included
        public ITaskItem Contributor { get; set; }
        public ITaskItem Creator { get; set; }
        public ITaskItem Date { get; set; }
        public ITaskItem Description { get; set; }
        public ITaskItem Language { get; set; }
        public ITaskItem LocalizedDescription { get; set; }
        public ITaskItem LocalizedTitle { get; set; }
        public ITaskItem Publisher { get; set; }
        public ITaskItem Title { get; set; }
        */

        [Required]
        public ITaskItem EntryPoint { get; set; }
        public ITaskItem Debug { get; set; }
        public ITaskItem DebugPassword { get; set; }
        public ITaskItem DefaultBackgroundColor { get; set; }
        public ITaskItem DefaultFramerate { get; set; }
        //public ITaskItem DefaultScriptLimits { get; set; }
        public ITaskItem DefaultSize { get; set; }
        public ITaskItem[] DefaultCSSFiles { get; set; }
        public ITaskItem DefaultCSSUrl { get; set; }
        public ITaskItem[] Define { get; set; }
        public ITaskItem DumpConfig { get; set; }
        public ITaskItem[] Externs { get; set; }
        public ITaskItem[] ExternalLibraryPath { get; set; }
        public ITaskItem GenerateFrameLoader { get; set; }
        public ITaskItem HeadlessServer { get; set; }
        public ITaskItem[] IncludeLibraries { get; set; }
        public ITaskItem[] IncludeResourceBundles { get; set; }
        public ITaskItem[] Includes { get; set; }
        public ITaskItem Incremental { get; set; }
        public ITaskItem KeepAS3Metadata { get; set; }
        public ITaskItem KeepAllTypeSelectors { get; set; }
        public ITaskItem KeepGeneratedActionscript { get; set; }
        public ITaskItem[] LibraryPath { get; set; }
        public ITaskItem License { get; set; }
        public ITaskItem LinkReport { get; set; }
        public ITaskItem LoadConfig { get; set; }
        public ITaskItem[] LoadExterns { get; set; }
        public ITaskItem Locale { get; set; }
        public ITaskItem Optimize { get; set; }
        public ITaskItem Output { get; set; }
        public ITaskItem RawMetadata { get; set; }
        public ITaskItem ResourceBundleList { get; set; }
        public ITaskItem[] RuntimeSharedLibraries { get; set; }
        public ITaskItem RuntimeSharedLibraryPath { get; set; }
        public ITaskItem Services { get; set; }
        public ITaskItem ShowActionscriptWarnings { get; set; }
        public ITaskItem ShowBindingWarnings { get; set; }
        public ITaskItem ShowShadowedDeviceFontWarnings { get; set; }
        public ITaskItem ShowUnusedTypeSelectorWarnings { get; set; }
        public ITaskItem[] SourcePath { get; set; }
        public ITaskItem StaticLinkRuntimeSharedLibraries { get; set; }
        public ITaskItem Strict { get; set; }
        public ITaskItem TargetPlayer { get; set; }
        public ITaskItem Theme { get; set; }
        public ITaskItem UseNetwork { get; set; }
        public ITaskItem UseResourceBundleMetadata { get; set; }
        public ITaskItem VerboseStackTraces { get; set; }
        public ITaskItem VerifyDigests { get; set; }
        public ITaskItem WarnWarningType { get; set; }
        public ITaskItem Warnings { get; set; }
        public ITaskItem Version { get; set; }

        /* Font settings not yet included */

        
        #endregion

        #region Overrides

        public override bool Execute()
        {
            bool isSuccess = base.Execute();

            if (Version != null)
                DestroyVersionInfo();

            return isSuccess;

        }//end method

        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(WorkingDirectory) ? base.GetWorkingDirectory() : WorkingDirectory;

        }//end method

        protected override string ToolName
        {
            get { return "mxmlc.exe"; }

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

            if (Version != null)
                CreateVersionInfo();

            clb.AppendSwitchIfNotNull("-accessible=", Accessible);
            clb.AppendSwitchIfNotNull("-actionscript-file-encoding ", ActionScriptFileEncoding);
            clb.AppendSwitchIfNotNull("-allow-source-path-overlap=", AllowSourcePathOverlap);
            clb.AppendSwitchIfNotNull("-as3=", AS3);
            clb.AppendSwitchIfNotNull("-benchmark=", Benchmark);
            clb.AppendSwitchIfNotNull("-context-root ", ContextRoot);
            clb.AppendSwitchIfNotNull("-debug=", Debug);
            clb.AppendSwitchIfNotNull("-debug-password ", DebugPassword);
            clb.AppendSwitchIfNotNull("-default-background-color ", DefaultBackgroundColor);
            clb.AppendSwitchIfNotNull("-default-frame-rate ", DefaultFramerate);
            //clb.AppendSwitchIfNotNull("-default-script-limits ", DefaultScriptLimits);
            clb.AppendSwitchIfNotNull("-default-size ", DefaultSize);
            clb.AppendSwitchIfNotNull("-default-css-files ", DefaultCSSFiles, " ");
            clb.AppendSwitchIfNotNull("-default-css-url ", DefaultCSSUrl);

            if (Define != null)
                for (int i = 0; i < Define.Length; i++)
                    clb.AppendSwitchIfNotNull("-define=", Define[i]);

            clb.AppendSwitchIfNotNull("-dump-config=", DumpConfig);
            clb.AppendSwitchIfNotNull("-es=", ES);
            clb.AppendSwitchIfNotNull("-externs ", Externs, " ");
            //clb.AppendSwitchIfNotNull("-external-library-path ", ExternalLibraryPath, " ");
            clb.AppendSwitchIfNotNull("-generate-frame-loader=", GenerateFrameLoader);
            clb.AppendSwitchIfNotNull("-headless-server=", HeadlessServer);
            clb.AppendSwitchIfNotNull("-include-libraries ", IncludeLibraries, " ");
            clb.AppendSwitchIfNotNull("-include-resource-bundles ", IncludeResourceBundles, " ");
            clb.AppendSwitchIfNotNull("-includes ", Includes, " ");
            clb.AppendSwitchIfNotNull("-incremental=", Incremental);
            clb.AppendSwitchIfNotNull("-keep-as3-metadata=", KeepAS3Metadata);
            clb.AppendSwitchIfNotNull("-keep-all-type-selectors=", KeepAllTypeSelectors);
            clb.AppendSwitchIfNotNull("-keep-generated-actionscript=", KeepGeneratedActionscript);

            if (LibraryPath != null)
                for (int i = 0; i < LibraryPath.Length; i++)
                    clb.AppendSwitchIfNotNull("-library-path+=", LibraryPath[i]);

            clb.AppendSwitchIfNotNull("-license ", License);
            clb.AppendSwitchIfNotNull("-link-report ", LinkReport);
            clb.AppendSwitchIfNotNull("-load-config ", LoadConfig);
            clb.AppendSwitchIfNotNull("-load-externs ", LoadExterns, " ");
            clb.AppendSwitchIfNotNull("-locale ", Locale);
            clb.AppendSwitchIfNotNull("-optimize=", Optimize);

            if (Output == null)
                Output = new TaskItem(DefaultOutput);
            clb.AppendSwitchIfNotNull("-output ", Output);

            clb.AppendSwitchIfNotNull("-raw-metadata ", RawMetadata);
            clb.AppendSwitchIfNotNull("-resource-bundle-list ", ResourceBundleList);
            clb.AppendSwitchIfNotNull("-runtime-shared-libraries ", RuntimeSharedLibraries, " ");
            clb.AppendSwitchIfNotNull("-runtime-shared-library-path ", RuntimeSharedLibraryPath);
            clb.AppendSwitchIfNotNull("-services ", Services);
            clb.AppendSwitchIfNotNull("-show-actionscript-warnings=", ShowActionscriptWarnings);
            clb.AppendSwitchIfNotNull("-show-binding-warnings=", ShowBindingWarnings);
            clb.AppendSwitchIfNotNull("-show-shadowed-device-font-warnings=", ShowShadowedDeviceFontWarnings);
            clb.AppendSwitchIfNotNull("-show-unused-type-selector-warnings=", ShowUnusedTypeSelectorWarnings);

            if (SourcePath != null)
                for (int i = 0; i < SourcePath.Length; i++)
                    clb.AppendSwitchIfNotNull("-source-path ", SourcePath[i]);

            if (Version != null)
                clb.AppendSwitchIfNotNull("-source-path ", TempDirectory);

            clb.AppendSwitchIfNotNull("-static-link-runtime-shared-libraries=", StaticLinkRuntimeSharedLibraries);
            clb.AppendSwitchIfNotNull("-strict=", Strict);
            clb.AppendSwitchIfNotNull("-target-player=", TargetPlayer);
            clb.AppendSwitchIfNotNull("-theme ", Theme);
            clb.AppendSwitchIfNotNull("-use-network=", UseNetwork);
            clb.AppendSwitchIfNotNull("-use-resource-bundle-metadata=", UseResourceBundleMetadata);
            clb.AppendSwitchIfNotNull("-verbose-stacktraces=", VerboseStackTraces);
            clb.AppendSwitchIfNotNull("-verify-digests=", VerifyDigests);
            clb.AppendSwitchIfNotNull("-warn-warning_type=", WarnWarningType);
            clb.AppendSwitchIfNotNull("-warnings=", Warnings);

            clb.AppendSwitchIfNotNull("-file-specs ", EntryPoint);
           
            Console.WriteLine(GenerateFullPathToTool() + " " + clb);

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

        private void CreateVersionInfo()
        {
            string[] version = Version.ItemSpec.Split('.');
            string asClass = VersionASClass.Replace("{major}", version[0])
                                           .Replace("{minor}", version[1])
                                           .Replace("{build}", version[2])
                                           .Replace("{revision}", version[3]);

            Console.WriteLine("version info: " + WorkingDirectory + "\\" + TempDirectory);
            Directory.CreateDirectory(TempDirectory);
            using (FileStream fs = File.Create(TempDirectory + "\\" + VersionInfoFile))
                fs.Write((new ASCIIEncoding()).GetBytes(asClass), 0, asClass.Length);

        }//end method

        private void DestroyVersionInfo()
        {
            File.Delete(TempDirectory + "\\" + VersionInfoFile);
            Directory.Delete(TempDirectory, true);

        }//end method

    }//end class

}//end namespace