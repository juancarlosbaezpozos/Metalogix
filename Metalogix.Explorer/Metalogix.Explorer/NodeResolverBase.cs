using Metalogix.ObjectResolution;
using System;

namespace Metalogix.Explorer
{
    public abstract class NodeResolverBase : ObjectResolver<Node, Location>
    {
        protected NodeResolverBase()
        {
        }

        public override bool TryResolveTypedObject(Location link, out Node resolvedObject)
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