using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.DataStructures.Generic
{
    public sealed class CommonSerializableSet<X> : SerializableList<X>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return true; }
        }

        public override X this[X key]
        {
            get { return default(X); }
        }

        public CommonSerializableSet()
        {
        }

        public CommonSerializableSet(CommonSerializableSet<X> items) : base(items)
        {
        }

        public CommonSerializableSet(XmlNode node)
        {
            this.FromXML(node);
        }
    }
}