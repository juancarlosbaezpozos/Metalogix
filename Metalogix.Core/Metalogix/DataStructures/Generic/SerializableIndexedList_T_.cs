using Metalogix;
using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.DataStructures.Generic
{
    public abstract class SerializableIndexedList<T> : ICollection<T>, IEnumerable<T>, IXMLAbleList, IXmlable,
        IEnumerable, Metalogix.DataStructures.IComparable, ICloneable, IComparer<T>
    {
        private readonly object m_oLockCollection = new object();

        private readonly string m_sIndexedPropertyName;

        protected Dictionary<object, T> m_collection;

        public virtual Type CollectionType
        {
            get
            {
                if (this.Count <= 0)
                {
                    return typeof(T);
                }

                return this[0].GetType();
            }
        }

        public int Count
        {
            get { return this.m_collection.Count; }
        }

        public abstract bool IsReadOnly { get; }

        public virtual object this[int index]
        {
            get
            {
                object value;
                int num = 0;
                lock (this.m_oLockCollection)
                {
                    foreach (KeyValuePair<object, T> mCollection in this.m_collection)
                    {
                        if (num != index)
                        {
                            num++;
                        }
                        else
                        {
                            value = mCollection.Value;
                            return value;
                        }
                    }

                    return null;
                }

                return value;
            }
        }

        protected T this[object oKey]
        {
            get
            {
                T t;
                lock (this.m_oLockCollection)
                {
                    this.m_collection.TryGetValue(oKey, out t);
                }

                return t;
            }
            set
            {
                lock (this.m_oLockCollection)
                {
                    if (this.m_collection.ContainsKey(oKey))
                    {
                        this.m_collection[oKey] = value;
                    }
                }
            }
        }

        public virtual T this[T key]
        {
            get { return this[this.GetKey(key)]; }
        }

        public SerializableIndexedList(string sIndexName)
        {
            this.m_sIndexedPropertyName = sIndexName;
            this.m_collection = new Dictionary<object, T>();
        }

        public virtual void Add(T item)
        {
            this.Add(item, false);
        }

        protected void Add(T item, bool bIgnoreReadOnly)
        {
            this.AddToCollection(item, bIgnoreReadOnly);
        }

        public void AddRange(T[] items)
        {
            this.AddRange(items, false);
        }

        protected void AddRange(T[] items, bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    this.Add(tArray[i]);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, items);
            }
        }

        public void AddRangeToCollection(T[] items)
        {
            this.AddRangeToCollection(items, false);
        }

        protected void AddRangeToCollection(T[] items, bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    this.AddToCollection(tArray[i]);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, items);
            }
        }

        protected void AddToCollection(T item)
        {
            this.AddToCollection(item, false);
        }

        protected void AddToCollection(T item, bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                object key = this.GetKey(item);
                lock (this.m_oLockCollection)
                {
                    if (!this.m_collection.ContainsKey(key))
                    {
                        this.m_collection.Add(key, item);
                    }
                    else
                    {
                        this.m_collection[key] = item;
                    }
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, item);
            }
        }

        public void Clear()
        {
            this.Clear(false);
        }

        protected void Clear(bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                lock (this.m_oLockCollection)
                {
                    this.m_collection.Clear();
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Refresh, this);
            }
        }

        public virtual void ClearCollection()
        {
            this.ClearCollection(false);
        }

        protected void ClearCollection(bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                lock (this.m_oLockCollection)
                {
                    this.m_collection.Clear();
                }
            }
            finally
            {
                if (this.OnCollectionChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this);
                    this.OnCollectionChanged(this, collectionChangeEventArg);
                }
            }
        }

        public virtual object Clone()
        {
            Type type = this.GetType();
            object[] xmlNode = new object[] { XmlUtility.StringToXmlNode(this.ToXML()) };
            return Activator.CreateInstance(type, xmlNode);
        }

        public virtual int Compare(T x, T y)
        {
            if (x == null || y == null)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                return -1;
            }

            string str = string.Concat(x.GetType().Name, x.ToString());
            string str1 = string.Concat(y.GetType().Name, y.ToString());
            return string.Compare(str, str1, StringComparison.InvariantCultureIgnoreCase);
        }

        public virtual bool Contains(T item)
        {
            bool flag;
            lock (this.m_oLockCollection)
            {
                flag = this.m_collection.ContainsValue(item);
            }

            return flag;
        }

        public bool ContainsRange(T[] items)
        {
            bool flag;
            T[] tArray = items;
            int num = 0;
            Label1:
            while (num < (int)tArray.Length)
            {
                T t = tArray[num];
                lock (this.m_oLockCollection)
                {
                    if (this.m_collection.ContainsValue(t))
                    {
                        goto Label0;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                return flag;
            }

            return true;
            Label0:
            num++;
            goto Label1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.m_oLockCollection)
            {
                this.m_collection.Values.CopyTo(array, arrayIndex);
            }
        }

        public void FireOnCollectionChanged(CollectionChangeAction action, object element)
        {
            if (this.OnCollectionChanged != null)
            {
                CollectionChangeEventArgs collectionChangeEventArg = new CollectionChangeEventArgs(action, element);
                this.OnCollectionChanged(this, collectionChangeEventArg);
            }
        }

        public virtual void FromXML(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = (xmlNode.Name == "XmlableSet" ? xmlNode : xmlNode.SelectSingleNode(".//XmlableSet"));
            if (xmlNodes != null)
            {
                XmlNodeList xmlNodeLists = xmlNodes.SelectNodes("./XmlableItem");
                if (xmlNodeLists != null)
                {
                    foreach (XmlNode xmlNodes1 in xmlNodeLists)
                    {
                        XmlAttribute itemOf = xmlNodes1.Attributes["Type"];
                        if (itemOf == null)
                        {
                            continue;
                        }

                        T t = default(T);
                        Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                        if (!xmlNodes1.HasChildNodes || xmlNodes1.FirstChild.NodeType == XmlNodeType.Text)
                        {
                            try
                            {
                                t = (type.IsSubclassOf(typeof(Enum))
                                    ? (T)Enum.Parse(type, xmlNodes1.InnerText)
                                    : (T)Convert.ChangeType(xmlNodes1.InnerText, type));
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            object[] objArray = new object[] { xmlNodes1.CloneNode(true) };
                            t = (T)Activator.CreateInstance(type, objArray);
                        }

                        if (t == null)
                        {
                            continue;
                        }

                        lock (this.m_oLockCollection)
                        {
                            this.m_collection.Add(this.GetKey(t), t);
                        }
                    }
                }
            }
        }

        protected virtual object GetKey(T item)
        {
            return item.GetType().GetProperty(this.m_sIndexedPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(item, null);
        }

        public bool IsEmpty()
        {
            return this.m_collection.Count == 0;
        }

        public virtual bool IsEqual(Metalogix.DataStructures.IComparable targetComparable,
            DifferenceLog differencesOutput, ComparisonOptions options)
        {
            bool flag;
            if (typeof(T).GetInterface(typeof(Metalogix.DataStructures.IComparable).FullName) == null)
            {
                differencesOutput.Write("Set items do not implement IComparable.");
                return false;
            }

            SerializableList<T> ts = targetComparable as SerializableList<T>;
            if (ts == null)
            {
                differencesOutput.Write("Target comparable is not a compatible type.");
                return false;
            }

            if (ts.Count != this.Count)
            {
                differencesOutput.Write("Target comparable has different number of items.");
                return false;
            }

            using (IEnumerator<T> enumerator = ((IEnumerable<T>)this).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    T item = ts[current];
                    if (item != null)
                    {
                        if (((object)current as Metalogix.DataStructures.IComparable).IsEqual(
                                (object)item as Metalogix.DataStructures.IComparable, differencesOutput, options))
                        {
                            continue;
                        }

                        flag = false;
                        return flag;
                    }
                    else
                    {
                        differencesOutput.Write("Target item cannot be found.");
                        flag = false;
                        return flag;
                    }
                }

                return true;
            }

            return flag;
        }

        public virtual bool Remove(T item)
        {
            return this.RemoveFromCollection(item);
        }

        protected bool RemoveFromCollection(T item)
        {
            return this.RemoveFromCollection(item, false);
        }

        protected bool RemoveFromCollection(T item, bool bIgnoreReadOnly)
        {
            bool flag;
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                object key = this.GetKey(item);
                lock (this.m_oLockCollection)
                {
                    if (item.Equals(this.m_collection[key]))
                    {
                        flag = this.m_collection.Remove(key);
                        return flag;
                    }
                }

                flag = false;
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, item);
            }

            return flag;
        }

        public void RemoveRange(T[] items)
        {
            this.RemoveRange(items, false);
        }

        protected void RemoveRange(T[] items, bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    this.Remove(tArray[i]);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, items);
            }
        }

        public void RemoveRangeFromCollection(T[] items)
        {
            this.RemoveRangeFromCollection(items, false);
        }

        protected void RemoveRangeFromCollection(T[] items, bool bIgnoreReadOnly)
        {
            if (!bIgnoreReadOnly && this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    this.RemoveFromCollection(tArray[i]);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, items);
            }
        }

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
        {
            return this.m_collection.Values.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_collection.Values.GetEnumerator();
        }

        public T[] ToArray()
        {
            T[] tArray = new T[this.m_collection.Count];
            int num = 0;
            foreach (T value in this.m_collection.Values)
            {
                tArray[num] = value;
                num++;
            }

            return tArray;
        }

        public string ToXML()
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    this.ToXML(xmlTextWriter);
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("XmlableSet");
            lock (this.m_oLockCollection)
            {
                foreach (T value in this.m_collection.Values)
                {
                    xmlWriter.WriteStartElement("XmlableItem");
                    xmlWriter.WriteAttributeString("Type", value.GetType().AssemblyQualifiedName);
                    IXMLAbleList xMLAbleLists = (object)value as IXMLAbleList;
                    if (xMLAbleLists == null)
                    {
                        IXmlable xmlable = (object)value as IXmlable;
                        if (xmlable != null)
                        {
                            xmlable.ToXML(xmlWriter);
                        }
                        else if ((object)value is IConvertible)
                        {
                            xmlWriter.WriteValue(Convert.ChangeType(value, typeof(string)));
                        }
                    }
                    else
                    {
                        xMLAbleLists.ToXML(xmlWriter);
                    }

                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
        }

        public event CollectionChangeEventHandler OnCollectionChanged;
    }
}