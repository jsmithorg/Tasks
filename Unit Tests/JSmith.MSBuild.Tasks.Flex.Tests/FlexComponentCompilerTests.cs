using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JSmith.MSBuild.Tasks.Flex;
using Microsoft.Build.Framework;

namespace JSmith.MSBuild.Tasks.Flex.Tests
{
    [TestFixture]
    public class FlexComponentCompilerTests
    {
        [Test]
        public void TestMethod()
        {
            FlexComponentCompiler fcc = new FlexComponentCompiler();
            //fcc.Directory = true;
            //fcc.ComputeDigest = true;
            bool isSuccess = fcc.Execute();

            Assert.IsTrue(isSuccess);
        }
    }
}
