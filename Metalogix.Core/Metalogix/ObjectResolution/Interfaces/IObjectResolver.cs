using System;

namespace Metalogix.ObjectResolution.Interfaces
{
    public interface IObjectResolver
    {
        Type ObjectLinkType { get; }

        Type ObjectResultType { get; }

        object ResolveObject(IObjectLink link);
    }
}