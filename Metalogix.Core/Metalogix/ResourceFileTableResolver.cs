using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using Metalogix.ObjectResolution;
using System;
using System.IO;

namespace Metalogix
{
    public class ResourceFileTableResolver : ObjectResolver<ResourceTable, ResourceTableLink>
    {
        public ResourceFileTableResolver()
        {
        }

        public override ResourceTable ResolveTypedObject(ResourceTableLink link)
        {
            if (!ResourceTable.CachedResourceTables.ContainsKey(link))
            {
                string commonDataPath = null;
                if ((ResourceScope.EnvironmentSpecific & link.Scope) == ResourceScope.EnvironmentSpecific)
                {
                    commonDataPath = ApplicationData.CommonDataPath;
                }
                else if ((ResourceScope.ApplicationSpecific & link.Scope) != ResourceScope.ApplicationSpecific)
                {
                    commonDataPath = ((ResourceScope.UserSpecific & link.Scope) != ResourceScope.UserSpecific
                        ? ApplicationData.ApplicationPath
                        : ApplicationData.CompanyPath);
                }
                else
                {
                    commonDataPath = ApplicationData.CommonApplicationDataPath;
                }

                commonDataPath = Path.Combine(commonDataPath, string.Concat(link.Name, ".xml"));
                ResourceTable.CachedResourceTables.AddSafe(link,
                    new ResourceTable(new FileTableDataResolver(commonDataPath)));
            }

            return ResourceTable.CachedResourceTables[link];
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