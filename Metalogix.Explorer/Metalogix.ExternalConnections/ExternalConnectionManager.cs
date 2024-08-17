using Metalogix;
using Metalogix.Core;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.ExternalConnections
{
    public class ExternalConnectionManager
    {
        private readonly static object _INSTANCE_LOCK;

        private static ExternalConnectionManager _INSTANCE;

        private static IExternalConnectionDatabase _DATABASE;

        private readonly Dictionary<int, ExternalConnection> m_connectionsCache;

        private readonly Dictionary<string, Hashtable> m_nodesToConnectionsCache;

        private readonly Dictionary<Type, int> m_existingExternalConnectionTypes;

        public static ExternalConnectionManager INSTANCE
        {
            get
            {
                if (ExternalConnectionManager._INSTANCE != null)
                {
                    return ExternalConnectionManager._INSTANCE;
                }

                lock (ExternalConnectionManager._INSTANCE_LOCK)
                {
                    if (ExternalConnectionManager._INSTANCE == null)
                    {
                        ExternalConnectionManager externalConnectionManager = new ExternalConnectionManager();
                        externalConnectionManager.LoadConnections();
                        ExternalConnectionManager._INSTANCE = externalConnectionManager;
                    }
                }

                return ExternalConnectionManager._INSTANCE;
            }
        }

        static ExternalConnectionManager()
        {
            IExternalConnectionDatabase webExternalConnectionDatabase;
            ExternalConnectionManager._INSTANCE_LOCK = new object();
            if (ApplicationData.IsWeb)
            {
                webExternalConnectionDatabase = new WebExternalConnectionDatabase();
            }
            else
            {
                IExternalConnectionDatabase externalConnectionDatabase =
                    new ExternalConnectionDatabase(Path.Combine(ApplicationData.ApplicationPath,
                        "ExternalConnections.sdf"));
                webExternalConnectionDatabase = externalConnectionDatabase;
            }

            ExternalConnectionManager._DATABASE = webExternalConnectionDatabase;
        }

        private ExternalConnectionManager()
        {
            this.m_connectionsCache = new Dictionary<int, ExternalConnection>();
            this.m_nodesToConnectionsCache = new Dictionary<string, Hashtable>();
            this.m_existingExternalConnectionTypes = new Dictionary<Type, int>();
        }

        public void AddConnection(ExternalConnection connection)
        {
            int num = ExternalConnectionManager._DATABASE.AddConnection(connection.GetType(), connection.ToXML());
            connection.ExternalConnectionID = num;
            this.m_connectionsCache.Add(num, connection);
            if (!this.m_existingExternalConnectionTypes.ContainsKey(connection.GetType()))
            {
                this.m_existingExternalConnectionTypes.Add(connection.GetType(), 1);
                return;
            }

            Dictionary<Type, int> mExistingExternalConnectionTypes = this.m_existingExternalConnectionTypes;
            Dictionary<Type, int> types = mExistingExternalConnectionTypes;
            Type type = connection.GetType();
            mExistingExternalConnectionTypes[type] = types[type] + 1;
        }

        public void AddConnectionToNode(ExternalConnection connection, Node node)
        {
            if (connection.ExternalConnectionID == -1)
            {
                throw new ArgumentException("Connection database id cannot be '-1'");
            }

            if (node.Location == null)
            {
                throw new ArgumentException("Node location cannot be null");
            }

            string str = this.PreProcessNodeId(node.Location.ToXML());
            ExternalConnectionManager._DATABASE.AddConnectionToNode(connection.ExternalConnectionID, str);
            if (!this.m_nodesToConnectionsCache.ContainsKey(str))
            {
                this.m_nodesToConnectionsCache.Add(str, new Hashtable());
            }

            this.m_nodesToConnectionsCache[str].Add(connection.ExternalConnectionID, connection.ExternalConnectionID);
        }

        public void CheckConnections<T>()
        {
            foreach (ExternalConnection value in this.m_connectionsCache.Values)
            {
                try
                {
                    if (value is T)
                    {
                        value.CheckConnection();
                    }
                }
                catch
                {
                }
            }

            Hashtable hashtables = new Hashtable();
            foreach (DataRow row in ExternalConnectionManager._DATABASE.GetNodes().Rows)
            {
                string str = this.PostProcessNodeId((string)row["nodeId"]);
                if (hashtables.ContainsKey(str))
                {
                    continue;
                }

                Location location = new Location(XmlUtility.StringToXmlNode(str));
                if (Settings.ActiveConnections.GetByConnString(location.ConnectionString) == null)
                {
                    continue;
                }

                Node node = location.GetNode();
                if (node == null)
                {
                    continue;
                }

                node.CheckExternalConnections<T>();
                hashtables.Add(str, node);
            }
        }

        private ExternalConnection CreateConnectionFrom(DataRow row)
        {
            ExternalConnection externalConnection;
            Type type = null;
            try
            {
                string item = (string)row["connectionType"];
                Assembly assembly = Assembly.Load((string)row["connectionAssembly"], null);
                type = assembly.GetType(item, false);
                if (type != null)
                {
                    int num = (int)row["connectionId"];
                    string str = (string)row["connectionXml"];
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(str);
                    ExternalConnection externalConnection1 = null;
                    try
                    {
                        try
                        {
                            externalConnection1 = (ExternalConnection)Activator.CreateInstance(type);
                            externalConnection1.FromXml(xmlDocument.DocumentElement);
                        }
                        catch
                        {
                            object[] documentElement = new object[] { xmlDocument.DocumentElement };
                            externalConnection1 = (ExternalConnection)Activator.CreateInstance(type, documentElement);
                        }
                    }
                    finally
                    {
                        externalConnection1.ExternalConnectionID = num;
                    }

                    return externalConnection1;
                }
                else
                {
                    externalConnection = null;
                }
            }
            catch (Exception exception)
            {
                externalConnection = null;
            }

            return externalConnection;
        }

        public ExternalConnection GetConnection(int connId)
        {
            if (this.m_connectionsCache.ContainsKey(connId))
            {
                return this.m_connectionsCache[connId];
            }

            ExternalConnection externalConnection = null;
            DataTable connection = ExternalConnectionManager._DATABASE.GetConnection(connId);
            externalConnection = this.CreateConnectionFrom(connection.Rows[0]);
            if (externalConnection != null)
            {
                if (this.m_connectionsCache.ContainsKey(externalConnection.ExternalConnectionID))
                {
                    this.m_connectionsCache.Remove(externalConnection.ExternalConnectionID);
                }

                this.m_connectionsCache.Add(externalConnection.ExternalConnectionID, externalConnection);
            }

            return externalConnection;
        }

        public Dictionary<int, ExternalConnection> GetConnections(Type connType)
        {
            if (connType == null)
            {
                return this.m_connectionsCache;
            }

            Dictionary<int, ExternalConnection> nums = new Dictionary<int, ExternalConnection>();
            if (this.m_existingExternalConnectionTypes.ContainsKey(connType))
            {
                foreach (KeyValuePair<int, ExternalConnection> mConnectionsCache in this.m_connectionsCache)
                {
                    if (mConnectionsCache.Value.GetType() != connType &&
                        !mConnectionsCache.Value.GetType().IsSubclassOf(connType))
                    {
                        continue;
                    }

                    nums.Add(mConnectionsCache.Key, mConnectionsCache.Value);
                }
            }

            return nums;
        }

        public Dictionary<int, ExternalConnection> GetConnectionsOfType<T>()
        {
            return this.GetConnections(typeof(T));
        }

        public Dictionary<int, ExternalConnection> GetConnectionsToNode(Node node, Type connectionType)
        {
            Dictionary<int, ExternalConnection> nums = new Dictionary<int, ExternalConnection>();
            if (!this.m_existingExternalConnectionTypes.ContainsKey(connectionType))
            {
                return nums;
            }

            if (node.Location == null)
            {
                throw new ArgumentException("Node location cannot be null");
            }

            string str = this.PreProcessNodeId(node.Location.ToXML());
            ICollection keys = null;
            if (this.m_nodesToConnectionsCache.ContainsKey(str))
            {
                keys = this.m_nodesToConnectionsCache[str].Keys;
            }

            if (keys == null)
            {
                DataTable connectionsToNode = ExternalConnectionManager._DATABASE.GetConnectionsToNode(str);
                Hashtable hashtables = new Hashtable();
                foreach (DataRow row in connectionsToNode.Rows)
                {
                    int item = (int)row["connectionId"];
                    if (!this.m_connectionsCache.ContainsKey(item))
                    {
                        continue;
                    }

                    hashtables.Add(item, item);
                }

                this.m_nodesToConnectionsCache.Add(str, hashtables);
                keys = hashtables.Keys;
            }

            foreach (int key in keys)
            {
                if (!this.m_connectionsCache.ContainsKey(key))
                {
                    continue;
                }

                ExternalConnection externalConnection = this.m_connectionsCache[key];
                if (connectionType != null && externalConnection.GetType() != connectionType &&
                    !externalConnection.GetType().IsSubclassOf(connectionType))
                {
                    continue;
                }

                nums.Add(key, externalConnection);
            }

            return nums;
        }

        public Dictionary<int, ExternalConnection> GetConnectionsToNodeOfType<T>(Node node)
        {
            return this.GetConnectionsToNode(node, typeof(T));
        }

        public Dictionary<string, Node> GetNodesToConnection(ExternalConnection connection)
        {
            Dictionary<string, Node> strs = new Dictionary<string, Node>();
            foreach (DataRow row in ExternalConnectionManager._DATABASE
                         .GetNodesToConnection(connection.ExternalConnectionID).Rows)
            {
                string item = (string)row["nodeId"];
                Node node = ExplorerNode.CreateNodeFromLocationString(item);
                if (node == null)
                {
                    continue;
                }

                strs.Add(item, node);
            }

            return strs;
        }

        public bool HasConnectionsOfType<T>()
        {
            return this.m_existingExternalConnectionTypes.ContainsKey(typeof(T));
        }

        private void LoadConnections()
        {
            foreach (DataRow row in ExternalConnectionManager._DATABASE.GetConnections().Rows)
            {
                ExternalConnection externalConnection = this.CreateConnectionFrom(row);
                if (externalConnection == null)
                {
                    continue;
                }

                this.m_connectionsCache.Add(externalConnection.ExternalConnectionID, externalConnection);
                if (this.m_existingExternalConnectionTypes.ContainsKey(externalConnection.GetType()))
                {
                    Dictionary<Type, int> mExistingExternalConnectionTypes = this.m_existingExternalConnectionTypes;
                    Dictionary<Type, int> types = mExistingExternalConnectionTypes;
                    Type type = externalConnection.GetType();
                    mExistingExternalConnectionTypes[type] = types[type] + 1;
                }
                else
                {
                    this.m_existingExternalConnectionTypes.Add(externalConnection.GetType(), 1);
                }
            }
        }

        private string PostProcessNodeId(string nodeId)
        {
            return Regex.Replace(nodeId, "Version=[\\d\\.]+",
                string.Concat("Version=", ConfigurationVariables.AssemblyVersionString), RegexOptions.Singleline);
        }

        private string PreProcessNodeId(string nodeId)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(nodeId);
                foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("/Location/Connection"))
                {
                    XmlElement xmlElement = xmlNodes as XmlElement;
                    if (xmlElement.HasAttribute("Password"))
                    {
                        xmlElement.RemoveAttribute("Password");
                    }

                    if (xmlElement.HasAttribute("SharePointVersion"))
                    {
                        xmlElement.RemoveAttribute("SharePointVersion");
                    }

                    if (!xmlElement.HasAttribute("UnderlyingAdapterType"))
                    {
                        continue;
                    }

                    xmlElement.RemoveAttribute("UnderlyingAdapterType");
                }

                nodeId = xmlDocument.InnerXml;
            }
            catch (Exception exception)
            {
            }

            return Regex.Replace(nodeId, "Version=[\\d\\.]+", "Version=0.0.0.0", RegexOptions.Singleline);
        }

        public void RefreshConnection(ExternalConnection connection)
        {
            if (!this.m_connectionsCache.ContainsKey(connection.ExternalConnectionID))
            {
                return;
            }

            this.m_connectionsCache.Remove(connection.ExternalConnectionID);
        }

        public void RefreshConnectionsToNode(Node node)
        {
            if (node.Location == null)
            {
                throw new ArgumentException("Node location cannot be null");
            }

            string str = this.PreProcessNodeId(node.Location.ToXML());
            if (!this.m_nodesToConnectionsCache.ContainsKey(str))
            {
                return;
            }

            this.m_nodesToConnectionsCache.Remove(str);
        }

        public void RemoveConnection(ExternalConnection connection)
        {
            if (connection.ExternalConnectionID == -1)
            {
                throw new ArgumentException("Connection database id cannot be '-1'");
            }

            ExternalConnectionManager._DATABASE.RemoveConnection(connection.ExternalConnectionID);
            this.m_connectionsCache.Remove(connection.ExternalConnectionID);
            if (this.m_existingExternalConnectionTypes.ContainsKey(connection.GetType()))
            {
                int item = this.m_existingExternalConnectionTypes[connection.GetType()] - 1;
                if (item <= 0)
                {
                    this.m_existingExternalConnectionTypes.Remove(connection.GetType());
                    return;
                }

                this.m_existingExternalConnectionTypes[connection.GetType()] = item;
            }
        }

        public void RemoveConnectionFromNode(ExternalConnection connection, Node node)
        {
            if (connection.ExternalConnectionID == -1)
            {
                throw new ArgumentException("Connection database id cannot be '-1'");
            }

            if (node.Location == null)
            {
                throw new ArgumentException("Node location cannot be null");
            }

            string str = this.PreProcessNodeId(node.Location.ToXML());
            ExternalConnectionManager._DATABASE.RemoveConnectionFromNode(connection.ExternalConnectionID, str);
            this.m_nodesToConnectionsCache[str].Remove(connection.ExternalConnectionID);
        }

        public void UpdateConnection(ExternalConnection connection)
        {
            ExternalConnectionManager._DATABASE.UpdateConnection(connection.ExternalConnectionID, connection.ToXML());
        }
    }
}