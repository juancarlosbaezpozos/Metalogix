using Metalogix;
using Metalogix.Actions;
using Metalogix.Core;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.Explorer.Properties;
using Metalogix.ExternalConnections;
using Metalogix.Metabase;
using Metalogix.Permissions;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Metalogix.Explorer
{
    public abstract class ExplorerNode : Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor, IDisposable
    {
        private ConnectionStatus m_status;

        private Node m_parent;

        private Metalogix.Permissions.Credentials m_credentials;

        private string m_Error;

        private NodeCollection m_childNodes;

        private object _fetchChildrenSyncLock = new object();

        private Exception _connectionError;

        protected string m_sImageName;

        private readonly object m_lockMetabaseConnection = new object();

        private Metalogix.Metabase.MetabaseConnection m_metabaseConnection;

        private readonly object m_lockRecord = new object();

        private volatile Metalogix.Metabase.Record m_record;

        protected bool? m_bInUseAsSource = null;

        public virtual Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials { get; set; }

        public NodeCollection Children
        {
            get
            {
                if (this.m_childNodes == null)
                {
                    this.FetchChildren();
                }

                return this.m_childNodes;
            }
        }

        [IsSystem(true)]
        public bool ChildrenFetched
        {
            get { return this.m_childNodes != null; }
        }

        public Metalogix.Explorer.Connection Connection
        {
            get
            {
                Node parent = this;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                if (!(parent is Metalogix.Explorer.Connection))
                {
                    return null;
                }

                return (Metalogix.Explorer.Connection)parent;
            }
        }

        [IsSystem(true)]
        public Exception ConnectionError
        {
            get { return this._connectionError; }
        }

        public virtual Metalogix.Permissions.Credentials Credentials
        {
            get
            {
                if (this.m_credentials == null)
                {
                    this.m_credentials = new Metalogix.Permissions.Credentials();
                }

                return this.m_credentials;
            }
            set { this.m_credentials = value; }
        }

        public virtual string DisplayName
        {
            get { return this.Name; }
        }

        public virtual string DisplayUrl
        {
            get { return this.Url; }
        }

        [IsSystem(true)]
        public string ErrorDescription
        {
            get
            {
                if (this._connectionError == null)
                {
                    return string.Empty;
                }

                return this._connectionError.Message;
            }
        }

        public string ErrorMsg
        {
            get { return this.m_Error; }
            set { this.m_Error = value; }
        }

        public bool HasEventClients
        {
            get
            {
                if (this.ChildrenChanged != null)
                {
                    return true;
                }

                if (this.StatusChanged != null)
                {
                    return true;
                }

                return this.DisplayNameChanged != null;
            }
        }

        public virtual System.Drawing.Image Image
        {
            get { return ImageCache.GetImage(this.ImageName, this.GetType().Assembly); }
        }

        [IsSystem(true)]
        public virtual string ImageName
        {
            get
            {
                if (this.m_sImageName == null)
                {
                    object[] customAttributes = this.GetType().GetCustomAttributes(typeof(ImageAttribute), true);
                    if ((int)customAttributes.Length == 1)
                    {
                        this.m_sImageName = ((ImageAttribute)customAttributes[0]).ImageName;
                    }
                }

                return this.m_sImageName;
            }
        }

        public virtual bool InUseAsSource
        {
            get
            {
                if (!this.m_bInUseAsSource.HasValue)
                {
                    if (this.Parent != null)
                    {
                        return this.Parent.InUseAsSource;
                    }

                    this.m_bInUseAsSource = new bool?(false);
                }

                return this.m_bInUseAsSource.Value;
            }
        }

        public virtual bool IsConnectionRoot
        {
            get { return this.m_parent == null; }
        }

        public virtual string this[string sFieldName]
        {
            get
            {
                string str;
                PropertyInfo property = this.GetType().GetProperty(sFieldName);
                if (property == null)
                {
                    return null;
                }

                try
                {
                    str = property.GetValue(this, null).ToString();
                }
                catch
                {
                    str = null;
                }

                return str;
            }
            set { }
        }

        public DateTime LastRefreshTimestamp { get; private set; }

        public virtual string LinkableUrl
        {
            get { return this.Url; }
        }

        public virtual bool LoadAutomatically
        {
            get { return true; }
        }

        public Metalogix.Explorer.Location Location
        {
            get
            {
                return new Metalogix.Explorer.Location(this.Path, this.DisplayUrl, this.Connection.ConnectionString);
            }
        }

        [Browsable(false)]
        public Metalogix.Metabase.MetabaseConnection MetabaseConnection
        {
            get
            {
                Metalogix.Metabase.MetabaseConnection mMetabaseConnection;
                lock (this.m_lockMetabaseConnection)
                {
                    if (this.m_metabaseConnection == null)
                    {
                        if (this.Parent == null)
                        {
                            return null;
                        }

                        return this.Parent.MetabaseConnection;
                    }
                    else
                    {
                        if (!this.m_metabaseConnection.IsConnected)
                        {
                            this.m_metabaseConnection.Connect();
                        }

                        mMetabaseConnection = this.m_metabaseConnection;
                    }
                }

                return mMetabaseConnection;
            }
            set
            {
                lock (this.m_lockMetabaseConnection)
                {
                    if (this.m_metabaseConnection != null)
                    {
                        this.m_metabaseConnection.Disconnect();
                        this.m_metabaseConnection = null;
                    }

                    this.m_metabaseConnection = value;
                    this.SetStatus(ConnectionStatus.NotChecked);
                    this.SetChildren(null);
                }
            }
        }

        public abstract string Name { get; }

        public Node Parent
        {
            get { return this.m_parent; }
        }

        public virtual string Path
        {
            get
            {
                if (this.Parent == null)
                {
                    return "";
                }

                return string.Concat(this.Parent.Path, "/", this.Name);
            }
        }

        [Browsable(false)]
        public Metalogix.Metabase.Record Record
        {
            get
            {
                Metalogix.Metabase.Record mRecord;
                lock (this.m_lockRecord)
                {
                    if (this.m_record != null)
                    {
                        if (!this.m_record.ParentWorkspace.Exists() ||
                            !this.m_record.ParentWorkspace.Connection.IsConnected ||
                            !string.Equals(this.WorkspaceName, this.m_record.ParentWorkspace.Name))
                        {
                            this.m_record = null;
                        }
                        else
                        {
                            mRecord = this.m_record;
                            return mRecord;
                        }
                    }

                    if (this.MetabaseConnection != null && this.m_record == null)
                    {
                        Workspace workspace = this.CreateOrGetWorkspace(this.WorkspaceName);
                        PropertyDescriptor item =
                            TypeDescriptor.GetProperties(typeof(Metalogix.Metabase.Record))["SourceURL"];
                        this.m_record = (Metalogix.Metabase.Record)workspace.Records.FindItem(item, this.Url) ??
                                        workspace.Records.AddNew(Guid.NewGuid(), this.Url);
                    }

                    return this.m_record;
                }

                return mRecord;
            }
        }

        public abstract string ServerRelativeUrl { get; }

        public ConnectionStatus Status
        {
            get { return this.m_status; }
        }

        public virtual string Url
        {
            get { return ""; }
        }

        [Browsable(false)]
        public virtual string WorkspaceName
        {
            get
            {
                int hashCode = this.Connection.Node.Url.GetHashCode();
                return hashCode.ToString(CultureInfo.InvariantCulture);
            }
        }

        [IsSystem(true)] public abstract string XML { get; }

        public ExplorerNode(Node parent)
        {
            this.m_parent = parent;
        }

        public void AddExternalConnection(ExternalConnection connection)
        {
            ExternalConnectionManager.INSTANCE.AddConnectionToNode(connection, this);
        }

        public void CheckExternalConnections<T>()
        {
            if (ExternalConnectionManager.INSTANCE.HasConnectionsOfType<T>())
            {
                foreach (ExternalConnection value in this.GetExternalConnectionsOfType<T>(true).Values)
                {
                    if (this.Status == ConnectionStatus.Warning)
                    {
                        return;
                    }
                    else if (this.Status == ConnectionStatus.Valid)
                    {
                        switch (value.Status)
                        {
                            case ConnectionStatus.Invalid:
                            case ConnectionStatus.Warning:
                            {
                                this.SetStatus(ConnectionStatus.Warning);
                                return;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (this.Status == ConnectionStatus.Warning)
            {
                this.SetStatus(ConnectionStatus.Valid);
            }
        }

        public virtual bool CheckWriteAccess(out string sMessage)
        {
            if (this.InUseAsSource)
            {
                sMessage = Resources.SourceInUse;
                return false;
            }

            sMessage = null;
            return true;
        }

        protected abstract void ClearChildNodes();

        protected virtual void ClearExcessNodeData()
        {
        }

        public virtual DummyNode CloneDummy()
        {
            DummyNode dummyNode = new DummyNode(this.m_parent);
            dummyNode.SetUrl(this.Url);
            dummyNode.SetImageName(this.ImageName);
            dummyNode.SetName(this.Name);
            dummyNode.SetServerRelativeUrl(this.ServerRelativeUrl);
            dummyNode.SetDisplayName(this.DisplayName);
            return dummyNode;
        }

        public virtual void Close()
        {
        }

        public void Collapse()
        {
            this.ClearExcessNodeData();
        }

        public static Node CreateNodeFromLocationString(string sLoc)
        {
            Node node = (new Metalogix.Explorer.Location(XmlUtility.StringToXmlNode(sLoc))).GetNode();
            if (node == null)
            {
                throw new Exception(string.Concat("Could not find node at the following location: ", sLoc));
            }

            return node;
        }

        public Workspace CreateOrGetWorkspace(string workspaceName)
        {
            if (string.IsNullOrEmpty(workspaceName))
            {
                throw new ArgumentNullException("workspaceName");
            }

            if (workspaceName.Length > 50)
            {
                throw new ArgumentException("Workspace name cannot exceed 50 characters.");
            }

            if (this.MetabaseConnection == null)
            {
                throw new NullReferenceException("Metabase connection cannot be null.");
            }

            return this.MetabaseConnection.GetWorkspace(workspaceName) ??
                   this.MetabaseConnection.CreateWorkspace(workspaceName);
        }

        public void DeleteWorkspace(string workspaceName)
        {
            if (this.MetabaseConnection == null)
            {
                throw new NullReferenceException("Metabase connection cannot be null.");
            }

            Workspace workspace = this.MetabaseConnection.GetWorkspace(workspaceName);
            if (workspace == null)
            {
                return;
            }

            workspace.Delete();
        }

        public void Dispose()
        {
            this.Dispose(false);
        }

        public virtual void Dispose(bool bForceGarbageCollection)
        {
            this.ClearExcessNodeData();
            if (this.ChildrenChanged == null)
            {
                this.ReleaseChildren();
            }

            lock (this.m_lockRecord)
            {
                if (this.m_record != null)
                {
                    this.m_record.Dispose();
                    this.m_record = null;
                }
            }

            lock (this.m_lockMetabaseConnection)
            {
                if (this.m_metabaseConnection != null)
                {
                    this.m_metabaseConnection.Disconnect();
                }
            }

            if (bForceGarbageCollection)
            {
                GC.Collect();
            }
        }

        protected abstract Node[] FetchChildNodes();

        public virtual void FetchChildren()
        {
            ConnectionStatus status;
            try
            {
                lock (this._fetchChildrenSyncLock)
                {
                    status = this.Status;
                    if (status != ConnectionStatus.Checking)
                    {
                        this.m_status = ConnectionStatus.Checking;
                        this._connectionError = null;
                    }
                }

                if (status != ConnectionStatus.Checking)
                {
                    bool flag = (!(this is Metalogix.Explorer.Connection) || this.Parent != null
                        ? false
                        : status == ConnectionStatus.NotChecked);
                    if (this.SetBusyDisplay != null && this is Metalogix.Explorer.Connection &&
                        status == ConnectionStatus.NotChecked)
                    {
                        this.SetBusyDisplay(true);
                    }

                    if (flag)
                    {
                        ((Metalogix.Explorer.Connection)this).CheckConnection();
                    }

                    Node[] nodeArray = this.FetchChildNodes();
                    if (this.SetBusyDisplay != null && this is Metalogix.Explorer.Connection)
                    {
                        this.SetBusyDisplay(false);
                    }

                    this.SetStatus(ConnectionStatus.Valid);
                    this.SetChildren(new NodeCollection(nodeArray));
                }
                else
                {
                    while (this.Status == ConnectionStatus.Checking)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                    string.Format("An error occurred while fecthing child nodes for Node '{0}'", this.Url), true);
                this._connectionError = exception;
                this.SetStatus(ConnectionStatus.Invalid);
                this.SetChildren(null);
            }
        }

        protected void FireChildrenChanged()
        {
            if (this.ChildrenChanged != null)
            {
                this.ChildrenChanged();
            }
        }

        protected void FireDisplayNameChanged()
        {
            if (this.DisplayNameChanged != null)
            {
                this.DisplayNameChanged();
            }
        }

        protected void FireStatusChanged()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged();
            }
        }

        public virtual AttributeCollection GetAttributes()
        {
            object[] customAttributes = this.GetType().GetCustomAttributes(true);
            Attribute[] attributeArray = new Attribute[(int)customAttributes.Length];
            customAttributes.CopyTo(attributeArray, 0);
            return new AttributeCollection(attributeArray);
        }

        public virtual string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.GetType());
        }

        public virtual string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.GetType());
        }

        public virtual TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.GetType());
        }

        public virtual PropertyDescriptorCollection GetCustomProperties(Attribute[] attributes)
        {
            RecordPropertyDescriptorList recordPropertyDescriptorList = new RecordPropertyDescriptorList(null);
            if (this.Record != null)
            {
                foreach (PropertyDescriptor property in this.Record.GetProperties(attributes))
                {
                    recordPropertyDescriptorList.Add(new NodePropertyDescriptor(property, MetabasePDTarget.Record));
                }
            }

            return recordPropertyDescriptorList;
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.GetType());
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.GetType());
        }

        public virtual object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.GetType(), editorBaseType);
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.GetType(), attributes);
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.GetType());
        }

        public Dictionary<int, ExternalConnection> GetExternalConnectionsOfType<T>(bool recurseUp)
        {
            if (!ExternalConnectionManager.INSTANCE.HasConnectionsOfType<T>())
            {
                return new Dictionary<int, ExternalConnection>();
            }

            Dictionary<int, ExternalConnection> connectionsToNodeOfType =
                ExternalConnectionManager.INSTANCE.GetConnectionsToNodeOfType<T>(this);
            if (connectionsToNodeOfType.Count != 0)
            {
                return connectionsToNodeOfType;
            }

            if (!recurseUp || this.Parent == null)
            {
                return connectionsToNodeOfType;
            }

            return this.Parent.GetExternalConnectionsOfType<T>(recurseUp);
        }

        public virtual Node GetNodeByPath(string sPath)
        {
            char[] chrArray = new char[] { '/' };
            string str = sPath.Trim(chrArray);
            string str1 = str.Trim();
            string str2 = "";
            if (this.Status == ConnectionStatus.Invalid)
            {
                return null;
            }

            int num = str.IndexOfAny(chrArray);
            if (num >= 0)
            {
                str1 = str.Substring(0, num);
                str2 = str.Substring(num);
            }

            Node item = null;
            if (this.Children == null)
            {
                throw new Exception("Could not retrieve the children of a node. The 'Children' property is null.");
            }

            item = this.Children[str1];
            if (item == null)
            {
                return null;
            }

            if (str2 == "")
            {
                return item;
            }

            return item.GetNodeByPath(str2);
        }

        public virtual Node GetNodeByUrl(string sURL)
        {
            if (this.Url == sURL)
            {
                return this;
            }

            return this.Children.GetNodeByUrl(sURL);
        }

        public virtual XmlNode GetNodeXML()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(this.XML);
            return xmlDocument.DocumentElement;
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection customProperties =
                this.GetCustomProperties(attributes) ?? new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this.GetType(), attributes))
            {
                PropertyInfo propertyInfo = this.GetType().GetProperty(property.Name);
                if (attributes != null)
                {
                    bool flag = false;
                    Attribute[] attributeArray = attributes;
                    int num = 0;
                    while (num < (int)attributeArray.Length)
                    {
                        if (propertyInfo.IsDefined(attributeArray[num].GetType(), true))
                        {
                            num++;
                        }
                        else
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        continue;
                    }
                }

                customProperties.Add(new NodePropertyDescriptor(property, MetabasePDTarget.Base));
            }

            return customProperties;
        }

        public virtual PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                return this;
            }

            if (this.GetProperties().Contains(pd))
            {
                return this;
            }

            return null;
        }

        public Metalogix.Metabase.Record GetRecord()
        {
            return this.GetRecord(this.CreateOrGetWorkspace(this.WorkspaceName));
        }

        public Metalogix.Metabase.Record GetRecord(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException("workspace");
            }

            return workspace.FetchSingleRecord(this.Url);
        }

        public abstract bool IsEqual(Metalogix.DataStructures.IComparable targetComparable,
            DifferenceLog differencesOutput, ComparisonOptions options);

        public void Refresh()
        {
            this.Dispose();
            this.UpdateCurrentNode();
            this.ReleaseChildren();
            if (this is Metalogix.Explorer.Connection && this.Parent == null &&
                this.Status != ConnectionStatus.NotChecked)
            {
                this.m_status = ConnectionStatus.NotChecked;
            }

            this.FetchChildren();
            if (this.Status == ConnectionStatus.Valid)
            {
                Thread thread = new Thread(new ThreadStart(this.RefreshChildren));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            this.LastRefreshTimestamp = DateTime.Now;
        }

        private void RefreshChildren()
        {
            try
            {
                foreach (Node child in this.Children)
                {
                    if (!child.LoadAutomatically)
                    {
                        continue;
                    }

                    child.FetchChildren();
                }
            }
            catch (Exception exception)
            {
            }
        }

        public void ReleaseChildren()
        {
            this.ClearChildNodes();
            this.m_childNodes = null;
        }

        public void RemoveExternalConnection(ExternalConnection connection)
        {
            ExternalConnectionManager.INSTANCE.RemoveConnectionFromNode(connection, this);
        }

        public virtual void ResetActionSourceState()
        {
            this.m_bInUseAsSource = null;
        }

        public virtual void SetAsActionSource()
        {
            this.m_bInUseAsSource = new bool?(true);
        }

        protected void SetChildren(NodeCollection children)
        {
            this.m_childNodes = children;
            this.FireChildrenChanged();
        }

        protected void SetStatus(ConnectionStatus status)
        {
            this.m_status = status;
            if (this.StatusChanged != null)
            {
                this.StatusChanged();
            }
        }

        public virtual void UpdateCurrentNode()
        {
            this.FireDisplayNameChanged();
        }

        public event NodeChildrenChangedHandler ChildrenChanged;

        public event DisplayNameChangedHandler DisplayNameChanged;

        public event SetBusyDisplayHandler SetBusyDisplay;

        public event NodeStatusChangedHandler StatusChanged;
    }
}