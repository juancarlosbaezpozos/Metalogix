using Metalogix.DataStructures.Generic;
using Metalogix.ObjectResolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Metalogix.Explorer
{
    public class NodeCollection : SerializableList<Node>
    {
        public override Type CollectionType
        {
            get { return this.GetHighestCommonType(); }
        }

        public static NodeCollection Empty { get; private set; }

        public bool HasNodeCollectionChangedListeners
        {
            get { return this.OnNodeCollectionChanged != null; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return true; }
        }

        public Node this[string sNodeName]
        {
            get
            {
                int indexByName = this.GetIndexByName(sNodeName);
                if (indexByName < 0)
                {
                    return null;
                }

                return (Node)this[indexByName];
            }
        }

        public override Node this[Node key]
        {
            get { return this[key.Name]; }
        }

        static NodeCollection()
        {
            NodeCollection.Empty = new NodeCollection();
        }

        public NodeCollection()
        {
        }

        public NodeCollection(IEnumerable<Node> nodes) : base(nodes)
        {
        }

        public NodeCollection(XmlNode xml)
        {
            if (xml != null)
            {
                this.FromXML(xml);
            }
        }

        public static NodeCollection CreateNodeCollection(string sXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXML);
            return new NodeCollection(xmlDocument.FirstChild);
        }

        public Node Find(Node node)
        {
            Node node1;
            using (IEnumerator<Node> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Node current = enumerator.Current;
                    if (current != node)
                    {
                        continue;
                    }

                    node1 = current;
                    return node1;
                }

                return null;
            }

            return node1;
        }

        public virtual void FireNodeCollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
            if (this.OnNodeCollectionChanged != null)
            {
                this.OnNodeCollectionChanged(changeType, changedNode);
            }
        }

        public override void FromXML(XmlNode xmlNode)
        {
            foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//Location"))
            {
                Node node = (new Location(xmlNodes)).GetNode();
                if (node == null)
                {
                    continue;
                }

                base.AddToCollection(node);
            }
        }

        private Type GetHighestCommonType()
        {
            Type type;
            Type type1 = null;
            using (IEnumerator<Node> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Node current = enumerator.Current;
                    if (current == null)
                    {
                        continue;
                    }

                    Type type2 = current.GetType();
                    if (type1 != null)
                    {
                        if (type1.IsAssignableFrom(type2))
                        {
                            continue;
                        }

                        if (!type2.IsAssignableFrom(type1))
                        {
                            Type baseType = type1.BaseType;
                            while (baseType != null)
                            {
                                if (!baseType.IsAssignableFrom(type2))
                                {
                                    baseType = baseType.BaseType;
                                }
                                else
                                {
                                    type1 = baseType;
                                    break;
                                }
                            }

                            if (baseType != null)
                            {
                                continue;
                            }

                            type = null;
                            return type;
                        }
                        else
                        {
                            type1 = type2;
                        }
                    }
                    else
                    {
                        type1 = type2;
                    }
                }

                return type1;
            }

            return type;
        }

        protected int GetIndexByName(string sNodeName)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (((Node)this[i]).Name.ToLower() == sNodeName.ToLower())
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual Node GetNodeByUrl(string sURL)
        {
            Node node;
            char chr;
            char chr1;
            int num = 0;
            Node node1 = null;
            sURL = sURL.ToUpper();
            using (IEnumerator<Node> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Node current = enumerator.Current;
                    int num1 = -1;
                    string upper = current.DisplayUrl.ToUpper();
                    if (sURL != upper)
                    {
                        do
                        {
                            num1++;
                            chr = (sURL.Length > num1 ? sURL[num1] : '\uFFFF');
                            chr1 = (upper.Length > num1 ? upper[num1] : '\uFFFF');
                        } while (chr == chr1);

                        if (num1 <= num)
                        {
                            continue;
                        }

                        num = num1;
                        node1 = current;
                    }
                    else
                    {
                        node = current;
                        return node;
                    }
                }

                if (node1 == null)
                {
                    return null;
                }

                return node1.GetNodeByUrl(sURL);
            }

            return node;
        }

        public string ToShortString()
        {
            if (base.Count == 0)
            {
                return "";
            }

            if (base.Count != 1)
            {
                return "<Multiple>";
            }

            if (this[0] == null)
            {
                return "";
            }

            return ((Node)this[0]).DisplayName;
        }

        public override string ToString()
        {
            string str;
            if (base.Count == 0)
            {
                return "";
            }

            if (base.Count != 1)
            {
                return "<Multiple>";
            }

            if (this[0] == null)
            {
                return "";
            }

            try
            {
                str = ((Node)this[0]).Location.ToString();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                str = string.Format("{0} (Invalid - {1})", ((Node)this[0]).DisplayName, exception.Message);
            }

            return str;
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("NodeCollection");
            for (int i = 0; i < base.Count; i++)
            {
                ((Node)this[i]).Location.ToXML(xmlTextWriter);
            }

            xmlTextWriter.WriteEndElement();
        }

        public event NodeCollectionChangedHandler OnNodeCollectionChanged;
    }
}