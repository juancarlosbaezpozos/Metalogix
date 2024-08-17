using Metalogix.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix
{
    public static class TypeUtils
    {
        private static TypeUtils.LookUpData _assemblyLookUp;

        private static TypeUtils.LookUpData _assemblyVersionLookUp;

        private static TypeUtils.LookUpData _namespaceLookUp;

        private static TypeUtils.LookUpData _nameLookUp;

        private static TypeUtils.LookUpData _typeLookUp;

        private readonly static object _updateTypeLock;

        public static TypeUtils.LookUpData AssemblyLookUp
        {
            get
            {
                TypeUtils.LookUpData lookUpDatum = TypeUtils._assemblyLookUp;
                if (lookUpDatum == null)
                {
                    lookUpDatum =
                        new TypeUtils.LookUpData(Path.Combine(ApplicationData.ApplicationPath, "AssemblyLookUp.xml"));
                    TypeUtils._assemblyLookUp = lookUpDatum;
                }

                return lookUpDatum;
            }
        }

        public static TypeUtils.LookUpData NameLookUp
        {
            get
            {
                TypeUtils.LookUpData lookUpDatum = TypeUtils._nameLookUp;
                if (lookUpDatum == null)
                {
                    lookUpDatum =
                        new TypeUtils.LookUpData(Path.Combine(ApplicationData.ApplicationPath, "NameLookUp.xml"));
                    TypeUtils._nameLookUp = lookUpDatum;
                }

                return lookUpDatum;
            }
        }

        public static TypeUtils.LookUpData NamespaceLookUp
        {
            get
            {
                TypeUtils.LookUpData lookUpDatum = TypeUtils._namespaceLookUp;
                if (lookUpDatum == null)
                {
                    lookUpDatum =
                        new TypeUtils.LookUpData(Path.Combine(ApplicationData.ApplicationPath, "NamespaceLookUp.xml"));
                    TypeUtils._namespaceLookUp = lookUpDatum;
                }

                return lookUpDatum;
            }
        }

        public static TypeUtils.LookUpData TypeLookUp
        {
            get
            {
                if (TypeUtils._typeLookUp == null)
                {
                    TypeUtils._typeLookUp =
                        new TypeUtils.LookUpData(Path.Combine(ApplicationData.ApplicationPath, "TypeLookUp.xml"));
                }

                return TypeUtils._typeLookUp;
            }
        }

        public static TypeUtils.LookUpData VersionLookUp
        {
            get
            {
                TypeUtils.LookUpData lookUpDatum = TypeUtils._assemblyVersionLookUp;
                if (lookUpDatum == null)
                {
                    lookUpDatum =
                        new TypeUtils.LookUpData(Path.Combine(ApplicationData.ApplicationPath, "VersionLookUp.xml"));
                    TypeUtils._assemblyVersionLookUp = lookUpDatum;
                }

                return lookUpDatum;
            }
        }

        static TypeUtils()
        {
            TypeUtils._updateTypeLock = new object();
        }

        public static string UpdateType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (name.IndexOf("Metalogix", StringComparison.OrdinalIgnoreCase) == -1)
            {
                return name;
            }

            if (!TypeUtils.TypeLookUp.LookUp.ContainsKey(name))
            {
                lock (TypeUtils._updateTypeLock)
                {
                    if (!TypeUtils.TypeLookUp.LookUp.ContainsKey(name))
                    {
                        TypeUtils.TypeLookUp.LookUp.Add(name, TypeUtils.UpdateTypeWorker(name));
                    }
                }
            }

            return TypeUtils.TypeLookUp.LookUp[name];
        }

        private static string UpdateTypeWorker(string name)
        {
            Type type = null;
            Guid empty = Guid.Empty;
            string str = string.Empty;
            int num = name.IndexOf('[');
            int num1 = (num != -1 ? name.LastIndexOf(']') : -1);
            if (num != -1 && num1 > num)
            {
                empty = Guid.NewGuid();
                str = name.Substring(num, num1 - num + 1);
                name = name.Replace(str, empty.ToString());
                List<string> strs = new List<string>();
                string[] strArrays = new string[] { "],[" };
                string[] strArrays1 = str.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < (int)strArrays1.Length; i++)
                {
                    string str1 = strArrays1[i];
                    char[] chrArray = new char[] { '[', ' ', ']' };
                    string str2 = str1.Trim(chrArray);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        strs.Add(str2);
                    }
                }

                foreach (string str3 in strs)
                {
                    str = str.Replace(str3, TypeUtils.UpdateTypeWorker(str3));
                }
            }

            string str4 = null;
            string str5 = null;
            string str6 = null;
            string str7 = null;
            Match match = Regex.Match(name, "(.*?), (.*?), Version=([\\d\\.]+)", RegexOptions.Singleline);
            str6 = (match.Groups[2].Success ? match.Groups[2].Value : string.Empty);
            if (!string.IsNullOrEmpty(str6) && TypeUtils.AssemblyLookUp.LookUp.ContainsKey(str6))
            {
                name = name.Replace(str6, TypeUtils.AssemblyLookUp.LookUp[str6]);
            }

            str7 = (match.Groups[3].Success ? match.Groups[3].Value : string.Empty);
            if (!string.IsNullOrEmpty(str7))
            {
                name = name.Replace(str7,
                    (TypeUtils.VersionLookUp.LookUp.ContainsKey(str7)
                        ? TypeUtils.VersionLookUp.LookUp[str7]
                        : ConfigurationVariables.AssemblyVersionString));
            }

            string str8 = (match.Groups[1].Success ? match.Groups[1].Value : name);
            int num2 = str8.LastIndexOf('.');
            str5 = (num2 != -1 ? str8.Substring(0, num2) : string.Empty);
            if (!string.IsNullOrEmpty(str5) && TypeUtils.NamespaceLookUp.LookUp.ContainsKey(str5))
            {
                name = name.Replace(str5, TypeUtils.NamespaceLookUp.LookUp[str5]);
            }

            str4 = (num2 != -1 ? str8.Substring(num2 + 1) : str8);
            if (empty != Guid.Empty)
            {
                str4 = str4.Replace(empty.ToString(), string.Empty);
            }

            if (!string.IsNullOrEmpty(str4) && TypeUtils.NameLookUp.LookUp.ContainsKey(str4))
            {
                name = name.Replace(str4, TypeUtils.NamespaceLookUp.LookUp[str4]);
            }

            if (empty != Guid.Empty)
            {
                name = name.Replace(empty.ToString(), str);
            }

            try
            {
                type = Type.GetType(name, false);
            }
            catch
            {
            }

            if (type != null)
            {
                return type.AssemblyQualifiedName;
            }

            Assembly mainAssembly = ApplicationData.MainAssembly;
            if (mainAssembly == null)
            {
                throw new TypeLoadException(string.Concat("Unable to find type '", name,
                    "' because MainAssembly is not defined."));
            }

            List<AssemblyName> assemblyNames = new List<AssemblyName>(mainAssembly.GetReferencedAssemblies())
            {
                mainAssembly.GetName()
            };
            List<AssemblyName>.Enumerator enumerator = assemblyNames.GetEnumerator();
            try
            {
                do
                {
                    Label0:
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    Assembly assembly = Assembly.Load(enumerator.Current);
                    if (ApplicationData.IsMetalogixAssembly(assembly))
                    {
                        try
                        {
                            Type[] exportedTypes = assembly.GetExportedTypes();
                            int num3 = 0;
                            while (num3 < (int)exportedTypes.Length)
                            {
                                Type type1 = exportedTypes[num3];
                                if (type1.IsAbstract || type1.IsInterface || !string.Equals(str4, type1.Name,
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    num3++;
                                }
                                else
                                {
                                    type = type1;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        goto Label0;
                    }
                } while (type == null);
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            if (type != null)
            {
                if (empty != Guid.Empty)
                {
                    name = string.Concat(type.FullName, str);
                    try
                    {
                        type = Type.GetType(name, false);
                    }
                    catch
                    {
                    }
                }

                if (type != null)
                {
                    return type.AssemblyQualifiedName;
                }
            }

            return name;
        }

        public class LookUpData
        {
            private readonly string _filePath;
            private readonly object _lock = new object();
            private Dictionary<string, string> _lookUp;

            public LookUpData(string filePath)
            {
                this._filePath = filePath;
            }

            public Dictionary<string, string> LookUp
            {
                get
                {
                    if (this._lookUp == null)
                    {
                        lock (this._lock)
                        {
                            if (this._lookUp == null)
                            {
                                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                if (File.Exists(this._filePath))
                                {
                                    try
                                    {
                                        XmlDocument document = new XmlDocument();
                                        document.Load(this._filePath);
                                        if (document.DocumentElement != null)
                                        {
                                            foreach (XmlElement element in document.DocumentElement.ChildNodes)
                                            {
                                                string key = element.Attributes["Source"].Value;
                                                string str2 = element.Attributes["Target"].Value;
                                                if (!dictionary.ContainsKey(key))
                                                {
                                                    dictionary.Add(key, str2);
                                                }
                                            }
                                        }
                                    }
                                    catch (XmlException)
                                    {
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                this._lookUp = dictionary;
                            }
                        }
                    }

                    return this._lookUp;
                }
            }
        }
    }
}