using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.RegularExpressions;

namespace JSmith.MSBuild.Tasks.Git
{
    public class GitRevision : GitClient
    {
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder clb = new CommandLineBuilder();
            clb.AppendSwitch("describe");

            return clb.ToString();

        }//end method

        public override bool Execute()
        {
            bool bSuccess = base.Execute();

            if (bSuccess)
            {
                Console.WriteLine(outputBuffer.ToString());

                ParseOutput();
            }
            else
            {
                //ResetMemberVariables();
            }

            return bSuccess;
        }

        private void ParseOutput()
        {
            string result = outputBuffer.ToString();

            string revision = String.Empty;
            string tag = String.Empty;

            int index = result.LastIndexOf("-");
            if (index >= 0)
            {
                result = result.Substring(0, index);
                index = result.LastIndexOf("-");

                if (index >= 0)
                    revision = result.Substring(index + 1);

                LastTag = result.Substring(0, index);

            }//end if

            RevisionSinceLastTag = (revision == null || revision == String.Empty) ? "0" : revision;

        }//end method

    }//end class

}//end namespace