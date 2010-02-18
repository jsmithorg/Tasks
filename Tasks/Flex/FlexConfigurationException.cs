using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSmith.MSBuild.Tasks.Flex
{
    public class FlexConfigurationException : Exception
    {
        public FlexConfigurationException() : base() { }
        public FlexConfigurationException(string message) : base(message) { }
        public FlexConfigurationException(string message, Exception innerException) : base(message, innerException) { }

    }//end class

}//end namespace