using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Metalogix.Utilities
{
    public static class SystemUtils
    {
        public static string EncodeTo64(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                return null;
            }

            return Convert.ToBase64String(Encoding.Unicode.GetBytes(rawData));
        }

        public static int GetFlagsEnumeratorFromStrings(Type enumeratorType, string[] sValues)
        {
            if ((int)sValues.Length != 1 || !(sValues[0] == "All"))
            {
                int num = 0;
                string[] strArrays = sValues;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    try
                    {
                        num |= (int)Enum.Parse(enumeratorType, str, true);
                    }
                    catch
                    {
                    }
                }

                return num;
            }

            int num1 = 0;
            foreach (int value in Enum.GetValues(enumeratorType))
            {
                num1 |= value;
            }

            return num1;
        }

        public static string[] GetFlagsEnumNamesAsArray(Type enumeratorType, int iFlagValue)
        {
            List<string> strs = new List<string>();
            int num = 0;
            foreach (int value in Enum.GetValues(enumeratorType))
            {
                num |= value;
                if ((iFlagValue & value) <= 0)
                {
                    continue;
                }

                strs.Add(Enum.GetName(enumeratorType, value));
            }

            if (iFlagValue != num)
            {
                return strs.ToArray();
            }

            return new string[] { "All" };
        }

        public static string GetResourceString(string sResourceName)
        {
            return SystemUtils.GetResourceString(sResourceName, false);
        }

        public static string GetResourceString(string sResourceName, bool bFromSystemAssembly)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string str = null;
            str = (callingAssembly == typeof(SystemUtils).Assembly || bFromSystemAssembly
                ? "Metalogix"
                : callingAssembly.GetName().Name);
            string str1 = string.Concat(str, ".Properties.Resources");
            return SystemUtils.GetResourceString(sResourceName, callingAssembly.GetType(str1));
        }

        public static string GetResourceString(string sResourceName, Type resourceType)
        {
            string str = null;
            if (sResourceName != null && resourceType != null)
            {
                PropertyInfo property = resourceType.GetProperty(sResourceName,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (property != null)
                {
                    str = property.GetValue(null, null).ToString();
                }
            }

            return str;
        }

        public static string GetResourceStringFromAssembly(string resourceName)
        {
            string end;
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string str = string.Concat(callingAssembly.GetName().Name, ".", resourceName);
            using (Stream manifestResourceStream = callingAssembly.GetManifestResourceStream(str))
            {
                if (manifestResourceStream == null)
                {
                    throw new Exception(string.Format("The resource: {0} did not exist within the assembly: {1}", str,
                        callingAssembly.Location));
                }

                end = (new StreamReader(manifestResourceStream)).ReadToEnd();
            }

            return end;
        }
    }
}