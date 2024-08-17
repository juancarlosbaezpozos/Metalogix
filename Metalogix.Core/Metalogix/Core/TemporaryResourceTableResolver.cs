using Metalogix;
using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using Metalogix.ObjectResolution;
using System;

namespace Metalogix.Core
{
    [IsDefault(true)]
    public class TemporaryResourceTableResolver : ObjectResolver<ResourceTable, ResourceTableLink>
    {
        public TemporaryResourceTableResolver()
        {
        }

        public override ResourceTable ResolveTypedObject(ResourceTableLink link)
        {
            if (!ResourceTable.CachedResourceTables.ContainsKey(link))
            {
                ResourceTable.CachedResourceTables.AddSafe(link, new ResourceTable(new TemporaryTableDataResolver()));
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