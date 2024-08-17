using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Metalogix.Licensing.SK
{
    internal static class RegistryHelper
    {
        private static RegistryKey GetBase(RegistryHelper.Base baseType)
        {
            if (baseType != RegistryHelper.Base.LocalMachine)
            {
                throw new Exception("Unknown base type.");
            }

            return Registry.LocalMachine;
        }

        public static object LoadValue(RegistryHelper.Base baseType, string path, string keyName)
        {
            object obj;
            using (RegistryKey @base = RegistryHelper.GetBase(baseType))
            {
                using (RegistryKey registryKey = @base.OpenSubKey(path, false))
                {
                    obj = (registryKey != null ? registryKey.GetValue(keyName) : null);
                }
            }

            return obj;
        }

        public static void SaveValue(RegistryHelper.Base baseType, string path, string keyName, object keyValue)
        {
            using (RegistryKey @base = RegistryHelper.GetBase(baseType))
            {
                Stack<string> strs = new Stack<string>();
                bool flag = false;
                string str = path;
                while (!flag)
                {
                    using (RegistryKey registryKey = @base.OpenSubKey(str, true))
                    {
                        flag = registryKey != null;
                    }

                    if (flag)
                    {
                        continue;
                    }

                    strs.Push(str);
                    str = str.Substring(0, str.LastIndexOf('\\'));
                }

                foreach (string str1 in strs)
                {
                    using (RegistryKey registryKey1 = @base.OpenSubKey(str, true))
                    {
                        string str2 = str1.Substring(str.Length + 1);
                        using (RegistryKey registryKey2 =
                               registryKey1.CreateSubKey(str2, RegistryKeyPermissionCheck.ReadSubTree))
                        {
                        }
                    }

                    str = str1;
                }

                using (RegistryKey registryKey3 = @base.OpenSubKey(path, true))
                {
                    registryKey3.SetValue(keyName, keyValue);
                }
            }
        }

        public enum Base
        {
            LocalMachine
        }
    }
}