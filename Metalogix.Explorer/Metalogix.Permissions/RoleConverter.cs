using Metalogix;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Permissions
{
    public abstract class RoleConverter
    {
        private static List<RoleConverter> s_mapperCollection;

        private static List<RoleConverter> MapperCollection
        {
            get
            {
                if (RoleConverter.s_mapperCollection == null)
                {
                    string signature = ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName);
                    RoleConverter.s_mapperCollection = new List<RoleConverter>();
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < (int)assemblies.Length; i++)
                    {
                        Assembly assembly = assemblies[i];
                        if (signature == ApplicationData.GetSignature(assembly.FullName))
                        {
                            try
                            {
                                Type[] exportedTypes = assembly.GetExportedTypes();
                                for (int j = 0; j < (int)exportedTypes.Length; j++)
                                {
                                    Type type = exportedTypes[j];
                                    if (type.BaseType != null && type.IsSubclassOf(typeof(RoleConverter)) &&
                                        !type.IsAbstract)
                                    {
                                        RoleConverter roleConverter = (RoleConverter)Activator.CreateInstance(type);
                                        RoleConverter.s_mapperCollection.Add(roleConverter);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                            }
                        }
                    }
                }

                return RoleConverter.s_mapperCollection;
            }
        }

        public abstract Type SourceRoleType { get; }

        public abstract Type TargetRoleType { get; }

        static RoleConverter()
        {
        }

        protected RoleConverter()
        {
        }

        public abstract Role Convert(Role sourceRole);

        public static Role ConvertRole(Role sourceRole, Type targetType)
        {
            Role role;
            if (sourceRole.GetType() == targetType)
            {
                return sourceRole;
            }

            List<RoleConverter>.Enumerator enumerator = RoleConverter.MapperCollection.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    RoleConverter current = enumerator.Current;
                    if (current.SourceRoleType != sourceRole.GetType() || current.TargetRoleType != targetType)
                    {
                        continue;
                    }

                    role = current.Convert(sourceRole);
                    return role;
                }

                return null;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return role;
        }
    }
}