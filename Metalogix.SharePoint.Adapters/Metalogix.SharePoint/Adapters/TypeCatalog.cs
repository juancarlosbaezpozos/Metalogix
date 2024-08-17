using Metalogix;
using Metalogix.SharePoint.Adapters.Authentication;
using System;

namespace Metalogix.SharePoint.Adapters
{
    internal static class TypeCatalog
    {
        private static Type[] s_adapterTypes;

        private readonly static object s_adapterTypesLock;

        private static Type[] s_authInitTypes;

        private readonly static object s_authInitLock;

        public static Type[] AvailableAdapterTypes
        {
            get
            {
                Type[] sAdapterTypes = TypeCatalog.s_adapterTypes;
                if (sAdapterTypes == null)
                {
                    lock (TypeCatalog.s_adapterTypesLock)
                    {
                        sAdapterTypes = TypeCatalog.s_adapterTypes;
                        if (sAdapterTypes == null)
                        {
                            TypeCatalog.s_adapterTypes = Catalogs.GetSubTypesOf(typeof(SharePointAdapter),
                                AssemblyTiers.Referenced | AssemblyTiers.Signed);
                            sAdapterTypes = TypeCatalog.s_adapterTypes;
                        }
                    }
                }

                return sAdapterTypes;
            }
        }

        public static Type[] AvailableInitializerTypes
        {
            get
            {
                Type[] sAuthInitTypes = TypeCatalog.s_authInitTypes;
                if (sAuthInitTypes == null)
                {
                    lock (TypeCatalog.s_authInitLock)
                    {
                        sAuthInitTypes = TypeCatalog.s_authInitTypes;
                        if (sAuthInitTypes == null)
                        {
                            TypeCatalog.s_authInitTypes = Catalogs.GetSubTypesOf(typeof(AuthenticationInitializer),
                                AssemblyTiers.Referenced | AssemblyTiers.Signed);
                            sAuthInitTypes = TypeCatalog.s_authInitTypes;
                        }
                    }
                }

                return sAuthInitTypes;
            }
        }

        static TypeCatalog()
        {
            TypeCatalog.s_adapterTypes = null;
            TypeCatalog.s_adapterTypesLock = new object();
            TypeCatalog.s_authInitTypes = null;
            TypeCatalog.s_authInitLock = new object();
        }
    }
}