using Metalogix;
using Metalogix.Core.ObjectResolution;
using Metalogix.ObjectResolution.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.ObjectResolution
{
    public class ObjectResolverCatalog
    {
        private static Dictionary<string, ObjectResolverCatalog.ResolverList> s_resolvers;

        public static Dictionary<string, string> OtherResolversFromPowershell { get; set; }

        static ObjectResolverCatalog()
        {
            Dictionary<string, string> resolversFromPowerShell = DefaultResolverSettings.ResolversFromPowerShell;
            if (resolversFromPowerShell == null || resolversFromPowerShell.Count <= 0)
            {
                ObjectResolverCatalog.LoadDefaultResolversFromAssembly(false);
                ObjectResolverCatalog.LoadDefaultResolversFromConfig();
                return;
            }

            ObjectResolverCatalog.LoadDefaultResolversFromAssembly(true);
            ObjectResolverCatalog.LoadResolversFromPowershell(resolversFromPowerShell);
        }

        public ObjectResolverCatalog()
        {
        }

        public static IObjectResolver GetDefaultResolver(IObjectLink link)
        {
            string resolverKey = ObjectResolverCatalog.GetResolverKey(link.ObjectResultType, link.GetType());
            ObjectResolverCatalog.ResolverList resolverList = null;
            if (!ObjectResolverCatalog.s_resolvers.TryGetValue(resolverKey, out resolverList))
            {
                return null;
            }

            return resolverList.GetDefault();
        }

        private static string GetResolverKey(IObjectResolver resolver)
        {
            return ObjectResolverCatalog.GetResolverKey(resolver.ObjectResultType, resolver.ObjectLinkType);
        }

        private static string GetResolverKey(Type resultType, Type linkType)
        {
            return string.Concat(resultType.Name, linkType.Name);
        }

        private static void LoadDefaultResolversFromAssembly(bool isPowershell)
        {
            Type[] typesByInterface = Catalogs.GetTypesByInterface(typeof(IObjectResolver),
                AssemblyTiers.Referenced | AssemblyTiers.Signed);
            ObjectResolverCatalog.s_resolvers = new Dictionary<string, ObjectResolverCatalog.ResolverList>();
            if (ApplicationData.MainAssembly != null)
            {
                foreach (DefaultObjectResolverAttribute defaultObjectResolverAttribute in ApplicationData.MainAssembly
                             .GetCustomAttributes(typeof(DefaultObjectResolverAttribute), false)
                             .Cast<DefaultObjectResolverAttribute>())
                {
                    string resolverKey = ObjectResolverCatalog.GetResolverKey(
                        defaultObjectResolverAttribute.ObjectResultType, defaultObjectResolverAttribute.ObjectLinkType);
                    ObjectResolverCatalog.ResolverList resolverList = new ObjectResolverCatalog.ResolverList();
                    resolverList.SpecifiedDefault =
                        Activator.CreateInstance(defaultObjectResolverAttribute.ResolverType) as IObjectResolver;
                    ObjectResolverCatalog.s_resolvers.Add(resolverKey, resolverList);
                    if (isPowershell)
                    {
                        continue;
                    }

                    DefaultResolverSettings.InitializeSetting(resolverKey,
                        defaultObjectResolverAttribute.ResolverType.AssemblyQualifiedName);
                }
            }

            Type[] typeArray = typesByInterface;
            for (int i = 0; i < (int)typeArray.Length; i++)
            {
                Type type = typeArray[i];
                IObjectResolver objectResolver = Activator.CreateInstance(type) as IObjectResolver;
                string str = ObjectResolverCatalog.GetResolverKey(objectResolver);
                ObjectResolverCatalog.ResolverList item = null;
                if (!ObjectResolverCatalog.s_resolvers.ContainsKey(str))
                {
                    item = new ObjectResolverCatalog.ResolverList();
                    ObjectResolverCatalog.s_resolvers.Add(str, item);
                }
                else
                {
                    item = ObjectResolverCatalog.s_resolvers[str];
                }

                item.Add(objectResolver);
                if (item.SpecifiedDefault == null)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(IsDefaultAttribute), false);
                    if ((int)customAttributes.Length > 0 && ((IsDefaultAttribute)customAttributes[0]).IsDefault)
                    {
                        item.SpecifiedDefault = objectResolver;
                    }
                }
            }
        }

        private static void LoadDefaultResolversFromConfig()
        {
            foreach (KeyValuePair<string, ObjectResolverCatalog.ResolverList> sResolver in ObjectResolverCatalog
                         .s_resolvers)
            {
                string setting = DefaultResolverSettings.GetSetting(sResolver.Key);
                if (string.IsNullOrEmpty(setting))
                {
                    continue;
                }

                ObjectResolverCatalog.SetDefaultResolver(sResolver.Key, setting);
            }
        }

        private static void LoadResolversFromPowershell(Dictionary<string, string> powershellResolvers)
        {
            ObjectResolverCatalog.OtherResolversFromPowershell = new Dictionary<string, string>();
            foreach (string key in powershellResolvers.Keys)
            {
                if (!ObjectResolverCatalog.s_resolvers.ContainsKey(key))
                {
                    ObjectResolverCatalog.OtherResolversFromPowershell.Add(key, powershellResolvers[key]);
                }
                else
                {
                    ObjectResolverCatalog.SetDefaultResolver(key, powershellResolvers[key]);
                }
            }
        }

        public static void SetDefaultResolver(string key, string resolverTypeName)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(resolverTypeName))
            {
                throw new ArgumentNullException("resolverTypeName");
            }

            Type type = Type.GetType(TypeUtils.UpdateType(resolverTypeName));
            if (type == null)
            {
                throw new Exception(string.Concat("Resolver type ", resolverTypeName, " is not found."));
            }

            IObjectResolver objectResolver = Activator.CreateInstance(type) as IObjectResolver;
            if (ObjectResolverCatalog.s_resolvers.ContainsKey(key))
            {
                ObjectResolverCatalog.s_resolvers[key].SpecifiedDefault = objectResolver;
                return;
            }

            ObjectResolverCatalog.s_resolvers.Add(key, new ObjectResolverCatalog.ResolverList()
            {
                SpecifiedDefault = objectResolver
            });
        }

        private class ResolverList : List<IObjectResolver>
        {
            public IObjectResolver SpecifiedDefault { get; set; }

            public ResolverList()
            {
            }

            public IObjectResolver GetDefault()
            {
                if (this.SpecifiedDefault != null)
                {
                    return this.SpecifiedDefault;
                }

                if (base.Count <= 0)
                {
                    return null;
                }

                return base[0];
            }
        }
    }
}