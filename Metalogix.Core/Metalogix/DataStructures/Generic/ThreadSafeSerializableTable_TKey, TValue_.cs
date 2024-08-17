using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.DataStructures.Generic
{
    public sealed class ThreadSafeSerializableTable<TKey, TValue> : SerializableTable<TKey, TValue>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public ThreadSafeSerializableTable()
        {
            this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>();
        }

        public ThreadSafeSerializableTable(int capacity)
        {
            this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>(capacity);
        }

        public ThreadSafeSerializableTable(ICollection<KeyValuePair<TKey, TValue>> items)
        {
            this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>(items.Count);
            foreach (KeyValuePair<TKey, TValue> item in items)
            {
                this.m_dictionary.Add(item.Key, item.Value);
            }
        }

        public ThreadSafeSerializableTable(SerializableTable<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>();
            }
            else
            {
                this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>(dictionary.Count);
                foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
                {
                    this.m_dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public ThreadSafeSerializableTable(XmlNode node)
        {
            this.m_dictionary = new ThreadSafeDictionary<TKey, TValue>();
            if (node != null)
            {
                this.FromXML(node);
            }
        }
    }
}