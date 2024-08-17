using Metalogix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Metalogix.Core
{
    public static class AssemblyResolver
    {
        private readonly static object Lock;

        private static bool _isInitialized;

        private static List<DirectoryInfo> _searchFolders;

        static AssemblyResolver()
        {
            AssemblyResolver.Lock = new object();
            ApplicationData.MainAssemblyChanged +=
                new EventHandler(AssemblyResolver.ApplicationData_MainAssemblyChanged);
        }

        private static void ApplicationData_MainAssemblyChanged(object sender, EventArgs e)
        {
            AssemblyResolver._isInitialized = false;
        }

        private static void CreateSearchFolders()
        {
            DirectoryInfo mainAssemblyFolder = AssemblyResolver.GetMainAssemblyFolder();
            List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>()
            {
                new DirectoryInfo(ApplicationData.CommonApplicationPath)
            };
            directoryInfos.AddRange(
                AssemblyResolver.GetDirectoriesIn(AssemblyResolver.GetParentFolder(mainAssemblyFolder)));
            AssemblyResolver._searchFolders = directoryInfos;
        }

        private static IEnumerable<DirectoryInfo> GetDirectoriesIn(DirectoryInfo folder)
        {
            List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>()
            {
                folder
            };
            directoryInfos.AddRange(folder.GetDirectories());
            return directoryInfos;
        }

        private static DirectoryInfo GetMainAssemblyFolder()
        {
            return new DirectoryInfo(Path.GetDirectoryName(ApplicationData.MainAssembly.Location));
        }

        private static DirectoryInfo GetParentFolder(DirectoryInfo folder)
        {
            return folder.Parent;
        }

        public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly assembly;
            AssemblyResolver.TryInitialize();
            AssemblyName assemblyName = new AssemblyName(args.Name);
            string str = string.Concat(assemblyName.Name, ".dll");
            List<DirectoryInfo>.Enumerator enumerator = AssemblyResolver._searchFolders.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DirectoryInfo current = enumerator.Current;
                    try
                    {
                        string str1 = Path.Combine(current.FullName, str);
                        if (File.Exists(str1))
                        {
                            assembly = Assembly.LoadFrom(str1);
                            return assembly;
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }

                return null;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return assembly;
        }

        private static void TryInitialize()
        {
            if (AssemblyResolver._isInitialized)
            {
                return;
            }

            lock (AssemblyResolver.Lock)
            {
                if (!AssemblyResolver._isInitialized)
                {
                    AssemblyResolver.CreateSearchFolders();
                    AssemblyResolver._isInitialized = true;
                }
            }
        }
    }
}