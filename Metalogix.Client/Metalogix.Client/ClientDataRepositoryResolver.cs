using Metalogix;
using Metalogix.DataResolution;
using Metalogix.ObjectResolution;
using System;

namespace Metalogix.Client
{
    public class ClientDataRepositoryResolver : ObjectResolver<DataResolver, DataRepositoryLink>
    {
        public ClientDataRepositoryResolver()
        {
        }

        public override DataResolver ResolveTypedObject(DataRepositoryLink link)
        {
            return new FolderDataResolver(ApplicationData.ApplicationPath);
        }

        public override bool TryResolveTypedObject(DataRepositoryLink link, out DataResolver resolvedObject)
        {
            bool flag;
            try
            {
                resolvedObject = this.ResolveTypedObject(link);
                return true;
            }
            catch
            {
                resolvedObject = null;
                flag = false;
            }

            return flag;
        }
    }
}