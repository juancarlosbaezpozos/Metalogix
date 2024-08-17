using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Licensing;
using Metalogix.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace Metalogix.Explorer
{
    public abstract class Settings
    {
        private const string CONNECTION_ASSOCIATIONS_KEY = "ConnectionAssociations";

        private const string ACTIVE_CONNECTIONS_KEY = "ActiveConnections";

        private static object s_instanceLock;

        private static ConnectionCollection s_activeConnections;

        private static Settings.MappingCollection s_connectionAssociations;

        public static ConnectionCollection ActiveConnections
        {
            get
            {
                if (Settings.s_activeConnections == null)
                {
                    lock (Settings.s_instanceLock)
                    {
                        if (Settings.s_activeConnections == null)
                        {
                            try
                            {
                                try
                                {
                                    string str = DataRepository.ReadDataAsString("ActiveConnections");
                                    if (string.IsNullOrEmpty(str))
                                    {
                                        Settings.s_activeConnections = new ConnectionCollection();
                                    }
                                    else
                                    {
                                        XmlDocument xmlDocument = new XmlDocument();
                                        xmlDocument.LoadXml(str);
                                        Settings.s_activeConnections = new ConnectionCollection(xmlDocument.FirstChild);
                                        if (ExplorerConfigurationVariables.UpgradeActiveConnections)
                                        {
                                            Settings.UpgradeActiveConnections();
                                            ExplorerConfigurationVariables.UpgradeActiveConnections = false;
                                        }
                                    }
                                }
                                catch (Exception exception1)
                                {
                                    Exception exception = exception1;
                                    Settings.s_activeConnections = new ConnectionCollection();
                                    Trace.WriteLine(string.Concat("Failed to load active connections: ", exception));
                                }
                            }
                            finally
                            {
                                if (Settings.s_activeConnections != null)
                                {
                                    Settings.s_activeConnections.OnNodeCollectionChanged +=
                                        new NodeCollectionChangedHandler(
                                            Settings.On_activeConnections_CollectionChanged);
                                }
                            }
                        }
                    }
                }

                return Settings.s_activeConnections;
            }
        }

        public static Settings.MappingCollection ConnectionAssociations
        {
            get
            {
                if (Settings.s_connectionAssociations == null)
                {
                    string str = DataRepository.ReadDataAsString("ConnectionAssociations");
                    if (string.IsNullOrEmpty(str))
                    {
                        Settings.s_connectionAssociations = new Settings.MappingCollection();
                    }
                    else
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(str);
                        Settings.s_connectionAssociations = new Settings.MappingCollection(xmlDocument.FirstChild);
                    }
                }

                return Settings.s_connectionAssociations;
            }
        }

        static Settings()
        {
            Settings.s_instanceLock = new object();
            MLLicenseProvider.Instance.LicenseUpdated += new EventHandler(Settings.OnLicenseUpdated);
        }

        protected Settings()
        {
        }

        private static void On_activeConnections_CollectionChanged(NodeCollectionChangeType changeType,
            Node changedNode)
        {
            Settings.SaveActiveConnections();
            if (Settings.ActiveConnectionsChanged != null)
            {
                Settings.ActiveConnectionsChanged(changeType, changedNode);
            }
        }

        private static void OnLicenseUpdated(object caller, EventArgs args)
        {
            foreach (Connection activeConnection in Settings.ActiveConnections)
            {
                activeConnection.UpdateLicensedStatus();
            }
        }

        public static void RefreshActiveConnections()
        {
            ConnectionCollection connectionCollection = null;
            string str = DataRepository.ReadDataAsString("ActiveConnections");
            if (string.IsNullOrEmpty(str))
            {
                connectionCollection = new ConnectionCollection();
            }
            else
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(str);
                connectionCollection = new ConnectionCollection(xmlDocument.FirstChild);
            }

            if (Settings.s_activeConnections == null)
            {
                Settings.s_activeConnections = new ConnectionCollection();
            }

            for (int i = 0; i < connectionCollection.Count; i++)
            {
                Connection item = (Connection)connectionCollection[i];
                if (!item.Node.Credentials.IsDefault)
                {
                    item.Node.Credentials = ((Connection)Settings.s_activeConnections[i]).Node.Credentials;
                }
            }

            Settings.s_activeConnections.ClearCollection();
            Settings.s_activeConnections.AddConnectionCollection(connectionCollection);
        }

        public static void SaveActiveConnections()
        {
            if (ApplicationData.IsWeb)
            {
                return;
            }

            DataRepository.WriteDataAsString("ActiveConnections", Settings.ActiveConnections.ToXML());
        }

        public static void SaveAssociatedConnections()
        {
            if (ApplicationData.IsWeb)
            {
                return;
            }

            DataRepository.WriteDataAsString("ConnectionAssociations", Settings.ConnectionAssociations.ToXML());
        }

        public static void UpgradeActiveConnections()
        {
            Settings.SaveActiveConnections();
        }

        public static event NodeCollectionChangedHandler ActiveConnectionsChanged;

        public class MappingCollection : SerializableTable<object, object>
        {
            public override bool IsReadOnly
            {
                get { return false; }
            }

            public MappingCollection()
            {
            }

            public MappingCollection(XmlNode node)
            {
                this.FromXML(node);
            }

            public override void FromXML(XmlNode xmlNode)
            {
                if ((xmlNode.Name == "XmlableTable" ? xmlNode : xmlNode.SelectSingleNode(".//XmlableTable")) != null)
                {
                    base.FromXML(xmlNode);
                    return;
                }

                foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//Mapping"))
                {
                    string innerXml = null;
                    string value = null;
                    if (xmlNodes.Attributes["Source"] == null || xmlNodes.Attributes["Target"] == null)
                    {
                        innerXml = xmlNodes.SelectSingleNode("./Source").InnerXml;
                        value = xmlNodes.SelectSingleNode("./Target").InnerXml;
                    }
                    else
                    {
                        innerXml = xmlNodes.Attributes["Source"].Value;
                        value = xmlNodes.Attributes["Target"].Value;
                    }

                    base.Add(innerXml, value);
                }
            }
        }
    }
}