using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.RegularExpressions;

namespace JSmith.MSBuild.Tasks.Git
{
    public class GitInfo : GitClient
    {
        public string DateFormat { get; set; }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder clb = new CommandLineBuilder();
            clb.AppendSwitch("log");
            clb.AppendSwitch("--date=iso");
            clb.AppendSwitch("-n 1");

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

            string[] lines = Regex.Split(result, Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.ToLower().StartsWith("commit"))
                {
                    Hash = line.Substring(line.IndexOf(" "));
                }
                else if (line.ToLower().StartsWith("author"))
                {
                    string authorAndEmail = line.Substring(line.IndexOf(" "));
                    int index = authorAndEmail.LastIndexOf(" ");
                    Author = authorAndEmail.Substring(0, index).Trim();
                    AuthorEmail = authorAndEmail.Substring(index).Trim().Trim(new char[]{ '<', '>'});

                }
                else if (line.ToLower().StartsWith("date"))
                {
                    RevisionDate = DateTime.Parse(line.Substring(line.IndexOf(" ")).Trim()).ToString(DateFormat);

                }//end if

            }//end for

            //author
            //hash
            //date
            //messages

        }//end method

    }//end class

}//end namespace