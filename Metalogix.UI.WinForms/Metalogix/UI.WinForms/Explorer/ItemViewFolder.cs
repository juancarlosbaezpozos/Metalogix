using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Metabase;
using Metalogix.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.UI.WinForms.Explorer
{
	public class ItemViewFolder : Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
	{
		private Folder m_ViewFolder;

		public Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials
		{
			get
			{
				return this.ViewFolderNode.AzureAdGraphCredentials;
			}
			set
			{
				this.ViewFolderNode.AzureAdGraphCredentials = value;
			}
		}

		public NodeCollection Children
		{
			get
			{
				return this.ViewFolderNode.Children;
			}
		}

		public bool ChildrenFetched
		{
			get
			{
				return this.ViewFolderNode.ChildrenFetched;
			}
		}

		public Metalogix.Explorer.Connection Connection
		{
			get
			{
				return this.ViewFolderNode.Connection;
			}
		}

		public Metalogix.Permissions.Credentials Credentials
		{
			get
			{
				return this.ViewFolderNode.Credentials;
			}
			set
			{
				this.ViewFolderNode.Credentials = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.ViewFolderNode.DisplayName;
			}
		}

		public string DisplayUrl
		{
			get
			{
				return this.ViewFolderNode.DisplayUrl;
			}
		}

		public string ErrorDescription
		{
			get
			{
				return this.ViewFolderNode.ErrorDescription;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.ViewFolderNode.ErrorMsg;
			}
			set
			{
				this.ViewFolderNode.ErrorMsg = value;
			}
		}

		public System.Drawing.Image Image
		{
			get
			{
				return this.ViewFolderNode.Image;
			}
		}

		public string ImageName
		{
			get
			{
				return this.ViewFolderNode.ImageName;
			}
		}

		public bool InUseAsSource
		{
			get
			{
				return this.ViewFolderNode.InUseAsSource;
			}
		}

		public bool IsConnectionRoot
		{
			get
			{
				return this.ViewFolderNode.IsConnectionRoot;
			}
		}

		public string this[string sFieldName]
		{
			get
			{
				return this.ViewFolderNode[sFieldName];
			}
		}

		public string LinkableUrl
		{
			get
			{
				return this.ViewFolderNode.LinkableUrl;
			}
		}

		public bool LoadAutomatically
		{
			get
			{
				return this.ViewFolderNode.LoadAutomatically;
			}
		}

		public Metalogix.Explorer.Location Location
		{
			get
			{
				return this.ViewFolderNode.Location;
			}
		}

		public Metalogix.Metabase.MetabaseConnection MetabaseConnection
		{
			get
			{
				return this.ViewFolderNode.MetabaseConnection;
			}
			set
			{
				this.ViewFolderNode.MetabaseConnection = value;
			}
		}

		public string Name
		{
			get
			{
				return this.ViewFolderNode.Name;
			}
		}

		public Node Parent
		{
			get
			{
				return this.ViewFolderNode.Parent;
			}
		}

		public string Path
		{
			get
			{
				return this.ViewFolderNode.Path;
			}
		}

		public Metalogix.Metabase.Record Record
		{
			get
			{
				return this.ViewFolderNode.Record;
			}
		}

		public string ServerRelativeUrl
		{
			get
			{
				return this.ViewFolderNode.ServerRelativeUrl;
			}
		}

		public ConnectionStatus Status
		{
			get
			{
				return this.ViewFolderNode.Status;
			}
		}

		public string Url
		{
			get
			{
				return this.ViewFolderNode.Url;
			}
		}

		public Folder ViewFolder
		{
			get
			{
				return this.m_ViewFolder;
			}
		}

		public Node ViewFolderNode
		{
			get
			{
				return this.ViewFolder as Node;
			}
		}

		public string WorkspaceName
		{
			get
			{
				return this.ViewFolderNode.WorkspaceName;
			}
		}

		public string XML
		{
			get
			{
				return this.ViewFolderNode.XML;
			}
		}

		public ItemViewFolder(Folder viewFolder)
		{
			this.m_ViewFolder = viewFolder;
		}

		public void AddExternalConnection(ExternalConnection connection)
		{
			this.ViewFolderNode.AddExternalConnection(connection);
		}

		private void AttachEvents()
		{
			this.ViewFolderNode.ChildrenChanged += new NodeChildrenChangedHandler(this.ViewFolderNodeOnChildrenChanged);
			this.ViewFolderNode.StatusChanged += new NodeStatusChangedHandler(this.ViewFolderNodeOnStatusChanged);
			this.ViewFolderNode.DisplayNameChanged += new DisplayNameChangedHandler(this.ViewFolderNodeOnDisplayNameChanged);
			this.ViewFolderNode.SetBusyDisplay += new SetBusyDisplayHandler(this.ViewFolderNodeOnSetBusyDisplay);
		}

		public void CheckExternalConnections<T>()
		{
			this.ViewFolderNode.CheckExternalConnections<T>();
		}

		public bool CheckWriteAccess(out string sFailureMessage)
		{
			return this.ViewFolderNode.CheckWriteAccess(out sFailureMessage);
		}

		public DummyNode CloneDummy()
		{
			return this.ViewFolderNode.CloneDummy();
		}

		public Workspace CreateOrGetWorkspace(string workspaceName)
		{
			return this.ViewFolderNode.CreateOrGetWorkspace(workspaceName);
		}

		public void DeleteWorkspace(string workspaceName)
		{
			this.ViewFolderNode.DeleteWorkspace(workspaceName);
		}

		private void DetachEvents()
		{
			this.ViewFolderNode.ChildrenChanged -= new NodeChildrenChangedHandler(this.ViewFolderNodeOnChildrenChanged);
			this.ViewFolderNode.StatusChanged -= new NodeStatusChangedHandler(this.ViewFolderNodeOnStatusChanged);
			this.ViewFolderNode.DisplayNameChanged -= new DisplayNameChangedHandler(this.ViewFolderNodeOnDisplayNameChanged);
			this.ViewFolderNode.SetBusyDisplay -= new SetBusyDisplayHandler(this.ViewFolderNodeOnSetBusyDisplay);
		}

		public void FetchChildren()
		{
			this.ViewFolderNode.FetchChildren();
		}

		public AttributeCollection GetAttributes()
		{
			return this.ViewFolderNode.GetAttributes();
		}

		public string GetClassName()
		{
			return this.ViewFolderNode.GetClassName();
		}

		public string GetComponentName()
		{
			return this.ViewFolderNode.GetComponentName();
		}

		public TypeConverter GetConverter()
		{
			return this.ViewFolderNode.GetConverter();
		}

		public EventDescriptor GetDefaultEvent()
		{
			return this.ViewFolderNode.GetDefaultEvent();
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return this.ViewFolderNode.GetDefaultProperty();
		}

		public object GetEditor(Type editorBaseType)
		{
			return this.ViewFolderNode.GetEditor(editorBaseType);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return this.ViewFolderNode.GetEvents(attributes);
		}

		public EventDescriptorCollection GetEvents()
		{
			return this.ViewFolderNode.GetEvents();
		}

		public Dictionary<int, ExternalConnection> GetExternalConnectionsOfType<T>(bool recurseUp)
		{
			return this.ViewFolderNode.GetExternalConnectionsOfType<T>(recurseUp);
		}

		public Node GetNodeByPath(string sPath)
		{
			return this.ViewFolderNode.GetNodeByPath(sPath);
		}

		public Node GetNodeByUrl(string sURL)
		{
			return this.ViewFolderNode.GetNodeByUrl(sURL);
		}

		public XmlNode GetNodeXML()
		{
			return this.ViewFolderNode.GetNodeXML();
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.ViewFolderNode.GetProperties(attributes);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return this.ViewFolderNode.GetProperties();
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this.ViewFolderNode.GetPropertyOwner(pd);
		}

		public Metalogix.Metabase.Record GetRecord()
		{
			return this.ViewFolderNode.GetRecord();
		}

		public Metalogix.Metabase.Record GetRecord(Workspace workspace)
		{
			return this.ViewFolderNode.GetRecord(workspace);
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			return this.ViewFolderNode.IsEqual(targetComparable, differencesOutput, options);
		}

		public void ReleaseChildren()
		{
			this.ViewFolderNode.ReleaseChildren();
		}

		public void RemoveExternalConnection(ExternalConnection connection)
		{
			this.ViewFolderNode.RemoveExternalConnection(connection);
		}

		public void ResetActionSourceState()
		{
			this.ViewFolderNode.ResetActionSourceState();
		}

		public void SetAsActionSource()
		{
			this.ViewFolderNode.SetAsActionSource();
		}

		public override string ToString()
		{
			if (this.ViewFolderNode == null)
			{
				return this.m_ViewFolder.GetItems().ToString();
			}
			return this.ViewFolderNode.Location.ToString();
		}

		private void ViewFolderNodeOnChildrenChanged()
		{
			if (this.ChildrenChanged != null)
			{
				this.ChildrenChanged();
			}
		}

		private void ViewFolderNodeOnDisplayNameChanged()
		{
			if (this.DisplayNameChanged != null)
			{
				this.DisplayNameChanged();
			}
		}

		private void ViewFolderNodeOnSetBusyDisplay(bool isBusy)
		{
			if (this.SetBusyDisplay != null)
			{
				this.SetBusyDisplay(isBusy);
			}
		}

		private void ViewFolderNodeOnStatusChanged()
		{
			if (this.StatusChanged != null)
			{
				this.StatusChanged();
			}
		}

		public event NodeChildrenChangedHandler ChildrenChanged;

		public event DisplayNameChangedHandler DisplayNameChanged;

		public event SetBusyDisplayHandler SetBusyDisplay;

		public event NodeStatusChangedHandler StatusChanged;
	}
}