using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JSmith.MSBuild.Tasks.Git;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace JSmith.MSBuild.Tasks.Tests
{
    [TestFixture]
    public class GitInfoTest
    {
        [Test]
        public void TestInstantiate()
        {
            GitClient task = new GitClient();
            task.WorkingDirectory = @"C:\Program Files\Git\bin\";
            task.Execute();
        }
    }
}
