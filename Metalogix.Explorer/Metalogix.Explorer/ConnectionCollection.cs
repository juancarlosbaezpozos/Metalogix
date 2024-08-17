using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.Explorer
{
    public class ConnectionCollection : NodeCollection
    {
        public ConnectionCollection()
        {
        }

        public ConnectionCollection(XmlNode xmlConnectionCollection)
        {
            if (xmlConnectionCollection == null)
            {
                return;
            }

            XmlNodeList xmlNodeLists = xmlConnectionCollection.SelectNodes("./Connection");
            if (xmlNodeLists == null)
            {
                return;
            }

            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                try
                {
                    this.Add(ConnectionFactory.CreateConnection(xmlNodes));
                }
                catch (Exception exception)
                {
                }
            }
        }

        public void Add(Connection connection)
        {
            base.AddToCollection((Node)connection);
            this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, (Node)connection);
        }

        public void AddConnectionCollection(ConnectionCollection collection)
        {
            base.AddRangeToCollection(collection.ToArray());
            this.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
        }

        public override void ClearCollection()
        {
            foreach (Connection connection in this)
            {
                connection.Close();
            }

            base.ClearCollection();
        }

        public void ClearConnections()
        {
            this.ClearCollection();
            this.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
        }

        public Connection GetByConnString(string sConn)
        {
            Connection connection;
            XmlNode xmlNode = XmlUtility.StringToXmlNode(sConn);
            using (IEnumerator<Node> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Connection current = (Connection)enumerator.Current;
                    if (!current.ConnectionEquals(xmlNode))
                    {
                        continue;
                    }

                    connection = current;
                    return connection;
                }

                return null;
            }

            return connection;
        }

        public override Node GetNodeByUrl(string sURL)
        {
            Node nodeByUrl = null;
            using (IEnumerator<Node> enumerator = base.GetEnumerator())
            {
                do
                {
                    Label0:
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    Connection current = (Connection)enumerator.Current;
                    if (typeof(Node).IsAssignableFrom(current.GetType()) && current.Status == ConnectionStatus.Valid)
                    {
                        nodeByUrl = ((Node)current).GetNodeByUrl(sURL);
                    }
                    else
                    {
                        goto Label0;
                    }
                } while (nodeByUrl == null);
            }

            return nodeByUrl;
        }

        public void Remove(Connection connection)
        {
            base.RemoveFromCollection(connection.Node);
            this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, connection.Node);
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ConnectionCollection");
            foreach (Connection connection in this)
            {
                xmlWriter.WriteRaw(connection.ConnectionString);
            }

            xmlWriter.WriteEndElement();
        }
    }
}