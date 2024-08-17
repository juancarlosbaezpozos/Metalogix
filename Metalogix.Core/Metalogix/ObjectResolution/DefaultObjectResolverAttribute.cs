using System;

namespace Metalogix.ObjectResolution
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DefaultObjectResolverAttribute : Attribute
    {
        public readonly Type ObjectLinkType;

        public readonly Type ObjectResultType;

        public readonly Type ResolverType;

        public DefaultObjectResolverAttribute(Type resolverType, Type objectResultType, Type objectLinkType)
        {
            this.ObjectResultType = objectResultType;
            this.ObjectLinkType = objectLinkType;
            this.ResolverType = resolverType;
        }
    }
}