using System;

namespace Metalogix.Utilities
{
    public static class FileUtils
    {
        internal readonly static string[] IllegalNameCharacters;

        static FileUtils()
        {
            string[] strArrays = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            FileUtils.IllegalNameCharacters = strArrays;
        }

        public static string RectifyName(string name, char replacementChar)
        {
            string str = name.Trim();
            for (int i = 0; i < (int)FileUtils.IllegalNameCharacters.Length; i++)
            {
                str = str.Replace(FileUtils.IllegalNameCharacters[i], replacementChar.ToString()).Trim();
            }

            return str;
        }

        public static void ValidateFileName(string fileName)
        {
            if (!fileName.Equals(FileUtils.RectifyName(fileName, '\u005F')))
            {
                throw new Exception("Special characters * ? : / < > \\ \" |  are not allowed in the filename.");
            }
        }
    }
}