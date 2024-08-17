using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalListItemCollection : ListItemCollection
	{
		private SPList m_parentList;

		private SPFolder m_parentFolder;

		private SPExternalContentType m_parentExternalContentType;

		public SPExternalContentType ExternalContentType
		{
			get
			{
				return this.m_parentExternalContentType;
			}
		}

		public override Folder ParentFolder
		{
			get
			{
				return this.m_parentFolder;
			}
		}

		public override List ParentList
		{
			get
			{
				return this.m_parentList;
			}
		}

		public SPList ParentSPList
		{
			get
			{
				return this.m_parentList;
			}
		}

		public SPExternalListItemCollection(SPList parentList, SPFolder parentFolder, SPExternalContentType parentContentType, Node[] items) : base(parentList, parentFolder, items)
		{
			this.m_parentList = parentList;
			this.m_parentFolder = parentFolder;
			this.m_parentExternalContentType = parentContentType;
		}

		public void FetchData()
		{
			base.Clear();
			foreach (SPExternalItem externalItem in this.ExternalContentType.GetExternalItems(null, this.ParentSPList.ConstantID))
			{
				SPExternalListItem sPExternalListItem = SPExternalListItem.CreateListItem(this.m_parentList, this.m_parentFolder, this, externalItem);
				base.AddToCollection(sPExternalListItem, false);
				this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, sPExternalListItem);
			}
		}
	}
}