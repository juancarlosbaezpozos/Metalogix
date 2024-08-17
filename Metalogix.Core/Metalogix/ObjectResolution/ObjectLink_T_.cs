using Metalogix.Data;
using Metalogix.ObjectResolution.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.ObjectResolution
{
    public abstract class ObjectLink<T> : IObjectLink, IXmlable
    {
        public Type ObjectResultType
        {
            get { return typeof(T); }
        }

        protected ObjectLink()
        {
        }

        public T Resolve()
        {
            return (T)ObjectResolverCatalog.GetDefaultResolver(this).ResolveObject(this);
        }

        public virtual string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public abstract void ToXML(XmlWriter xmlWriter);
    }
}