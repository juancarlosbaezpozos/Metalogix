using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Utilities
{
    public class ReflectionUtils
    {
        public ReflectionUtils()
        {
        }

        public static T[] GetApplicationAttributesMultiple<T>()
            where T : Attribute
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                return new T[0];
            }

            object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(T), true);
            if (customAttributes != null && (int)customAttributes.Length != 0)
            {
                return (T[])customAttributes;
            }

            return new T[0];
        }

        public static IEnumerable<T> GetAttributesFromAssembly<T>(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            object[] customAttributes = assembly.GetCustomAttributes(typeof(T), true);
            if ((int)customAttributes.Length != 0)
            {
                object[] objArray = customAttributes;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    yield return (T)objArray[i];
                }
            }
        }

        public static IEnumerable<T> GetAttributesFromType<T>(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object[] customAttributes = type.GetCustomAttributes(typeof(T), true);
            if ((int)customAttributes.Length != 0)
            {
                object[] objArray = customAttributes;
                for (int i = 0; i < (int)objArray.Length; i++)
                {
                    yield return (T)objArray[i];
                }
            }
        }

        public static string GetSignature(string sAssemblyFullName)
        {
            string str = sAssemblyFullName;
            int num = str.LastIndexOf("=", StringComparison.Ordinal) + 1;
            return str.Substring(num, str.Length - num);
        }

        public static T GetSingleAttributeFromAssembly<T>(Assembly assembly)
        {
            IEnumerator<T> enumerator = ReflectionUtils.GetAttributesFromAssembly<T>(assembly).GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return default(T);
        }

        public static T GetSingleAttributeFromType<T>(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            object[] customAttributes = type.GetCustomAttributes(typeof(T), true);
            if ((int)customAttributes.Length == 0)
            {
                return default(T);
            }

            return (T)customAttributes[0];
        }

        public static IEnumerable<Type> GetTypesOfInterface<T>(Assembly assembly, bool searchReferences)
        {
            List<Type> types = new List<Type>();
            if (assembly != null && ReflectionUtils.IsMetalogixAssembly(assembly))
            {
                Type[] typeArray = assembly.GetTypes();
                for (int num = 0; num < (int)typeArray.Length; num++)
                {
                    Type type = typeArray[num];
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        if (((IEnumerable<Type>)type.GetInterfaces()).Any(i => i.FullName == typeof(T).FullName))
                        {
                            types.Add(type);
                        }
                    }
                }

                if (searchReferences)
                {
                    AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                    for (int j = 0; j < (int)referencedAssemblies.Length; j++)
                    {
                        AssemblyName assemblyName = referencedAssemblies[j];
                        types.AddRange(ReflectionUtils.GetTypesOfInterface<T>(Assembly.Load(assemblyName), false));
                    }
                }
            }

            return types;
        }

        public static IEnumerable<Type> GetTypesOfSubclass<T>(Assembly assembly, bool searchReferences)
        {
            List<Type> types = new List<Type>();
            if (assembly != null && ReflectionUtils.IsMetalogixAssembly(assembly))
            {
                Type[] typeArray = assembly.GetTypes();
                for (int i = 0; i < (int)typeArray.Length; i++)
                {
                    Type type = typeArray[i];
                    if (!type.IsAbstract && !type.IsInterface && type.IsSubclassOf(typeof(T)))
                    {
                        types.Add(type);
                    }
                }

                if (searchReferences)
                {
                    AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                    for (int j = 0; j < (int)referencedAssemblies.Length; j++)
                    {
                        AssemblyName assemblyName = referencedAssemblies[j];
                        types.AddRange(ReflectionUtils.GetTypesOfSubclass<T>(Assembly.Load(assemblyName), false));
                    }
                }
            }

            return types;
        }

        public static bool IsMetalogixAssembly(Assembly assembly)
        {
            return ReflectionUtils.IsMetalogixAssembly(assembly.FullName);
        }

        public static bool IsMetalogixAssembly(string sAssemblyFullName)
        {
            return ReflectionUtils.GetSignature(sAssemblyFullName) ==
                   ReflectionUtils.GetSignature(Assembly.GetExecutingAssembly().FullName);
        }
    }
}