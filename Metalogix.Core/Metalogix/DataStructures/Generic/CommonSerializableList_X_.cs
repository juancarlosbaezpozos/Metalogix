using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.DataStructures.Generic
{
    public sealed class CommonSerializableList<X> : SerializableList<X>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override X this[X key]
        {
            get { return default(X); }
        }

        public CommonSerializableList()
        {
        }

        public CommonSerializableList(X[] items) : base(items)
        {
        }

        public CommonSerializableList(SerializableList<X> items) : base(items)
        {
        }

        public CommonSerializableList(XmlNode node)
        {
            this.FromXML(node);
        }
    }
}