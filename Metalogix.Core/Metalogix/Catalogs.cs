using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix
{
    public static class Catalogs
    {
        private static DirectoryInfo[] s_catalogs;

        private static Assembly[] s_unsignedAssemblies;

        private static Assembly[] s_signedAssemblies;

        private static Assembly[] s_referencedAssemblies;

        private readonly static object s_cataloguedAssembliesLock;

        public static DirectoryInfo[] CatalogDirectories
        {
            get
            {
                if (Catalogs.s_catalogs == null || (int)Catalogs.s_catalogs.Length == 0)
                {
                    DirectoryInfo parent = (new DirectoryInfo(ApplicationData.MainAssembly.Location)).Parent;
                    DirectoryInfo directoryInfo = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent;
                    Catalogs.s_catalogs =
                        (!string.Equals(parent.FullName, directoryInfo.FullName, StringComparison.OrdinalIgnoreCase)
                            ? new DirectoryInfo[] { parent, directoryInfo }
                            : new DirectoryInfo[] { parent });
                }

                return Catalogs.s_catalogs;
            }
            set
            {
                if (value == null)
                {
                    Catalogs.s_catalogs = null;
                    return;
                }

                DirectoryInfo parent = (new DirectoryInfo(ApplicationData.MainAssembly.Location)).Parent;
                DirectoryInfo directoryInfo = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent;
                bool flag = value.Any<DirectoryInfo>((DirectoryInfo dir) => dir.FullName == parent.FullName);
                bool flag1 = value.Any<DirectoryInfo>((DirectoryInfo dir) => dir.FullName == directoryInfo.FullName);
                if (flag && flag1)
                {
                    Catalogs.s_catalogs = value;
                    Catalogs.ReleaseHeldAssemblies();
                    return;
                }

                List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>(value);
                if (!flag)
                {
                    directoryInfos.Add(parent);
                }

                if (!flag1 && parent.FullName != directoryInfo.FullName)
                {
                    directoryInfos.Add(directoryInfo);
                }

                Catalogs.s_catalogs = directoryInfos.ToArray();
            }
        }

        public static Assembly[] ReferencedAssemblies
        {
            get
            {
                Assembly[] sReferencedAssemblies = Catalogs.s_referencedAssemblies;
                if (sReferencedAssemblies == null)
                {
                    lock (Catalogs.s_cataloguedAssembliesLock)
                    {
                        sReferencedAssemblies = Catalogs.s_referencedAssemblies;
                        if (sReferencedAssemblies == null)
                        {
                            Catalogs.GetCataloguedAssemblies();
                            sReferencedAssemblies = Catalogs.s_referencedAssemblies;
                        }
                    }
                }

                return sReferencedAssemblies;
            }
        }

        public static Assembly[] SignedAssemblies
        {
            get
            {
                Assembly[] sSignedAssemblies = Catalogs.s_signedAssemblies;
                if (sSignedAssemblies == null)
                {
                    lock (Catalogs.s_cataloguedAssembliesLock)
                    {
                        sSignedAssemblies = Catalogs.s_signedAssemblies;
                        if (sSignedAssemblies == null)
                        {
                            Catalogs.GetCataloguedAssemblies();
                            sSignedAssemblies = Catalogs.s_signedAssemblies;
                        }
                    }
                }

                return sSignedAssemblies;
            }
        }

        public static Assembly[] UnsignedAssemblies
        {
            get
            {
                Assembly[] sUnsignedAssemblies = Catalogs.s_unsignedAssemblies;
                if (sUnsignedAssemblies == null)
                {
                    lock (Catalogs.s_cataloguedAssembliesLock)
                    {
                        sUnsignedAssemblies = Catalogs.s_unsignedAssemblies;
                        if (sUnsignedAssemblies == null)
                        {
                            Catalogs.GetCataloguedAssemblies();
                            sUnsignedAssemblies = Catalogs.s_unsignedAssemblies;
                        }
                    }
                }

                return sUnsignedAssemblies;
            }
        }

        static Catalogs()
        {
            Catalogs.s_catalogs = null;
            Catalogs.s_unsignedAssemblies = null;
            Catalogs.s_signedAssemblies = null;
            Catalogs.s_referencedAssemblies = null;
            Catalogs.s_cataloguedAssembliesLock = new object();
            ApplicationData.MainAssemblyChanged += new EventHandler(Catalogs.OnMainAssemblyChanged);
        }

        private static void GetCataloguedAssemblies()
        {
            List<Assembly> mainReferencedAssemblies = Catalogs.GetMainReferencedAssemblies();
            List<Assembly> assemblies = new List<Assembly>();
            List<Assembly> assemblies1 = new List<Assembly>();
            DirectoryInfo[] catalogDirectories = Catalogs.CatalogDirectories;
            for (int i = 0; i < (int)catalogDirectories.Length; i++)
            {
                FileInfo[] files = catalogDirectories[i].GetFiles("*.dll");
                for (int j = 0; j < (int)files.Length; j++)
                {
                    FileInfo fileInfo = files[j];
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
                        if (!mainReferencedAssemblies.Contains(assembly))
                        {
                            if (!ApplicationData.IsMetalogixAssembly(assembly))
                            {
                                assemblies.Add(assembly);
                            }
                            else
                            {
                                assemblies1.Add(assembly);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            Catalogs.s_unsignedAssemblies = assemblies.ToArray();
            Catalogs.s_signedAssemblies = assemblies1.ToArray();
            Catalogs.s_referencedAssemblies = mainReferencedAssemblies.ToArray();
        }

        private static void GetImplementersOfInterface(Type interfaceType, IEnumerable<Assembly> assemblies,
            List<Type> listToFill)
        {
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    for (int i = 0; i < (int)exportedTypes.Length; i++)
                    {
                        Type type = exportedTypes[i];
                        if (type.BaseType != null && !type.IsAbstract &&
                            type.GetInterface(interfaceType.FullName) != null)
                        {
                            listToFill.Add(type);
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        private static List<Assembly> GetMainReferencedAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            try
            {
                Assembly mainAssembly = ApplicationData.MainAssembly;
                if (mainAssembly == null)
                {
                    Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
                    List<string> strs = new List<string>();
                    Assembly[] assemblyArray1 = assemblyArray;
                    for (int i = 0; i < (int)assemblyArray1.Length; i++)
                    {
                        Assembly assembly = assemblyArray1[i];
                        if (ApplicationData.IsMetalogixAssembly(assembly.GetName().FullName))
                        {
                            assemblies.Add(assembly);
                            strs.Add(assembly.GetName().FullName);
                            Catalogs.GetReferencedAssembliesRecursive(assembly, strs, assemblies);
                        }
                    }
                }
                else
                {
                    assemblies.Add(mainAssembly);
                    List<string> strs1 = new List<string>()
                    {
                        mainAssembly.GetName().FullName
                    };
                    Catalogs.GetReferencedAssembliesRecursive(mainAssembly, strs1, assemblies);
                }
            }
            catch (Exception exception)
            {
            }

            return assemblies;
        }

        private static void GetReferencedAssembliesRecursive(Assembly currentAssembly, List<string> encounteredNames,
            List<Assembly> assemblies)
        {
            Assembly assembly;
            List<Assembly> assemblies1 = new List<Assembly>();
            AssemblyName[] referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            for (int i = 0; i < (int)referencedAssemblies.Length; i++)
            {
                AssemblyName assemblyName = referencedAssemblies[i];
                if (ApplicationData.IsMetalogixAssembly(assemblyName.FullName) &&
                    !encounteredNames.Contains(assemblyName.FullName))
                {
                    encounteredNames.Add(assemblyName.FullName);
                    try
                    {
                        assembly = Assembly.Load(assemblyName);
                    }
                    catch (Exception exception)
                    {
                        continue;
                    }

                    assemblies1.Add(assembly);
                }
            }

            assemblies.AddRange(assemblies1);
            foreach (Assembly assembly1 in assemblies1)
            {
                Catalogs.GetReferencedAssembliesRecursive(assembly1, encounteredNames, assemblies);
            }
        }

        private static void GetSubTypesFromAssemblies(Type baseType, IEnumerable<Assembly> assemblies,
            List<Type> listToFill)
        {
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    for (int i = 0; i < (int)exportedTypes.Length; i++)
                    {
                        Type type = exportedTypes[i];
                        if (type.BaseType != null && !type.IsAbstract && type.IsSubclassOf(baseType))
                        {
                            listToFill.Add(type);
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        public static Type[] GetSubTypesOf(Type baseType, AssemblyTiers tiersToSearch)
        {
            List<Type> types = new List<Type>();
            if ((int)(tiersToSearch & AssemblyTiers.Referenced) > 0)
            {
                Catalogs.GetSubTypesFromAssemblies(baseType, Catalogs.ReferencedAssemblies, types);
            }

            if ((int)(tiersToSearch & AssemblyTiers.Signed) > 0)
            {
                Catalogs.GetSubTypesFromAssemblies(baseType, Catalogs.SignedAssemblies, types);
            }

            if ((int)(tiersToSearch & AssemblyTiers.Unsigned) > 0)
            {
                Catalogs.GetSubTypesFromAssemblies(baseType, Catalogs.UnsignedAssemblies, types);
            }

            return types.ToArray();
        }

        public static Type[] GetTypesByInterface(Type implementedInterface, AssemblyTiers tiersToSearch)
        {
            List<Type> types = new List<Type>();
            if ((int)(tiersToSearch & AssemblyTiers.Referenced) > 0)
            {
                Catalogs.GetImplementersOfInterface(implementedInterface, Catalogs.ReferencedAssemblies, types);
            }

            if ((int)(tiersToSearch & AssemblyTiers.Signed) > 0)
            {
                Catalogs.GetImplementersOfInterface(implementedInterface, Catalogs.SignedAssemblies, types);
            }

            if ((int)(tiersToSearch & AssemblyTiers.Unsigned) > 0)
            {
                Catalogs.GetImplementersOfInterface(implementedInterface, Catalogs.UnsignedAssemblies, types);
            }

            return types.ToArray();
        }

        private static void OnMainAssemblyChanged(object sender, EventArgs e)
        {
            Catalogs.s_catalogs = null;
            Catalogs.ReleaseHeldAssemblies();
        }

        private static void ReleaseHeldAssemblies()
        {
            Catalogs.s_unsignedAssemblies = null;
            Catalogs.s_signedAssemblies = null;
            Catalogs.s_referencedAssemblies = null;
        }
    }
}