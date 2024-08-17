using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPFolderCollection : NodeCollection
	{
		private SPList _parentList;

		private SPFolder _parentFolder;

		protected bool _hasFetched = false;

		public bool HasFetched
		{
			get
			{
				return this._hasFetched;
			}
		}

		public SPFolder ParentFolder
		{
			get
			{
				return this._parentFolder;
			}
		}

		public SPList ParentList
		{
			get
			{
				return this._parentList;
			}
		}

		public SPFolderCollection(SPList parentList, SPFolder parentFolder)
		{
			this._parentList = parentList;
			this._parentFolder = parentFolder;
		}

		public override void Add(Node item)
		{
			if (!(item is SPFolder))
			{
				throw new Exception("The node being added is not a SPFolder");
			}
			this.AddFolder(item.XML, new AddFolderOptions(), AddFolderMode.Comprehensive);
		}

		public SPFolder AddFolder(string xml, AddFolderOptions Options, AddFolderMode Mode = 0)
		{
			if (this.ParentList.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			SPUtils.MapBaseNameToEmptyTitle(ref xml);
			string str = null;
			if (Mode != AddFolderMode.Optimistic)
			{
				str = this.ParentList.Adapter.Writer.AddFolder(this.ParentList.ID, this.ParentFolder.FolderPath, xml, Options);
			}
			else
			{
				FieldsLookUp fieldsSchemaLookup = this._parentList.FieldCollection.GetFieldsSchemaLookup();
				str = this._parentList.Adapter.Writer.AddFolderOptimistically(new Guid(this._parentList.ID), this.ParentList.Name, this.ParentFolder.FolderPath, xml, Options, ref fieldsSchemaLookup);
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			SPFolder sPFolder = SPFolder.CreateFolder(this.ParentList, this.ParentFolder, xmlDocument.SelectSingleNode("//Folder | //ListItem"));
			int indexByName = base.GetIndexByName(sPFolder.Name);
			if (indexByName >= 0)
			{
				base.RemoveIndex(indexByName);
			}
			base.AddToCollection(sPFolder);
			base.Sort(new FolderNameSorter());
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, sPFolder);
			return sPFolder;
		}

		internal SPFolder AddFolderFromListItem(SPListItem item)
		{
			SPFolder sPFolder;
			SPList parentList = this.ParentList;
			SPFolder subFolderByPath = parentList;
			if (subFolderByPath.SubFolders.HasFetched)
			{
				string str = item.ServerRelativeUrl.Replace(parentList.ServerRelativeUrl, "").Trim(new char[] { '/' });
				int num = str.LastIndexOf('/');
				if (num > 1)
				{
					subFolderByPath = subFolderByPath.GetSubFolderByPath(str.Substring(0, num));
					if (subFolderByPath == null)
					{
						sPFolder = null;
						return sPFolder;
					}
				}
				SPFolder sPFolder1 = SPFolder.CreateFolder(parentList, subFolderByPath, item.GetNodeXML());
				int indexByName = subFolderByPath.SubFolders.GetIndexByName(sPFolder1.Name);
				if (indexByName >= 0)
				{
					subFolderByPath.SubFolders.RemoveIndex(indexByName);
				}
				subFolderByPath.SubFolders.AddToCollection(sPFolder1);
				subFolderByPath.SubFolders.Sort(new FolderNameSorter());
				subFolderByPath.SubFolders.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, sPFolder1);
				sPFolder = sPFolder1;
			}
			else
			{
				sPFolder = null;
			}
			return sPFolder;
		}

		public bool DeleteFolder(string sFolderName)
		{
			if (this.ParentFolder.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			SPFolder item = (SPFolder)base[sFolderName];
			this.ParentFolder.Adapter.Writer.DeleteFolder(this.ParentList.ID, item.ItemID, item.FolderPath);
			bool flag = base.RemoveFromCollection(item);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, item);
			return flag;
		}

		public virtual void FetchData()
		{
			this.ClearCollection();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(this._parentList.Adapter.Reader.GetFolders(this._parentList.ID, null, this.ParentFolder.DirName));
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Folder"))
			{
				base.AddToCollection(SPFolder.CreateFolder(this.ParentList, this.ParentFolder, xmlNodes));
			}
			base.Sort(new FolderNameSorter());
			this._hasFetched = true;
		}

		public override bool Remove(Node item)
		{
			if (!(item is SPFolder))
			{
				throw new Exception("The node being removed is not a SPFolder");
			}
			return this.DeleteFolder(((SPFolder)item).Name);
		}
	}
}