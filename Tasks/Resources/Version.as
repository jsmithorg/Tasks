/**
*   This file is machine-generated. Changes to this file may be overwritten.
*/
package 
{
	public class Version
	{
        //private static const defaultFormat:String = ""{0}.{1}.{2}.{3}"";
		public static const major:int = {major};
		public static const minor:int = {minor};
		public static const build:int = {build};
		public static const revision:int = {revision};
		
        public static function toString(format:String = "{0}.{1}.{2}.{3}"):String
        {
            //format = (format == null) ? defaultFormat : format;
            
            return format.replace("{0}", major).replace("{1}", minor).replace("{2}", build).replace("{3}", revision);
            //return major + ""."" + minor + ""."" + build + ""."" + revision;

        }//end method

	}//end class
	
}//end namespace