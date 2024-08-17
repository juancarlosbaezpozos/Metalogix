using Metalogix;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Permissions
{
    public abstract class PrincipalConverter
    {
        private static List<PrincipalConverter> s_mapperCollection;

        private static List<PrincipalConverter> MapperCollection
        {
            get
            {
                if (PrincipalConverter.s_mapperCollection == null)
                {
                    string signature = ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName);
                    PrincipalConverter.s_mapperCollection = new List<PrincipalConverter>();
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < (int)assemblies.Length; i++)
                    {
                        Assembly assembly = assemblies[i];
                        try
                        {
                            if (signature == ApplicationData.GetSignature(assembly.FullName))
                            {
                                Type[] exportedTypes = assembly.GetExportedTypes();
                                for (int j = 0; j < (int)exportedTypes.Length; j++)
                                {
                                    Type type = exportedTypes[j];
                                    if (type.BaseType != null && type.IsSubclassOf(typeof(PrincipalConverter)) &&
                                        !type.IsAbstract)
                                    {
                                        PrincipalConverter principalConverter =
                                            (PrincipalConverter)Activator.CreateInstance(type);
                                        PrincipalConverter.s_mapperCollection.Add(principalConverter);
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }

                return PrincipalConverter.s_mapperCollection;
            }
        }

        public abstract Type SourcePrincipalType { get; }

        public abstract Type TargetPrincipalType { get; }

        static PrincipalConverter()
        {
        }

        protected PrincipalConverter()
        {
        }

        public abstract SecurityPrincipal Convert(SecurityPrincipal sourcePrincipal);

        public static SecurityPrincipal ConvertPrincipal(SecurityPrincipal sourcePrincipal, Type targetType)
        {
            SecurityPrincipal securityPrincipal;
            if (sourcePrincipal.GetType() == targetType)
            {
                return sourcePrincipal;
            }

            List<PrincipalConverter>.Enumerator enumerator = PrincipalConverter.MapperCollection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PrincipalConverter current = enumerator.Current;
                    if (current.SourcePrincipalType != sourcePrincipal.GetType() ||
                        current.TargetPrincipalType != targetType)
                    {
                        continue;
                    }

                    securityPrincipal = current.Convert(sourcePrincipal);
                    return securityPrincipal;
                }

                return null;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return securityPrincipal;
        }
    }
}