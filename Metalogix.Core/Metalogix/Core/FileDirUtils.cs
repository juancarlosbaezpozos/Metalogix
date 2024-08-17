using Metalogix;
using Metalogix.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.Core
{
    public class FileDirUtils
    {
        public FileDirUtils()
        {
        }

        public static bool HasWritePermission()
        {
            string[] commonDataPath = new string[]
                { ApplicationData.CommonDataPath, ApplicationData.CommonApplicationPath };
            return FileDirUtils.HasWritePermissionOnDirectories(commonDataPath);
        }

        public static bool HasWritePermissionOnDirectories(params string[] folderPaths)
        {
            if ((int)folderPaths.Length == 0)
            {
                return true;
            }

            bool flag = true;
            Exception exception = null;
            if (folderPaths.Any<string>((string path) =>
                    !FileDirUtils.HasWritePermissionOnDirectory(path, out exception)))
            {
                GlobalServices.ErrorHandler.HandleException("Content Matrix - Insufficient permissions",
                    string.Concat("Please try running as an administrator as write access is required for:",
                        Environment.NewLine, string.Join(Environment.NewLine, folderPaths)), exception);
                flag = false;
            }

            return flag;
        }

        public static bool HasWritePermissionOnDirectory(string folderPath, out Exception exPermission)
        {
            bool flag;
            try
            {
                exPermission = null;
                Guid guid = Guid.NewGuid();
                string str = Path.Combine(folderPath, string.Concat(guid.ToString("N"), ".txt"));
                File.AppendAllText(str, "Write Test");
                File.Delete(str);
                flag = true;
            }
            catch (Exception exception)
            {
                exPermission = exception;
                flag = false;
            }

            return flag;
        }
    }
}