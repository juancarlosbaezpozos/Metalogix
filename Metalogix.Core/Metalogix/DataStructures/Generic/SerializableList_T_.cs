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
    public abstract class SerializableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IXMLAbleList, IXmlable,
        IEnumerable, Metalogix.DataStructures.IComparable, ICloneable, IComparer<T>
    {
        protected List<T> m_collection;

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

        public abstract bool IsSet { get; }

        public virtual bool IsSorted
        {
            get { return false; }
        }

        public virtual object this[int index]
        {
            get { return this.m_collection[index]; }
        }

        public abstract T this[T key] { get; }

        T System.Collections.Generic.IList<T>.this[int index]
        {
            get { return this.m_collection[index]; }
            set
            {
                try
                {
                    if (this.IsReadOnly)
                    {
                        throw new ArgumentException("Read Only list");
                    }

                    if (this.IsSet && this.m_collection.Contains(value) && this.m_collection.IndexOf(value) != index)
                    {
                        throw new ArgumentException("Collection does not allow duplicates.");
                    }

                    this.m_collection[index] = value;
                }
                finally
                {
                    this.FireOnCollectionChanged(CollectionChangeAction.Refresh, value);
                    if (this.IsSorted)
                    {
                        this.m_collection.Sort(this);
                    }
                }
            }
        }

        public SerializableList()
        {
            this.m_collection = new List<T>();
        }

        public SerializableList(IEnumerable<T> items)
        {
            this.m_collection = (items == null ? new List<T>() : new List<T>(items));
        }

        public SerializableList(SerializableList<T> set)
        {
            this.m_collection = (set == null ? new List<T>() : new List<T>(set.m_collection));
        }

        public virtual void Add(T item)
        {
            this.AddToCollection(item);
        }

        public void AddRange(T[] items)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    T t = tArray[i];
                    if (!this.IsSet || !this.m_collection.Contains(t))
                    {
                        this.Add(t);
                    }
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, items);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
        }

        public void AddRangeToCollection(T[] items)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    T t = tArray[i];
                    if (!this.IsSet || !this.m_collection.Contains(t))
                    {
                        this.m_collection.Add(t);
                    }
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, items);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
        }

        protected void AddToCollection(T item)
        {
            this.AddToCollection(item, true);
        }

        protected void AddToCollection(T item, bool bCheckExists)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                if (!this.IsSet || !bCheckExists || !this.m_collection.Contains(item))
                {
                    this.m_collection.Add(item);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, item);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
        }

        public void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                foreach (T t in new List<T>(this.m_collection))
                {
                    this.Remove(t);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Refresh, this);
            }
        }

        public virtual void ClearCollection()
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                this.m_collection.Clear();
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

        public bool Contains(T item)
        {
            return this.m_collection.Contains(item);
        }

        public bool ContainsRange(T[] items)
        {
            T[] tArray = items;
            for (int i = 0; i < (int)tArray.Length; i++)
            {
                T t = tArray[i];
                if (!this.m_collection.Contains(t))
                {
                    return false;
                }
            }

            return true;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.m_collection.CopyTo(array, arrayIndex);
        }

        public void FireOnCollectionChanged(CollectionChangeAction action, object element)
        {
            if (this.OnCollectionChanged != null)
            {
                CollectionChangeEventArgs collectionChangeEventArg = new CollectionChangeEventArgs(action, element);
                this.OnCollectionChanged(this, collectionChangeEventArg);
            }
        }

        public void FromXML(string sXml)
        {
            this.FromXML(XmlUtility.StringToXmlNode(sXml));
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

                        this.m_collection.Add(t);
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.m_collection.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return this.m_collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                if (!this.IsSet || !this.m_collection.Contains(item))
                {
                    this.m_collection.Insert(index, item);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Add, item);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
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

            using (IEnumerator<T> enumerator = this.GetEnumerator())
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

        public virtual void RemoveAt(int index)
        {
            this.RemoveIndex(index);
        }

        protected bool RemoveFromCollection(T item)
        {
            bool flag;
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                flag = this.m_collection.Remove(item);
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, item);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }

            return flag;
        }

        public bool RemoveIndex(int index)
        {
            bool flag;
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            T item = this.m_collection[index];
            try
            {
                this.m_collection.RemoveAt(index);
                flag = true;
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, item);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }

            return flag;
        }

        public void RemoveRange(T[] items)
        {
            if (this.IsReadOnly)
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
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
        }

        public void RemoveRangeFromCollection(T[] items)
        {
            if (this.IsReadOnly)
            {
                throw new ArgumentException("Read Only list");
            }

            try
            {
                T[] tArray = items;
                for (int i = 0; i < (int)tArray.Length; i++)
                {
                    T t = tArray[i];
                    this.m_collection.Remove(t);
                }
            }
            finally
            {
                this.FireOnCollectionChanged(CollectionChangeAction.Remove, items);
                if (this.IsSorted)
                {
                    this.m_collection.Sort(this);
                }
            }
        }

        public void Sort()
        {
            this.m_collection.Sort();
        }

        public void Sort(IComparer<T> comparer)
        {
            this.m_collection.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            this.m_collection.Sort(comparison);
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_collection.GetEnumerator();
        }

        public T[] ToArray()
        {
            return this.m_collection.ToArray();
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
            foreach (T mCollection in this.m_collection)
            {
                xmlWriter.WriteStartElement("XmlableItem");
                xmlWriter.WriteAttributeString("Type", mCollection.GetType().AssemblyQualifiedName);
                IXMLAbleList xMLAbleLists = (object)mCollection as IXMLAbleList;
                if (xMLAbleLists == null)
                {
                    IXmlable xmlable = (object)mCollection as IXmlable;
                    if (xmlable != null)
                    {
                        xmlable.ToXML(xmlWriter);
                    }
                    else if ((object)mCollection is IConvertible)
                    {
                        xmlWriter.WriteValue(Convert.ChangeType(mCollection, typeof(string)));
                    }
                }
                else
                {
                    xMLAbleLists.ToXML(xmlWriter);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        public event CollectionChangeEventHandler OnCollectionChanged;
    }
}