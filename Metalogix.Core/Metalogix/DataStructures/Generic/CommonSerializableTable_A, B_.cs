using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.DataStructures.Generic
{
    public sealed class CommonSerializableTable<A, B> : SerializableTable<A, B>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public CommonSerializableTable()
        {
        }

        public CommonSerializableTable(int capacity) : base(capacity)
        {
        }

        public CommonSerializableTable(KeyValuePair<A, B>[] items) : base(items)
        {
        }

        public CommonSerializableTable(SerializableTable<A, B> dictionary) : base(dictionary)
        {
        }

        public CommonSerializableTable(XmlNode node)
        {
            this.FromXML(node);
        }
    }
}