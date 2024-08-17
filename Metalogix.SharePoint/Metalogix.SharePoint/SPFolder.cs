using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.BCS;
using Metalogix.SharePoint.Nintex;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Image("Metalogix.SharePoint.Icons.SPFolder.ico")]
	[Name("Folder")]
	[PluralName("Folders")]
	public class SPFolder : SPNode, ISecurableObject, Folder
	{
		protected XmlNode _xmlNode;

		protected bool _hasFetched = false;

		private int _itemId = -1;

		private string _contentTypeId = null;

		private int? _ctId = null;

		private SPList _parentList;

		private SPFolder _parentFolder;

		private SPFolderCollection _folders = null;

		private SPListItemCollection _items = null;

		private SPRoleAssignmentCollection _roleAssignments = null;

		protected readonly object _lockXml = new object();

		private object _lockItems = new object();

		private object _lockRoleAssignments = new object();

		private string m_sConstantFileLeafRef = null;

		private string m_sConstantFileDirRef = null;

		private string m_sConstantContentTypeID = null;

		private string m_sFileLeafRef = null;

		private string m_sFileDirRef = null;

		protected string m_sWebRelativeUrl = null;

		private int m_iItemCount = 0;

		private bool? _hasUniquePermissions = null;

		public SPListItemCollection AllItems
		{
			get
			{
				SPListItemCollection itemCollection = this.GetItemCollection(true, (SPListItemCollection items) => items.FetchData(true, ListItemQueryType.ListItem | ListItemQueryType.Folder, null));
				return itemCollection;
			}
		}

		public string ConstantContentTypeId
		{
			get
			{
				return this._contentTypeId;
			}
		}

		internal virtual string ConstantDirName
		{
			get
			{
				return string.Concat((this.m_sConstantFileDirRef == "" ? "" : string.Concat(this.m_sConstantFileDirRef, "/")), this.ConstantName);
			}
		}

		internal virtual string ConstantFolderPath
		{
			get
			{
				return (this is SPList ? "" : string.Concat(this.ParentFolder.ConstantFolderPath, "/", this.ConstantName));
			}
		}

		public int ConstantItemID
		{
			get
			{
				return this._ctId.Value;
			}
		}

		public virtual string ConstantName
		{
			get
			{
				return this.m_sConstantFileLeafRef;
			}
		}

		public string ConstantServerRelativeUrl
		{
			get
			{
				return this.ConstantDirName;
			}
		}

		protected string ContentTypeId
		{
			get
			{
				return this._contentTypeId;
			}
		}

		public virtual DateTime Created
		{
			get
			{
				DateTime dateTime = Utils.ParseDateAsUtc(this.FolderXML.Attributes["Created"].Value);
				return dateTime;
			}
		}

		public virtual string CreatedBy
		{
			get
			{
				return this.FolderXML.Attributes["CreatedBy"].Value;
			}
		}

		public virtual string DirName
		{
			get
			{
				return string.Concat((this.m_sFileDirRef == "" ? "" : string.Concat(this.m_sFileDirRef, "/")), this.Name);
			}
		}

		public override string DisplayName
		{
			get
			{
				string name;
				if (this.ItemCount <= 0)
				{
					name = this.Name;
				}
				else
				{
					string str = (this.ItemCount > 1 ? "s" : string.Empty);
					name = string.Format("{0} ({1} item{2})", this.Name, this.ItemCount, str);
				}
				return name;
			}
		}

		public virtual string FolderPath
		{
			get
			{
				return (this is SPList ? "" : string.Concat(this.ParentFolder.FolderPath, "/", this.Name));
			}
		}

		public XmlNode FolderXML
		{
			get
			{
				XmlNode xmlNodes;
				lock (this._lockXml)
				{
					if (this._xmlNode == null)
					{
						this.SetFullXML(this.GetFolderXml(false));
					}
					xmlNodes = this._xmlNode;
				}
				return xmlNodes;
			}
		}

		[IsSystem(true)]
		internal bool HasFolders
		{
			get
			{
				return this._folders != null;
			}
		}

		public virtual bool HasUniquePermissions
		{
			get
			{
				if (!this._hasUniquePermissions.HasValue)
				{
					if (this.FolderXML.Attributes["HasUniquePermissions"] != null)
					{
						this._hasUniquePermissions = new bool?(bool.Parse(this.FolderXML.Attributes["HasUniquePermissions"].Value));
					}
					else if (!base.Adapter.IsNws)
					{
						this._hasUniquePermissions = new bool?(false);
					}
					else
					{
						bool flag = false;
						bool.TryParse(base.Adapter.Reader.HasUniquePermissions(this._parentList.ConstantID, this.ConstantItemID), out flag);
						this._hasUniquePermissions = new bool?(flag);
					}
				}
				return this._hasUniquePermissions.Value;
			}
		}

		public override string ImageName
		{
			get
			{
				return ((string.IsNullOrEmpty(this.ContentTypeId) ? true : !this.ContentTypeId.ToLower().StartsWith("0x0120d520")) ? "Metalogix.SharePoint.Icons.SPFolder.ico" : "Metalogix.SharePoint.Icons.SPDocumentSet.ico");
			}
		}

		public bool IsDocumentSet
		{
			get
			{
				return (string.IsNullOrEmpty(this.ContentTypeId) ? false : this.ContentTypeId.ToLower().StartsWith("0x0120d520"));
			}
		}

		public bool IsOneNoteFolder
		{
			get
			{
				bool flag;
				XmlNode nodeXML = this.GetNodeXML();
				string[] strArrays = new string[] { "OneNote.Notebook" };
				if (nodeXML.IsAttributeValueInSet("ProgId", strArrays))
				{
					flag = false;
				}
				else
				{
					strArrays = new string[] { "OneNote.Notebook" };
					flag = !nodeXML.IsAttributeValueInSet("HTML_x0020_File_x0020_Type", strArrays);
				}
				return (flag ? false : true);
			}
		}

		public int ItemCount
		{
			get
			{
				return this.m_iItemCount;
			}
		}

		public virtual int ItemID
		{
			get
			{
				return this._itemId;
			}
		}

		public SPListItemCollection Items
		{
			get
			{
				SPListItemCollection itemCollection = this.GetItemCollection(false, (SPListItemCollection items) => items.FetchData());
				return itemCollection;
			}
		}

		public virtual DateTime Modified
		{
			get
			{
				DateTime dateTime = Utils.ParseDateAsUtc(this.FolderXML.Attributes["Modified"].Value);
				return dateTime;
			}
		}

		public virtual string ModifiedBy
		{
			get
			{
				return this.FolderXML.Attributes["ModifiedBy"].Value;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_sFileLeafRef;
			}
		}

		public virtual SPFolder ParentFolder
		{
			get
			{
				return this._parentFolder;
			}
		}

		public virtual SPList ParentList
		{
			get
			{
				return this._parentList;
			}
		}

		public virtual SecurityPrincipalCollection Principals
		{
			get
			{
				return this.ParentList.ParentWeb.Principals;
			}
		}

		public virtual RoleAssignmentCollection RoleAssignments
		{
			get
			{
				lock (this._lockRoleAssignments)
				{
					if (this._roleAssignments == null)
					{
						this._roleAssignments = this.GetRoleAssignmentCollection(false);
						this._roleAssignments.RoleAssignmentCollectionChanged += new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
					}
				}
				return this._roleAssignments;
			}
		}

		public virtual RoleCollection Roles
		{
			get
			{
				return this.ParentList.ParentWeb.Roles;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				return this.DirName;
			}
		}

		public virtual SPFolderCollection SubFolders
		{
			get
			{
				if (this._folders == null)
				{
					SPFolderCollection sPFolderCollection = new SPFolderCollection(this.ParentList, this);
					sPFolderCollection.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_subFolders_CollectionChanged);
					sPFolderCollection.FetchData();
					this._folders = sPFolderCollection;
				}
				return this._folders;
			}
		}

		public override string Url
		{
			get
			{
				return string.Concat(this.ParentList.Url, this.FolderPath);
			}
		}

		public virtual string WebRelativeUrl
		{
			get
			{
				if (this.m_sWebRelativeUrl == null)
				{
					string serverRelativeUrl = this.ServerRelativeUrl;
					char[] chrArray = new char[] { '/' };
					string str = serverRelativeUrl.Trim(chrArray);
					string serverRelativeUrl1 = this.ParentList.ParentWeb.ServerRelativeUrl;
					chrArray = new char[] { '/' };
					string str1 = str.Substring(serverRelativeUrl1.Trim(chrArray).Length);
					chrArray = new char[] { '/' };
					this.m_sWebRelativeUrl = str1.Trim(chrArray);
				}
				return this.m_sWebRelativeUrl;
			}
		}

		public override string XML
		{
			get
			{
				return this.FolderXML.OuterXml;
			}
		}

		protected SPFolder(SPList parentList, SPFolder parentFolder, XmlNode xmlNode) : base(parentList.Adapter, parentFolder)
		{
			this._parentFolder = parentFolder;
			this._parentList = parentList;
			this.StoreQuickProperties(xmlNode);
		}

		protected SPFolder(SharePointAdapter adapter, SPNode parent) : base(adapter, parent)
		{
		}

		public override bool AnalyzeChurn(DateTime pivotDate, bool bRecursive, out long lByteschanged, out long lItemsChanged)
		{
			bool flag;
			lByteschanged = (long)0;
			lItemsChanged = (long)0;
			if (this.ParentList.ParentWeb.IsSearchable)
			{
				string str = base.Adapter.Reader.AnalyzeChurn(pivotDate, this.ParentList.ConstantID, this.ConstantItemID, bRecursive);
				if (!string.IsNullOrEmpty(str))
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
					string str1 = (xmlNode.Attributes["ItemsChanged"] != null ? xmlNode.Attributes["ItemsChanged"].Value : "0");
					string str2 = (xmlNode.Attributes["BytesChanged"] != null ? xmlNode.Attributes["BytesChanged"].Value : "0");
					bool flag1 = long.TryParse(str1, out lItemsChanged);
					flag1 = long.TryParse(str2, out lByteschanged);
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		protected override void ClearChildNodes()
		{
			if (this._folders != null)
			{
				this._folders.OnNodeCollectionChanged -= new NodeCollectionChangedHandler(this.On_subFolders_CollectionChanged);
				this._folders = null;
			}
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			lock (this._lockXml)
			{
				this._xmlNode = null;
				this._hasFetched = false;
			}
			this.ResetQuickProperties();
			this.ReleasePermissionsData();
			lock (this._lockItems)
			{
				if (this._items != null)
				{
					this._items.OnNodeCollectionChanged -= new NodeCollectionChangedHandler(this.On_items_CollectionChanged);
					if (this._items.HasNodeCollectionChangedListeners)
					{
						this._items.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_items_CollectionChanged);
					}
					else
					{
						this._items = null;
					}
				}
			}
		}

		public virtual void CollectionChanged(SPListItemCollection collection)
		{
			if ((collection.ParentFolder != this || collection.ParentList != this.ParentList || this._items == null ? false : this._items != collection))
			{
				lock (this._lockItems)
				{
					this._items = null;
				}
			}
		}

		public static SPFolder CreateFolder(SPList parentList, SPFolder parentFolder, XmlNode xmlNode)
		{
			SPFolder sPFolder;
			if (!(parentList is SPNintexWorkflowList))
			{
				sPFolder = new SPFolder(parentList, parentFolder, xmlNode);
			}
			else
			{
				sPFolder = new SPNintexWorkflow(parentList, parentFolder, xmlNode);
			}
			return sPFolder;
		}

		public virtual void Delete()
		{
			if (base.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			base.Adapter.Writer.DeleteFolder(this.ParentList.ConstantID, this.ConstantItemID, this.ConstantFolderPath);
		}

		protected override Node[] FetchChildNodes()
		{
			Node[] item = new Node[this.SubFolders.Count];
			for (int i = 0; i < this.SubFolders.Count; i++)
			{
				item[i] = (Node)this.SubFolders[i];
			}
			return item;
		}

		protected virtual void FetchData()
		{
			if (this.GetFolderXml(true) != null)
			{
				XmlNode folderXml = this.GetFolderXml(true);
				this.StoreQuickProperties(folderXml);
				lock (this._lockXml)
				{
					this._xmlNode = folderXml;
				}
			}
		}

		internal XmlNode GetFolderXml(bool bAlwaysRefetch)
		{
			XmlNode xmlNodes;
			lock (this._lockXml)
			{
				if ((this._xmlNode == null ? false : !bAlwaysRefetch))
				{
					xmlNodes = this._xmlNode;
					return xmlNodes;
				}
			}
			GetListItemOptions getListItemOption = new GetListItemOptions()
			{
				IncludeExternalizationData = true,
				IncludePermissionsInheritance = true
			};
			ISharePointReader reader = base.Adapter.Reader;
			string constantID = this.ParentList.ConstantID;
			int constantItemID = this.ConstantItemID;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(reader.GetListItems(constantID, constantItemID.ToString(), null, this.ParentFolder.DirName, true, ListItemQueryType.ListItem | ListItemQueryType.Folder, this.ParentList.GetListXML(false).OuterXml, getListItemOption));
			xmlNodes = base.AttachVirtualData(xmlNode.SelectSingleNode("//ListItem"), "XML");
			return xmlNodes;
		}

		protected SPListItemCollection GetItemCollection(bool bForceFetch, SPFolder.SPListItemCollectionFetchDelegate fetchDeleg)
		{
			if (fetchDeleg == null)
			{
				throw new Exception("Could not get list item collection. No fetch delegate provided.");
			}
			SPListItemCollection sPListItemCollection = this._items;
			if (sPListItemCollection == null)
			{
				bool flag = false;
				lock (this._lockItems)
				{
					if (this._items == null)
					{
						SPListItemCollection sPListItemCollection1 = new SPListItemCollection(this.ParentList, this, null);
						sPListItemCollection1.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_items_CollectionChanged);
						fetchDeleg(sPListItemCollection1);
						this._items = sPListItemCollection1;
						flag = true;
					}
					sPListItemCollection = this._items;
				}
				if ((flag ? false : bForceFetch))
				{
					fetchDeleg(sPListItemCollection);
				}
			}
			else if (bForceFetch)
			{
				fetchDeleg(sPListItemCollection);
			}
			return sPListItemCollection;
		}

		public SPListItemCollection GetItems(bool bRecursive, ListItemQueryType itemTypes, string sFields, bool bIncludeExternalizationData, bool bIncludePermissionsInheritance)
		{
			if (this._items == null)
			{
				this._items = new SPListItemCollection(this.ParentList, this, null);
				this._items.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_items_CollectionChanged);
			}
			this._items.FetchData(bRecursive, itemTypes, sFields, bIncludeExternalizationData, bIncludePermissionsInheritance);
			return this._items;
		}

		public SPListItemCollection GetItems(bool bRecursive, ListItemQueryType itemTypes, string sFields)
		{
			SPListItemCollection itemCollection = this.GetItemCollection(true, (SPListItemCollection items) => items.FetchData(bRecursive, itemTypes, sFields));
			return itemCollection;
		}

		public SPListItemCollection GetItems(bool bRecursive, ListItemQueryType itemTypes)
		{
			SPListItemCollection itemCollection = this.GetItemCollection(true, (SPListItemCollection items) => items.FetchDataWithAllFieldData(bRecursive, itemTypes));
			return itemCollection;
		}

		public ListItemCollection GetItems()
		{
			ListItemCollection items;
			if (this.ParentList.BaseTemplate != ListTemplateType.ExternalList)
			{
				items = this.GetItems(false, ListItemQueryType.ListItem);
			}
			else
			{
				SPExternalContentType externalContentType = this.ParentList.ExternalContentType;
				if (externalContentType == null)
				{
					throw new Exception("Unable to find the external content type for the list.");
				}
				SPExternalListItemCollection sPExternalListItemCollection = new SPExternalListItemCollection(this.ParentList, this, externalContentType, null);
				sPExternalListItemCollection.FetchData();
				items = sPExternalListItemCollection;
			}
			return items;
		}

		public override Node GetNodeByPath(string sPath)
		{
			Node node;
			char[] chrArray = new char[] { '/' };
			string str = sPath.Trim(chrArray);
			string str1 = str.Trim();
			string str2 = "";
			int num = str.IndexOfAny(chrArray);
			if (num >= 0)
			{
				str1 = str.Substring(0, num);
				str2 = str.Substring(num);
			}
			Node item = null;
			if (str2 == "")
			{
				item = this.Items[str1];
			}
			if (item == null)
			{
				item = base.Children[str1];
			}
			if (item != null)
			{
				node = (!(str2 == "") ? item.GetNodeByPath(str2) : item);
			}
			else
			{
				node = null;
			}
			return node;
		}

		public override Node GetNodeByUrl(string sURL)
		{
			Node nodeByUrl = base.GetNodeByUrl(sURL);
			if (nodeByUrl == null)
			{
				nodeByUrl = this.GetItems(true, ListItemQueryType.ListItem, null).GetNodeByUrl(sURL);
			}
			return nodeByUrl;
		}

		public string GetReferencedManagedMetadata()
		{
			string referencedTaxonomyFullXml;
			SPFieldCollection taxonomyFields = this.ParentList.FieldCollection.GetTaxonomyFields();
			if (taxonomyFields.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings);
				try
				{
					xmlWriter.WriteStartElement("TaxonomyFields");
					foreach (SPField taxonomyField in taxonomyFields)
					{
						if (this.FolderXML.Attributes[taxonomyField.Name] != null)
						{
							taxonomyField.SetReferencedManagedMetadata(xmlWriter, this.FolderXML.Attributes[taxonomyField.Name].Value);
						}
					}
					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
				}
				finally
				{
					if (xmlWriter != null)
					{
						((IDisposable)xmlWriter).Dispose();
					}
				}
				referencedTaxonomyFullXml = this.ParentList.Adapter.Reader.GetReferencedTaxonomyFullXml(stringBuilder.ToString());
			}
			else
			{
				referencedTaxonomyFullXml = string.Empty;
			}
			return referencedTaxonomyFullXml;
		}

		internal virtual SPRoleAssignmentCollection GetRoleAssignmentCollection(bool bAlwaysRefetch)
		{
			SPRoleAssignmentCollection sPRoleAssignmentCollection;
			lock (this._lockRoleAssignments)
			{
				if ((this._roleAssignments == null ? false : !bAlwaysRefetch))
				{
					sPRoleAssignmentCollection = this._roleAssignments;
					return sPRoleAssignmentCollection;
				}
			}
			string str = null;
			str = (!this.HasUniquePermissions ? this.ParentFolder.GetRoleAssignmentCollection(false).ToXML() : base.Adapter.Reader.GetRoleAssignments(this.ParentList.ID, this.ItemID));
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			XmlNode xmlNodes = base.AttachVirtualData(xmlDocument.DocumentElement, "RoleAssignments");
			sPRoleAssignmentCollection = new SPRoleAssignmentCollection(this, xmlNodes);
			return sPRoleAssignmentCollection;
		}

		public SPFolder GetSubFolderByPath(string sPath)
		{
			SPFolder sPFolder;
			if (!string.IsNullOrEmpty(sPath))
			{
				char[] chrArray = new char[] { '/', '\\' };
				string str = sPath.Trim(chrArray);
				string str1 = str.Trim();
				string str2 = "";
				int num = str.IndexOfAny(chrArray);
				if (num >= 0)
				{
					str1 = str.Substring(0, num);
					str2 = str.Substring(num);
				}
				if (!(str1 == ""))
				{
					SPFolder item = (SPFolder)this.SubFolders[str1];
					if (item != null)
					{
						sPFolder = (!(str2 == "") ? item.GetSubFolderByPath(str2) : item);
					}
					else
					{
						sPFolder = null;
					}
				}
				else
				{
					sPFolder = this;
				}
			}
			else
			{
				sPFolder = this;
			}
			return sPFolder;
		}

		public SPListItemCollection GetTerseItemReferences(bool bRecursive, string sIDs, ListItemQueryType itemTypes, GetListItemOptions getOptions)
		{
			SPListItemCollection sPListItemCollection = new SPListItemCollection(this.ParentList, this, null);
			sPListItemCollection.FetchTerseData(bRecursive, itemTypes, sIDs, null, getOptions, false);
			return sPListItemCollection;
		}

		public SPListItemCollection GetTerseItems(bool bRecursive, ListItemQueryType itemTypes, string sFields, GetListItemOptions getOptions)
		{
			SPListItemCollection itemCollection = this.GetItemCollection(true, (SPListItemCollection items) => items.FetchTerseData(bRecursive, itemTypes, sFields, getOptions, false));
			return itemCollection;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			if (!(comparableNode is SPFolder))
			{
				throw new Exception("SPFolder can only be compared to another SPFolder");
			}
			SPFolder sPFolder = (SPFolder)comparableNode;
			if (!(this.Name != sPFolder.Name))
			{
				flag = true;
			}
			else
			{
				differencesOutput.Write(string.Concat("The folder name: '", this.Name, "' is different. "), this.Name);
				flag = false;
			}
			return flag;
		}

		private void On_items_CollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
		{
			base.SetChildren(null);
		}

		private void On_subFolders_CollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
		{
			base.SetChildren(null);
		}

		public virtual void ReleasePermissionsData()
		{
			lock (this._lockRoleAssignments)
			{
				if (this._roleAssignments != null)
				{
					this._roleAssignments.RoleAssignmentCollectionChanged -= new CollectionChangeEventHandler(this.RoleAssignmentCollectionChanged);
				}
				this._roleAssignments = null;
			}
		}

		private void ResetQuickProperties()
		{
			if (this._ctId.HasValue)
			{
				this._itemId = this._ctId.Value;
			}
			this.m_sFileLeafRef = this.m_sConstantFileLeafRef;
			this.m_sFileDirRef = this.m_sConstantFileDirRef;
			this._contentTypeId = this.m_sConstantContentTypeID;
		}

		private void RoleAssignmentCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if ((this._xmlNode == null ? false : this._xmlNode.Attributes["HasUniquePermissions"] != null))
			{
				this._xmlNode.Attributes["HasUniquePermissions"].Value = "True";
			}
			this._hasUniquePermissions = new bool?(true);
		}

		public void SetFullXML(XmlNode xmlNode)
		{
			lock (this._lockXml)
			{
				this._xmlNode = xmlNode;
			}
			this.StoreQuickProperties(xmlNode);
		}

		private void StoreQuickProperties(XmlNode folderNode)
		{
			string value;
			this._itemId = int.Parse(folderNode.Attributes["ID"].Value);
			this.m_sFileLeafRef = folderNode.Attributes["FileLeafRef"].Value;
			this.m_sFileDirRef = folderNode.Attributes["FileDirRef"].Value;
			XmlAttribute itemOf = folderNode.Attributes["ItemCount"];
			this.m_iItemCount = (itemOf != null ? Convert.ToInt32(itemOf.Value) : 0);
			if (folderNode.Attributes["ContentTypeId"] == null)
			{
				value = null;
			}
			else
			{
				value = folderNode.Attributes["ContentTypeId"].Value;
			}
			this._contentTypeId = value;
			if (!this._ctId.HasValue)
			{
				this._ctId = new int?(this._itemId);
				this.m_sConstantFileLeafRef = this.m_sFileLeafRef;
				this.m_sConstantFileDirRef = this.m_sFileDirRef;
				this.m_sConstantContentTypeID = this._contentTypeId;
			}
		}

		public override void UpdateCurrentNode()
		{
			this.ClearExcessNodeData();
			if (base.HasEventClients)
			{
				this.FetchData();
				base.UpdateCurrentNode();
			}
		}

		public virtual void UpdateSettings(string sXml)
		{
			if (!this.WriteVirtually)
			{
				if (base.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				string str = base.Adapter.Writer.UpdateListItem(this.ParentList.ConstantID, this.ParentFolder.ConstantName, this.ConstantItemID, sXml, null, null, new UpdateListItemOptions());
				this.StoreQuickProperties(XmlUtility.StringToXmlNode(str).SelectSingleNode(".//ListItem"));
			}
			else
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(sXml);
				base.SaveVirtualData(this.FolderXML, xmlNode, "XML");
				this.SetFullXML(xmlNode);
			}
		}

		protected delegate void SPListItemCollectionFetchDelegate(SPListItemCollection collection);
	}
}