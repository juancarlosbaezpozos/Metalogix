using Metalogix;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Metalogix.SharePoint.Adapters.DB
{
    public static class UtilityLauncher
    {
        private static string _blobUnshredderExeName;

        private static string _resourceLocalizerExeName;

        private static string _blobUnshredderExePath;

        private static string _resourceLocalizerExePath;

        public static string BlobUnshredderExePath
        {
            get
            {
                if (UtilityLauncher._blobUnshredderExePath == null)
                {
                    try
                    {
                        UtilityLauncher._blobUnshredderExePath =
                            UtilityLauncher.GetLauncherLocation(UtilityLauncher._blobUnshredderExeName);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        throw new Exception(
                            string.Format("Couldn't not find {0} in installed directory.",
                                UtilityLauncher._blobUnshredderExeName), exception.InnerException);
                    }
                }

                return UtilityLauncher._blobUnshredderExePath;
            }
        }

        public static string ResourceLocalizerExePath
        {
            get
            {
                if (UtilityLauncher._resourceLocalizerExePath == null)
                {
                    try
                    {
                        UtilityLauncher._resourceLocalizerExePath =
                            UtilityLauncher.GetLauncherLocation(UtilityLauncher._resourceLocalizerExeName);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        throw new Exception(
                            string.Format("Couldn't not find '{0}' in installed directory.",
                                UtilityLauncher._resourceLocalizerExeName), exception.InnerException);
                    }
                }

                return UtilityLauncher._resourceLocalizerExePath;
            }
        }

        static UtilityLauncher()
        {
            UtilityLauncher._blobUnshredderExeName = "Metalogix.SharePoint.BlobUnshredder.exe";
            UtilityLauncher._resourceLocalizerExeName = "Metalogix.SharePoint.ResourceLocalizer.exe";
            UtilityLauncher._blobUnshredderExePath = null;
            UtilityLauncher._resourceLocalizerExePath = null;
        }

        private static string GetLauncherLocation(string launcherName)
        {
            DirectoryInfo parent = (new DirectoryInfo(ApplicationData.MainAssembly.Location)).Parent;
            return Path.Combine(parent.FullName, launcherName);
        }

        public static byte[] LaunchBlobUnshredder(BlobUnshredderArgsBuilder argsBuilder)
        {
            UtilityLauncher.LaunchConsole(UtilityLauncher.BlobUnshredderExePath, argsBuilder);
            string saveFileName = argsBuilder.SaveFileName;
            if (!File.Exists(saveFileName))
            {
                throw new UtilityLauncherException(argsBuilder.Build(),
                    string.Format("Content file '{0}' does not exists or removed.", saveFileName));
            }

            byte[] numArray = UtilityLauncher.ReadFileInChunks(saveFileName, argsBuilder.ChunkSize);
            File.Delete(saveFileName);
            return numArray;
        }

        private static string LaunchConsole(string launcherPath, object argsBuilder)
        {
            if (argsBuilder == null)
            {
                throw new ArgumentNullException("Valid arguments are not provided");
            }

            string str = launcherPath;
            if (!File.Exists(str))
            {
                throw new FileNotFoundException(string.Format("File '{0}' does not exists or removed", str));
            }

            string str1 = null;
            if (argsBuilder is BlobUnshredderArgsBuilder)
            {
                str1 = ((BlobUnshredderArgsBuilder)argsBuilder).Build();
            }
            else if (argsBuilder is ResourceLocalizerArgsBuilder)
            {
                str1 = ((ResourceLocalizerArgsBuilder)argsBuilder).Build();
            }

            if (string.IsNullOrEmpty(str1))
            {
                throw new ArgumentNullException("Valid arguments are not provided");
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(str, str1)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(str),
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            Process process = Process.Start(processStartInfo);
            process.WaitForExit();
            string end = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(end))
            {
                throw new UtilityLauncherException(str1, end);
            }

            return process.StandardOutput.ReadToEnd();
        }

        public static string LaunchResourceLocalizer(ResourceLocalizerArgsBuilder argsBuilder)
        {
            return UtilityLauncher.LaunchConsole(UtilityLauncher.ResourceLocalizerExePath, argsBuilder);
        }

        // Metalogix.SharePoint.Adapters.DB.UtilityLauncher
        private static byte[] ReadFileInChunks(string outputFile, int chunkSize)
        {
            chunkSize = ((chunkSize <= 0) ? 524288 : (chunkSize * 1024));
            int num = 0;
            byte[] result;
            using (FileStream fileStream = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    byte[] array = new byte[fileStream.Length];
                    do
                    {
                        binaryReader.BaseStream.Seek(0L, SeekOrigin.Current);
                        byte[] array2 = binaryReader.ReadBytes(chunkSize);
                        array2.CopyTo(array, num);
                        num += array2.Length;
                    } while ((long)num < fileStream.Length);

                    result = array;
                }
            }

            return result;
        }
    }
}