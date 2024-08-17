using Metalogix;
using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
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
    public abstract class SerializableTable<X, Y> : IDictionary<X, Y>, ICollection<KeyValuePair<X, Y>>,
        IEnumerable<KeyValuePair<X, Y>>, IXMLAbleList, IXmlable, IEnumerable, Metalogix.DataStructures.IComparable,
        ICloneable
    {
        protected IDictionary<X, Y> m_dictionary;

        public virtual Type CollectionType
        {
            get { return typeof(Y); }
        }

        public int Count
        {
            get { return this.m_dictionary.Count; }
        }

        public abstract bool IsReadOnly { get; }

        public virtual Y this[X key]
        {
            get { return this.m_dictionary[key]; }
            set
            {
                if (this.IsReadOnly)
                {
                    throw new Exception("This is a Read Only dictionary");
                }

                if (this.m_dictionary.ContainsKey(key))
                {
                    this.m_dictionary[key] = value;
                    return;
                }

                this.m_dictionary.Add(key, value);
            }
        }

        public virtual object this[int index]
        {
            get
            {
                object obj;
                if (this.m_dictionary.Count < index || index < 0)
                {
                    return null;
                }

                int num = 0;
                using (IEnumerator<X> enumerator = this.m_dictionary.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        X current = enumerator.Current;
                        if (num == index)
                        {
                            obj = current;
                            return obj;
                        }
                        else
                        {
                            num++;
                        }
                    }

                    return null;
                }

                return obj;
            }
            set
            {
                if (this.m_dictionary.Count < index || index < 0)
                {
                    return;
                }

                int num = 0;
                foreach (X key in this.m_dictionary.Keys)
                {
                    if (num == index)
                    {
                        this.m_dictionary[key] = (Y)value;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
        }

        public ICollection<X> Keys
        {
            get { return this.m_dictionary.Keys; }
        }

        public ICollection<Y> Values
        {
            get { return this.m_dictionary.Values; }
        }

        public SerializableTable()
        {
            this.m_dictionary = new Dictionary<X, Y>();
        }

        public SerializableTable(int capacity)
        {
            this.m_dictionary = new Dictionary<X, Y>(capacity);
        }

        public SerializableTable(KeyValuePair<X, Y>[] items)
        {
            this.m_dictionary = new Dictionary<X, Y>((int)items.Length);
            KeyValuePair<X, Y>[] keyValuePairArray = items;
            for (int i = 0; i < (int)keyValuePairArray.Length; i++)
            {
                KeyValuePair<X, Y> keyValuePair = keyValuePairArray[i];
                this.m_dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SerializableTable(SerializableTable<X, Y> dictionary)
        {
            this.m_dictionary = (dictionary == null ? new Dictionary<X, Y>() : new Dictionary<X, Y>(dictionary));
        }

        public void Add(X key, Y value)
        {
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            try
            {
                this.m_dictionary.Add(key, value);
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Add,
                            (object)(new KeyValuePair<X, Y>(key, value)));
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }
        }

        public void Add(KeyValuePair<X, Y> item)
        {
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            try
            {
                this.m_dictionary.Add(item.Key, item.Value);
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Add, (object)item);
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }
        }

        public void AddRange(KeyValuePair<X, Y>[] items)
        {
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            try
            {
                KeyValuePair<X, Y>[] keyValuePairArray = items;
                for (int i = 0; i < (int)keyValuePairArray.Length; i++)
                {
                    KeyValuePair<X, Y> keyValuePair = keyValuePairArray[i];
                    this.m_dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Add, items);
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }
        }

        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            try
            {
                this.m_dictionary.Clear();
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this);
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }
        }

        public virtual object Clone()
        {
            return new CommonSerializableTable<X, Y>(this);
        }

        public bool Contains(KeyValuePair<X, Y> item)
        {
            return this.m_dictionary.Contains(item);
        }

        public bool ContainsKey(X key)
        {
            return this.m_dictionary.ContainsKey(key);
        }

        public bool ContainsValue(Y value)
        {
            return this.m_dictionary.Values.Contains(value);
        }

        public void CopyTo(KeyValuePair<X, Y>[] array, int arrayIndex)
        {
            int num = 0;
            IEnumerator<KeyValuePair<X, Y>> enumerator = this.m_dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                try
                {
                    array[num + arrayIndex] = enumerator.Current;
                }
                finally
                {
                    num++;
                }
            }
        }

        public virtual void FromXML(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = (xmlNode.Name == "XmlableTable" ? xmlNode : xmlNode.SelectSingleNode(".//XmlableTable"));
            if (xmlNodes != null)
            {
                XmlNodeList xmlNodeLists = xmlNodes.SelectNodes("./XmlableEntry");
                if (xmlNodeLists != null)
                {
                    foreach (XmlNode xmlNodes1 in xmlNodeLists)
                    {
                        X x = default(X);
                        Y y = default(Y);
                        XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode("./Key");
                        if (xmlNodes2 != null)
                        {
                            XmlAttribute itemOf = xmlNodes2.Attributes["Type"];
                            if (itemOf != null)
                            {
                                Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                                if (xmlNodes2.HasChildNodes && xmlNodes2.FirstChild.NodeType != XmlNodeType.Text)
                                {
                                    object[] objArray = new object[] { xmlNodes2.CloneNode(true) };
                                    x = (X)Activator.CreateInstance(type, objArray);
                                }
                                else if (type.GetInterface("IConvertible") == null)
                                {
                                    ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(string) });
                                    if (constructor != null)
                                    {
                                        object[] innerText = new object[] { xmlNodes2.InnerText };
                                        x = (X)constructor.Invoke(innerText);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        x = (type.IsSubclassOf(typeof(Enum))
                                            ? (X)Enum.Parse(type, xmlNodes2.InnerText)
                                            : (X)Convert.ChangeType(xmlNodes2.InnerText, type));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            else if (typeof(X) == typeof(string))
                            {
                                x = (X)Convert.ChangeType(xmlNodes2.InnerText, typeof(string));
                            }
                        }

                        XmlNode xmlNodes3 = xmlNodes1.SelectSingleNode("./Value");
                        if (xmlNodes3 != null)
                        {
                            XmlAttribute xmlAttribute = xmlNodes3.Attributes["Type"];
                            if (xmlAttribute != null)
                            {
                                Type type1 = Type.GetType(TypeUtils.UpdateType(xmlAttribute.Value));
                                if (xmlNodes3.HasChildNodes && xmlNodes3.FirstChild.NodeType != XmlNodeType.Text)
                                {
                                    object[] objArray1 = new object[] { xmlNodes3.CloneNode(true) };
                                    y = (Y)Activator.CreateInstance(type1, objArray1);
                                }
                                else if (type1.GetInterface("IConvertible") == null)
                                {
                                    ConstructorInfo constructorInfo =
                                        type1.GetConstructor(new Type[] { typeof(string) });
                                    if (constructorInfo != null)
                                    {
                                        object[] innerText1 = new object[] { xmlNodes3.InnerText };
                                        y = (Y)constructorInfo.Invoke(innerText1);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        y = (type1.IsSubclassOf(typeof(Enum))
                                            ? (Y)Enum.Parse(type1, xmlNodes3.InnerText)
                                            : (Y)Convert.ChangeType(xmlNodes3.InnerText, type1));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            else if (typeof(Y) == typeof(string))
                            {
                                y = (Y)Convert.ChangeType(xmlNodes3.InnerText, typeof(string));
                            }
                        }

                        if (x == null || y == null)
                        {
                            continue;
                        }

                        this.m_dictionary.Add(x, y);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<X, Y>> GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        public bool IsEmpty()
        {
            return this.m_dictionary.Count == 0;
        }

        public virtual bool IsEqual(Metalogix.DataStructures.IComparable targetComparable,
            DifferenceLog differencesOutput, ComparisonOptions options)
        {
            if (typeof(Y).GetInterface(typeof(IConvertible).FullName) != null)
            {
                return this.isEqualIConvertible(targetComparable, differencesOutput, options);
            }

            if (typeof(Y).GetInterface(typeof(Metalogix.DataStructures.IComparable).FullName) == null)
            {
                differencesOutput.Write("Value items do not implement IComparable.");
                return false;
            }

            SerializableTable<X, Y> xes = targetComparable as SerializableTable<X, Y>;
            if (xes == null)
            {
                differencesOutput.Write("Target comparable is not a compatible type.");
                return false;
            }

            if (xes.Count != this.Count)
            {
                differencesOutput.Write("Target comparable has different number of items.");
                return false;
            }

            IEnumerator<KeyValuePair<X, Y>> enumerator = this.m_dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!xes.ContainsKey(enumerator.Current.Key))
                {
                    differencesOutput.Write("Target key cannot be found.");
                    return false;
                }

                Y item = xes[enumerator.Current.Key];
                if (item == null)
                {
                    differencesOutput.Write("Target value cannot be found.");
                    return false;
                }

                KeyValuePair<X, Y> current = enumerator.Current;
                if (((object)current.Value as Metalogix.DataStructures.IComparable).IsEqual(
                        (object)item as Metalogix.DataStructures.IComparable, differencesOutput, options))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private bool isEqualIConvertible(Metalogix.DataStructures.IComparable targetComparable,
            DifferenceLog differencesOutput, ComparisonOptions options)
        {
            SerializableTable<X, Y> xes = targetComparable as SerializableTable<X, Y>;
            if (xes == null)
            {
                differencesOutput.Write("Target comparable is not a compatible type.");
                return false;
            }

            if (xes.Count != this.Count)
            {
                differencesOutput.Write("Target comparable has different number of items.");
                return false;
            }

            IEnumerator<KeyValuePair<X, Y>> enumerator = this.m_dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!xes.ContainsKey(enumerator.Current.Key))
                {
                    differencesOutput.Write("Target key cannot be found.");
                    return false;
                }

                Y item = xes[enumerator.Current.Key];
                if (item == null)
                {
                    differencesOutput.Write("Target value cannot be found.");
                    return false;
                }

                if (item.Equals(enumerator.Current.Value))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public bool Remove(X key)
        {
            bool flag;
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            KeyValuePair<X, Y> keyValuePair = new KeyValuePair<X, Y>(key, this.m_dictionary[key]);
            try
            {
                flag = this.m_dictionary.Remove(key);
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Remove, (object)keyValuePair);
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }

            return flag;
        }

        public bool Remove(KeyValuePair<X, Y> item)
        {
            bool flag;
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            try
            {
                flag = this.m_dictionary.Remove(item.Key);
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Remove, (object)item);
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }

            return flag;
        }

        public void RemoveRange(X[] keys)
        {
            if (this.IsReadOnly)
            {
                throw new Exception("This is a Read Only dictionary");
            }

            List<KeyValuePair<X, Y>> keyValuePairs = new List<KeyValuePair<X, Y>>();
            try
            {
                X[] xArray = keys;
                for (int i = 0; i < (int)xArray.Length; i++)
                {
                    X x = xArray[i];
                    KeyValuePair<X, Y> keyValuePair = new KeyValuePair<X, Y>(x, this.m_dictionary[x]);
                    this.m_dictionary.Remove(keyValuePair.Key);
                    keyValuePairs.Add(keyValuePair);
                }
            }
            finally
            {
                if (this.OnDictionaryChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Remove, keyValuePairs.ToArray());
                    this.OnDictionaryChanged(this, collectionChangeEventArg);
                }
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        public KeyValuePair<X, Y>[] ToArray()
        {
            KeyValuePair<X, Y>[] keyValuePairArray = new KeyValuePair<X, Y>[this.Count];
            this.CopyTo(keyValuePairArray, 0);
            return keyValuePairArray;
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
            xmlWriter.WriteStartElement("XmlableTable");
            IEnumerator<KeyValuePair<X, Y>> enumerator = this.m_dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                xmlWriter.WriteStartElement("XmlableEntry");
                KeyValuePair<X, Y> current = enumerator.Current;
                X key = current.Key;
                bool flag = key.GetType().GetConstructor(new Type[]
                {
                    typeof(string)
                }) != null;
                KeyValuePair<X, Y> current2 = enumerator.Current;
                Y value = current2.Value;
                bool flag2 = value.GetType().GetConstructor(new Type[]
                {
                    typeof(string)
                }) != null;
                KeyValuePair<X, Y> current3 = enumerator.Current;
                if (current3.Key is IXMLAbleList)
                {
                    goto IL_FE;
                }

                KeyValuePair<X, Y> current4 = enumerator.Current;
                if (current4.Key is IXmlable)
                {
                    goto IL_FE;
                }

                KeyValuePair<X, Y> current5 = enumerator.Current;
                if (current5.Key is IConvertible || flag)
                {
                    goto IL_FE;
                }

                goto IL_14F;
                IL_443:
                xmlWriter.WriteEndElement();
                continue;
                IL_14F:
                if (!flag2)
                {
                    goto IL_443;
                }

                IL_155:
                xmlWriter.WriteStartElement("Key");
                KeyValuePair<X, Y> current6 = enumerator.Current;
                X key2 = current6.Key;
                if (key2.GetType() == typeof(string))
                {
                    KeyValuePair<X, Y> current7 = enumerator.Current;
                    X key3 = current7.Key;
                    xmlWriter.WriteValue(key3.ToString());
                }
                else
                {
                    string arg_1DC_1 = "Type";
                    KeyValuePair<X, Y> current8 = enumerator.Current;
                    X key4 = current8.Key;
                    xmlWriter.WriteAttributeString(arg_1DC_1, key4.GetType().AssemblyQualifiedName);
                    KeyValuePair<X, Y> current9 = enumerator.Current;
                    if (current9.Key is IXMLAbleList)
                    {
                        KeyValuePair<X, Y> current10 = enumerator.Current;
                        (current10.Key as IXMLAbleList).ToXML(xmlWriter);
                    }
                    else
                    {
                        KeyValuePair<X, Y> current11 = enumerator.Current;
                        if (current11.Key is IXmlable)
                        {
                            KeyValuePair<X, Y> current12 = enumerator.Current;
                            (current12.Key as IXmlable).ToXML(xmlWriter);
                        }
                        else
                        {
                            KeyValuePair<X, Y> current13 = enumerator.Current;
                            if (current13.Key is IConvertible)
                            {
                                KeyValuePair<X, Y> current14 = enumerator.Current;
                                xmlWriter.WriteValue(Convert.ChangeType(current14.Key, typeof(string)));
                            }
                            else
                            {
                                KeyValuePair<X, Y> current15 = enumerator.Current;
                                X key5 = current15.Key;
                                xmlWriter.WriteValue(key5.ToString());
                            }
                        }
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Value");
                KeyValuePair<X, Y> current16 = enumerator.Current;
                Y value2 = current16.Value;
                if (value2.GetType() == typeof(string))
                {
                    KeyValuePair<X, Y> current17 = enumerator.Current;
                    Y value3 = current17.Value;
                    xmlWriter.WriteValue(value3.ToString());
                }
                else
                {
                    string arg_353_1 = "Type";
                    KeyValuePair<X, Y> current18 = enumerator.Current;
                    Y value4 = current18.Value;
                    xmlWriter.WriteAttributeString(arg_353_1, value4.GetType().AssemblyQualifiedName);
                    KeyValuePair<X, Y> current19 = enumerator.Current;
                    if (current19.Value is IXMLAbleList)
                    {
                        KeyValuePair<X, Y> current20 = enumerator.Current;
                        (current20.Value as IXMLAbleList).ToXML(xmlWriter);
                    }
                    else
                    {
                        KeyValuePair<X, Y> current21 = enumerator.Current;
                        if (current21.Value is IXmlable)
                        {
                            KeyValuePair<X, Y> current22 = enumerator.Current;
                            (current22.Value as IXmlable).ToXML(xmlWriter);
                        }
                        else
                        {
                            KeyValuePair<X, Y> current23 = enumerator.Current;
                            if (current23.Value is IConvertible)
                            {
                                KeyValuePair<X, Y> current24 = enumerator.Current;
                                xmlWriter.WriteValue(Convert.ChangeType(current24.Value, typeof(string)));
                            }
                            else
                            {
                                KeyValuePair<X, Y> current25 = enumerator.Current;
                                Y value5 = current25.Value;
                                xmlWriter.WriteValue(value5.ToString());
                            }
                        }
                    }
                }

                xmlWriter.WriteEndElement();
                goto IL_443;
                IL_FE:
                KeyValuePair<X, Y> current26 = enumerator.Current;
                if (current26.Value is IXMLAbleList)
                {
                    goto IL_155;
                }

                KeyValuePair<X, Y> current27 = enumerator.Current;
                if (current27.Value is IXmlable)
                {
                    goto IL_155;
                }

                KeyValuePair<X, Y> current28 = enumerator.Current;
                if (!(current28.Value is IConvertible))
                {
                    goto IL_14F;
                }

                goto IL_155;
            }

            xmlWriter.WriteEndElement();
        }

        public bool TryGetValue(X key, out Y value)
        {
            return this.m_dictionary.TryGetValue(key, out value);
        }

        public event CollectionChangeEventHandler OnDictionaryChanged;
    }
}