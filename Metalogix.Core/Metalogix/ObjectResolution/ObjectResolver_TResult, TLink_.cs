using Metalogix.Core.Properties;
using Metalogix.ObjectResolution.Interfaces;
using System;
using System.Reflection;

namespace Metalogix.ObjectResolution
{
    public abstract class ObjectResolver<TResult, TLink> : IObjectResolver
        where TLink : ObjectLink<TResult>
    {
        public Type ObjectLinkType
        {
            get { return typeof(TLink); }
        }

        public Type ObjectResultType
        {
            get { return typeof(TResult); }
        }

        protected ObjectResolver()
        {
        }

        protected void AssertTypeMatch(IObjectLink link)
        {
            if (!this.TryAssertTypeMatch(link))
            {
                if (link != null)
                {
                    throw new InvalidCastException(string.Format(Resources.LinkResolutionTypeMismatch,
                        link.ObjectResultType.Name, typeof(TResult).Name));
                }

                throw new ArgumentNullException(string.Format(Resources.LinkForResolutionNull, typeof(TResult).Name));
            }
        }

        public object ResolveObject(IObjectLink link)
        {
            this.AssertTypeMatch(link);
            return this.ResolveTypedObject((TLink)link);
        }

        public abstract TResult ResolveTypedObject(TLink link);

        protected bool TryAssertTypeMatch(IObjectLink link)
        {
            if (link != null && typeof(TLink).IsAssignableFrom(link.GetType()))
            {
                return true;
            }

            return false;
        }

        public abstract bool TryResolveTypedObject(TLink link, out TResult resolvedObject);
    }
}