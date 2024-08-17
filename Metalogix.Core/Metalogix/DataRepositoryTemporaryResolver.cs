using Metalogix.DataResolution;
using Metalogix.ObjectResolution;
using System;

namespace Metalogix
{
    public class DataRepositoryTemporaryResolver : ObjectResolver<DataResolver, DataRepositoryLink>
    {
        public DataRepositoryTemporaryResolver()
        {
        }

        public override DataResolver ResolveTypedObject(DataRepositoryLink link)
        {
            return new TemporaryTableDataResolver();
        }

        public override bool TryResolveTypedObject(DataRepositoryLink link, out DataResolver resolvedObject)
        {
            resolvedObject = new TemporaryTableDataResolver();
            return true;
        }
    }
}