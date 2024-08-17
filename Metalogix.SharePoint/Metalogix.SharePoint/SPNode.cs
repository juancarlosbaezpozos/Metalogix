using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.Interfaces;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Any SharePoint Object")]
	[PluralName("All SharePoint Objects")]
	public abstract class SPNode : ExplorerNode
	{
		protected SharePointAdapter m_adapter;

		private readonly object m_lockVirtualConnection = new object();

		private Metalogix.Metabase.MetabaseConnection m_virtualConnection;

		private string m_sWorkspaceGuid = null;

		private object m_actionSourceLock = new object();

		private Metalogix.Metabase.Record m_record = null;

		protected object m_oLockRecord = new object();

		public SharePointAdapter Adapter
		{
			get
			{
				return this.m_adapter;
			}
		}

		public override Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials
		{
			get
			{
				Metalogix.Permissions.AzureAdGraphCredentials azureAdGraphCredentials;
				if (this.Adapter == null)
				{
					azureAdGraphCredentials = null;
				}
				else
				{
					azureAdGraphCredentials = this.Adapter.AzureAdGraphCredentials;
				}
				return azureAdGraphCredentials;
			}
			set
			{
				if (this.Adapter != null)
				{
					this.Adapter.AzureAdGraphCredentials = value;
				}
			}
		}

		[IsSystem(true)]
		public bool CanWriteCreatedModifiedMetaInfo
		{
			get
			{
				return (this.Adapter == null ? false : this.Adapter.SupportsWritingAuthorshipData);
			}
		}

		public override Metalogix.Permissions.Credentials Credentials
		{
			get
			{
				Metalogix.Permissions.Credentials credential;
				credential = (this.Adapter != null ? this.Adapter.Credentials : new Metalogix.Permissions.Credentials());
				return credential;
			}
			set
			{
				if (this.Adapter != null)
				{
					this.Adapter.Credentials = value;
				}
			}
		}

		public override string DisplayUrl
		{
			get
			{
				string str;
				if (this.Adapter == null)
				{
					str = null;
				}
				else if ((this.Adapter.ServerDisplayName == null ? false : !(this.Adapter.ServerDisplayName.Trim() == "")))
				{
					string serverDisplayName = this.Adapter.ServerDisplayName;
					if (!string.IsNullOrEmpty(this.ServerRelativeUrl))
					{
						str = (!this.ServerRelativeUrl.StartsWith("/") ? string.Concat(serverDisplayName, "/", this.ServerRelativeUrl) : string.Concat(serverDisplayName, this.ServerRelativeUrl));
					}
					else
					{
						str = serverDisplayName;
					}
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public override string this[string sFieldName]
		{
			get
			{
				string str;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
				try
				{
					PropertyDescriptor item = properties[sFieldName];
					if (item != null)
					{
						str = item.GetValue(this).ToString();
						return str;
					}
				}
				catch
				{
				}
				str = null;
				return str;
			}
			set
			{
			}
		}

		public override string LinkableUrl
		{
			get
			{
				string str;
				if ((this.Adapter == null || this.Adapter.ServerLinkName == null ? false : !(this.Adapter.ServerLinkName.Trim() == "")))
				{
					string serverLinkName = this.Adapter.ServerLinkName;
					if (!string.IsNullOrEmpty(this.ServerRelativeUrl))
					{
						str = (!this.ServerRelativeUrl.StartsWith("/") ? string.Concat(serverLinkName, "/", this.ServerRelativeUrl) : string.Concat(serverLinkName, this.ServerRelativeUrl));
					}
					else
					{
						str = serverLinkName;
					}
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public Metalogix.Metabase.MetabaseConnection VirtualConnection
		{
			get
			{
				Metalogix.Metabase.MetabaseConnection virtualConnection;
				if (this.m_virtualConnection == null)
				{
					SPNode parent = base.Parent as SPNode;
					if (parent != null)
					{
						virtualConnection = parent.VirtualConnection;
					}
					else if (this is Metalogix.Explorer.Connection)
					{
						virtualConnection = this.m_virtualConnection;
					}
					else
					{
						virtualConnection = null;
					}
				}
				else
				{
					virtualConnection = this.m_virtualConnection;
				}
				return virtualConnection;
			}
			set
			{
				lock (this.m_lockVirtualConnection)
				{
					if (this.m_virtualConnection != null)
					{
						if (this.m_virtualConnection != null)
						{
							this.m_virtualConnection.Dispose();
							this.m_virtualConnection = null;
						}
					}
					this.m_virtualConnection = value;
				}
			}
		}

		internal string WorkspaceGuid
		{
			get
			{
				string workspaceGuid;
				bool flag;
				if (this.m_sWorkspaceGuid == null)
				{
					if (base.Parent == null)
					{
						flag = true;
					}
					else
					{
						flag = (!(this is SPSite) ? false : !(base.Parent is SPBaseServer));
					}
					if (!flag)
					{
						SPNode parent = base.Parent as SPNode;
						if (parent != null)
						{
							workspaceGuid = parent.WorkspaceGuid;
							return workspaceGuid;
						}
					}
					workspaceGuid = null;
				}
				else
				{
					workspaceGuid = this.m_sWorkspaceGuid;
				}
				return workspaceGuid;
			}
		}

		public virtual bool WriteVirtually
		{
			get
			{
				return this.InUseAsSource;
			}
		}

		public SPNode(SharePointAdapter adapter, SPNode parent) : base(parent)
		{
			this.m_adapter = adapter;
		}

		public virtual bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
		{
			lByteschanged = (long)0;
			lItemsChanged = (long)0;
			return false;
		}

		internal XmlNode AttachVirtualData(XmlNode sourceNode, string sPropertyName)
		{
			XmlNode xmlNodes = sourceNode;
			try
			{
				if (this.WriteVirtually)
				{
					lock (this.m_oLockRecord)
					{
						Metalogix.Metabase.Record virtualRecord = this.GetVirtualRecord();
						if (virtualRecord != null)
						{
							xmlNodes = MetabaseUtility.ProcessEditScript(sourceNode, virtualRecord, sPropertyName);
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
			return xmlNodes;
		}

		public override bool CheckWriteAccess(out string sMessage)
		{
			bool flag;
			if (this.Adapter.Writer != null)
			{
				flag = base.CheckWriteAccess(out sMessage);
			}
			else
			{
				sMessage = Resources.TargetIsReadOnly;
				flag = false;
			}
			return flag;
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			lock (this.m_oLockRecord)
			{
				if (this.m_record != null)
				{
					this.m_record.CommitChanges();
					this.m_record = null;
				}
			}
		}

		public virtual SPOptimizationNode GetAlertsOptimizationTree()
		{
			SPOptimizationNode sPOptimizationNode;
			try
			{
				string str = this.Adapter.Reader.FindAlerts();
				if (!string.IsNullOrEmpty(str))
				{
					sPOptimizationNode = SPOptimizationNode.InstantiateFromXML(XmlUtility.StringToXmlNode(str));
				}
				else
				{
					sPOptimizationNode = null;
				}
			}
			catch
			{
				sPOptimizationNode = null;
			}
			return sPOptimizationNode;
		}

		public override PropertyDescriptorCollection GetCustomProperties(Attribute[] attributes)
		{
			XmlNode nodeXML = this.GetNodeXML();
			PropertyDescriptor[] xmlPropertyDescriptor = new PropertyDescriptor[nodeXML.Attributes.Count];
			int num = 0;
			foreach (XmlAttribute attribute in nodeXML.Attributes)
			{
				string name = attribute.Name;
				object[] objArray = new object[] { nodeXML.Name, attribute.Name };
				string str = string.Format("./@{1}", objArray);
				Attribute[] categoryAttribute = new Attribute[] { new CategoryAttribute("SharePoint Properties") };
				xmlPropertyDescriptor[num] = new XmlPropertyDescriptor(name, str, categoryAttribute);
				num++;
			}
			return new PropertyDescriptorCollection(xmlPropertyDescriptor);
		}

		public virtual SPOptimizationNode GetPermissionsOptimizationTree()
		{
			SPOptimizationNode sPOptimizationNode;
			try
			{
				string str = this.Adapter.Reader.FindUniquePermissions();
				if (!string.IsNullOrEmpty(str))
				{
					sPOptimizationNode = SPOptimizationNode.InstantiateFromXML(XmlUtility.StringToXmlNode(str));
				}
				else
				{
					sPOptimizationNode = null;
				}
			}
			catch
			{
				sPOptimizationNode = null;
			}
			return sPOptimizationNode;
		}

		internal Metalogix.Metabase.Record GetVirtualRecord()
		{
			bool flag;
			if (this.m_record != null)
			{
				lock (this.m_oLockRecord)
				{
					if (this.m_record == null)
					{
						flag = true;
					}
					else
					{
						flag = (!this.m_record.ParentWorkspace.Exists() ? false : this.m_record.ParentWorkspace.Name.StartsWith(this.WorkspaceGuid));
					}
					if (!flag)
					{
						this.m_record = null;
					}
				}
			}
			if (this.m_record == null)
			{
				lock (this.m_oLockRecord)
				{
					if (this.m_record == null)
					{
						Workspace virtualWorkspace = this.GetVirtualWorkspace();
						if (virtualWorkspace != null)
						{
							this.m_record = virtualWorkspace.FetchSingleRecord(this.Url);
						}
					}
				}
			}
			return this.m_record;
		}

		protected virtual Workspace GetVirtualWorkspace()
		{
			Workspace workspace;
			if (!this.WriteVirtually)
			{
				workspace = null;
			}
			else
			{
				string workspaceGuid = this.WorkspaceGuid;
				if (workspaceGuid == null)
				{
					workspace = null;
				}
				else
				{
					string str = string.Concat(workspaceGuid, base.GetType().Name);
					Workspace workspace1 = this.VirtualConnection.GetWorkspace(str);
					if (workspace1 == null)
					{
						workspace1 = this.VirtualConnection.CreateWorkspace(str);
					}
					workspace = workspace1;
				}
			}
			return workspace;
		}

		public bool IsChildOf(SPNode node)
		{
			bool flag;
			Node parent = base.Parent;
			while (true)
			{
				if (!(parent == null ? false : !(parent is SPServer)))
				{
					flag = false;
					break;
				}
				else if (!(parent.DisplayUrl == node.DisplayUrl))
				{
					parent = parent.Parent;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public bool IsChildOf(SPServer node)
		{
			bool flag;
			Node parent = base.Parent;
			while (true)
			{
				if (parent != null)
				{
					if (parent is SPServer)
					{
						if (parent.DisplayUrl == node.DisplayUrl)
						{
							flag = true;
							break;
						}
					}
					parent = parent.Parent;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public override void ResetActionSourceState()
		{
			lock (this.m_actionSourceLock)
			{
				try
				{
					Metalogix.Metabase.MetabaseConnection virtualConnection = this.VirtualConnection;
					bool flag = true;
					bool flag1 = false;
					string adapterContext = null;
					string adapterType = virtualConnection.Adapter.AdapterType;
					if (adapterType != null)
					{
						if (adapterType == "SqlCe")
						{
							adapterContext = virtualConnection.Adapter.AdapterContext;
							foreach (Metalogix.Explorer.Connection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
							{
								if ((activeConnection.MetabaseConnection == null ? false : activeConnection.MetabaseConnection.MetabaseContext == adapterContext))
								{
									flag1 = false;
									flag = true;
									break;
								}
							}
							flag1 = true;
							flag = false;
							goto Label0;
						}
						else
						{
							if (adapterType != "SqlServer")
							{
								goto Label2;
							}
							flag = true;
							flag1 = false;
							goto Label0;
						}
					}
				Label2:
					flag = true;
					flag1 = false;
				Label0:
					if (flag)
					{
						lock (virtualConnection.Workspaces.WorkspaceLock)
						{
							List<Workspace> workspaces = new List<Workspace>();
							foreach (Workspace workspace in virtualConnection.Workspaces)
							{
								if ((string.IsNullOrEmpty(workspace.Name) ? false : workspace.Name.StartsWith(this.WorkspaceGuid)))
								{
									workspaces.Add(workspace);
								}
							}
							virtualConnection.Workspaces.RemoveRange(workspaces);
							virtualConnection.Workspaces.CommitChanges();
						}
					}
					this.VirtualConnection = null;
					virtualConnection.Dispose();
					if (flag1)
					{
						File.Delete(adapterContext);
					}
					this.m_sWorkspaceGuid = null;
				}
				finally
				{
					base.ResetActionSourceState();
				}
			}
		}

		internal void SaveVirtualData(XmlNode originalNode, XmlNode changedNode, string sPropertyName)
		{
			if (this.WriteVirtually)
			{
				lock (this.m_oLockRecord)
				{
					Metalogix.Metabase.Record virtualRecord = this.GetVirtualRecord();
					if (virtualRecord != null)
					{
						MetabaseUtility.SaveXMLDiffAsProperty(originalNode, changedNode, virtualRecord, sPropertyName, false);
					}
				}
			}
		}

		public override void SetAsActionSource()
		{
			if (this.InUseAsSource)
			{
				throw new Exception(Resources.SourceInUseError);
			}
			lock (this.m_actionSourceLock)
			{
				bool flag = false;
				try
				{
					base.SetAsActionSource();
					this.VirtualConnection = base.MetabaseConnection ?? MetabaseFactory.CreateDefaultMetabaseConnection();
					this.m_sWorkspaceGuid = Guid.NewGuid().ToString();
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						base.ResetActionSourceState();
						this.VirtualConnection = null;
						this.m_sWorkspaceGuid = null;
					}
				}
			}
		}
	}
}