using Metalogix;
using Metalogix.Core.ObjectResolution;
using Metalogix.DataStructures.Generic;
using Metalogix.ObjectResolution;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace Metalogix.Core.ConfigVariables
{
    public class ResourceDatabaseTableResolver : ObjectResolver<ResourceTable, ResourceTableLink>
    {
        public readonly static string SettingsKey;

        static ResourceDatabaseTableResolver()
        {
            ResourceDatabaseTableResolver.SettingsKey =
                string.Concat(MethodBase.GetCurrentMethod().DeclaringType.Name, "Database");
        }

        public ResourceDatabaseTableResolver()
        {
        }

        public static string GetConnectionString()
        {
            string empty = string.Empty;
            Dictionary<string, string> otherResolversFromPowershell =
                ObjectResolverCatalog.OtherResolversFromPowershell;
            if (otherResolversFromPowershell == null || otherResolversFromPowershell.Count <= 0)
            {
                empty = DefaultResolverSettings.GetSetting(ResourceDatabaseTableResolver.SettingsKey);
            }
            else
            {
                otherResolversFromPowershell.TryGetValue(ResourceDatabaseTableResolver.SettingsKey, out empty);
            }

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(empty);
            if (!sqlConnectionStringBuilder.IntegratedSecurity)
            {
                sqlConnectionStringBuilder.Password = Cryptography
                    .DecryptTextusingAES(sqlConnectionStringBuilder.Password).ToInsecureString();
            }

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public override ResourceTable ResolveTypedObject(ResourceTableLink link)
        {
            ResourceScope resourceScope;
            ResourceTable item;
            try
            {
                if (!ResourceTable.CachedResourceTables.ContainsKey(link))
                {
                    if ((ResourceScope.EnvironmentSpecific & link.Scope) == ResourceScope.EnvironmentSpecific)
                    {
                        resourceScope = ResourceScope.EnvironmentSpecific;
                    }
                    else if ((ResourceScope.ApplicationSpecific & link.Scope) != ResourceScope.ApplicationSpecific)
                    {
                        resourceScope = ((ResourceScope.UserSpecific & link.Scope) != ResourceScope.UserSpecific
                            ? ResourceScope.ApplicationAndUserSpecific
                            : ResourceScope.UserSpecific);
                    }
                    else
                    {
                        resourceScope = ResourceScope.ApplicationSpecific;
                    }

                    string connectionString = ResourceDatabaseTableResolver.GetConnectionString();
                    DatabaseTableDataResolverOptions databaseTableDataResolverOption =
                        new DatabaseTableDataResolverOptions()
                        {
                            ConnectionString = connectionString,
                            Scope = resourceScope.ToString()
                        };
                    DatabaseTableDataResolverOptions databaseTableDataResolverOption1 = databaseTableDataResolverOption;
                    ResourceTable.CachedResourceTables.AddSafe(link,
                        new ResourceTable(new DatabaseTableDataResolver(databaseTableDataResolverOption1)));
                }

                item = ResourceTable.CachedResourceTables[link];
            }
            catch (Exception exception)
            {
                throw new ArgumentException(exception.ToString());
            }

            return item;
        }

        public override bool TryResolveTypedObject(ResourceTableLink link, out ResourceTable resolvedObject)
        {
            bool flag;
            try
            {
                resolvedObject = this.ResolveTypedObject(link);
                flag = true;
            }
            catch (Exception exception)
            {
                resolvedObject = null;
                flag = false;
            }

            return flag;
        }
    }
}