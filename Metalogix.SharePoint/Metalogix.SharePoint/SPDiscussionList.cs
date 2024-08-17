using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPDiscussionList : SPList
	{
		private SPListItemCollection m_allItems;

		private SPDiscussionList.SPDiscussionNodeList m_rootNodeList = null;

		public new SPListItemCollection AllItems
		{
			get
			{
				if (this.m_allItems == null)
				{
					this.m_allItems = base.GetItems(true, ListItemQueryType.ListItem | ListItemQueryType.Folder, null);
				}
				return this.m_allItems;
			}
		}

		public SPListItemCollection DiscussionItems
		{
			get
			{
				return new SPListItemCollection(this, this, this.GetDiscussionItems(0));
			}
		}

		private SPDiscussionList.SPDiscussionNodeList RootNodeList
		{
			get
			{
				if (this.m_rootNodeList == null)
				{
					this.m_rootNodeList = new SPDiscussionList.SPDiscussionNodeList(null);
					foreach (SPDiscussionItem allItem in this.AllItems)
					{
						this.m_rootNodeList.AddItem(allItem);
					}
				}
				return this.m_rootNodeList;
			}
		}

		public SPDiscussionList(SPWeb parentWeb, XmlNode listXML) : base(parentWeb, listXML)
		{
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			this.m_allItems = null;
			this.m_rootNodeList = null;
		}

		public override void CollectionChanged(SPListItemCollection collection)
		{
			base.CollectionChanged(collection);
			if (this.m_allItems != null)
			{
				if (this.m_allItems != collection)
				{
					this.m_allItems = null;
					this.m_rootNodeList = null;
				}
			}
		}

		public SPDiscussionItem[] GetDiscussionItems(int iParentID)
		{
			SPDiscussionList.SPDiscussionNode sPDiscussionNode = this.RootNodeList.FindNode(iParentID);
			SPDiscussionList.SPDiscussionNodeList sPDiscussionNodeList = (sPDiscussionNode == null ? this.RootNodeList : sPDiscussionNode.Children);
			List<SPDiscussionItem> sPDiscussionItems = new List<SPDiscussionItem>();
			foreach (SPDiscussionList.SPDiscussionNode sPDiscussionNode1 in sPDiscussionNodeList)
			{
				sPDiscussionItems.Add(sPDiscussionNode1.Item);
			}
			SPDiscussionItem[] sPDiscussionItemArray = new SPDiscussionItem[sPDiscussionItems.Count];
			sPDiscussionItems.CopyTo(sPDiscussionItemArray);
			return sPDiscussionItemArray;
		}

		public SPDiscussionItem[] GetThread(int iLeafID)
		{
			SPDiscussionList.SPDiscussionNodeList sPDiscussionNodeList = new SPDiscussionList.SPDiscussionNodeList(null);
			foreach (SPDiscussionItem allItem in this.AllItems)
			{
				sPDiscussionNodeList.AddItem(allItem);
			}
			SPDiscussionList.SPDiscussionNode parent = sPDiscussionNodeList.FindNode(iLeafID);
			List<SPDiscussionItem> sPDiscussionItems = new List<SPDiscussionItem>();
			while (parent != null)
			{
				sPDiscussionItems.Insert(0, parent.Item);
				parent = parent.Parent;
			}
			return sPDiscussionItems.ToArray();
		}

		private class SPDiscussionNode
		{
			private SPDiscussionList.SPDiscussionNode m_parent;

			private SPDiscussionItem m_item;

			private SPDiscussionList.SPDiscussionNodeList m_children;

			public SPDiscussionList.SPDiscussionNodeList Children
			{
				get
				{
					if (this.m_children == null)
					{
						this.m_children = new SPDiscussionList.SPDiscussionNodeList(this);
					}
					return this.m_children;
				}
			}

			public SPDiscussionItem Item
			{
				get
				{
					return this.m_item;
				}
			}

			public SPDiscussionList.SPDiscussionNode Parent
			{
				get
				{
					return this.m_parent;
				}
			}

			public SPDiscussionNode(SPDiscussionItem item, SPDiscussionList.SPDiscussionNode parent)
			{
				this.m_item = item;
				this.m_parent = parent;
			}
		}

		private class SPDiscussionNodeList
		{
			private SPDiscussionList.SPDiscussionNode m_owner;

			private List<SPDiscussionList.SPDiscussionNode> m_list;

			public int Count
			{
				get
				{
					return this.m_list.Count;
				}
			}

			public SPDiscussionList.SPDiscussionNode Owner
			{
				get
				{
					return this.m_owner;
				}
			}

			public SPDiscussionNodeList(SPDiscussionList.SPDiscussionNode owner)
			{
				this.m_owner = owner;
			}

			public void AddItem(SPDiscussionItem item)
			{
				bool flag;
				foreach (SPDiscussionList.SPDiscussionNode mList in this.m_list)
				{
					bool flag1 = false;
					if (!mList.Item.Adapter.SharePointVersion.IsSharePoint2007OrLater)
					{
						flag = true;
					}
					else
					{
						flag = (string.IsNullOrEmpty(item.Ordering) ? false : !string.IsNullOrEmpty(mList.Item.Ordering));
					}
					if (!flag)
					{
						if (item.ParentFolderID == mList.Item.ID)
						{
							flag1 = true;
						}
					}
					else if (item.Ordering.StartsWith(mList.Item.Ordering))
					{
						flag1 = true;
					}
					else if (mList.Item.Adapter.SharePointVersion.IsSharePoint2007OrLater)
					{
						if (item.ParentFolderID == mList.Item.ID)
						{
							flag1 = true;
						}
					}
					if (flag1)
					{
						mList.Children.AddItem(item);
						return;
					}
				}
				this.m_list.Add(new SPDiscussionList.SPDiscussionNode(item, this.Owner));
			}

			public SPDiscussionList.SPDiscussionNode FindNode(int iID)
			{
				SPDiscussionList.SPDiscussionNode sPDiscussionNode;
				foreach (SPDiscussionList.SPDiscussionNode sPDiscussionNode1 in this)
				{
					if (sPDiscussionNode1.Item.ID != iID)
					{
						SPDiscussionList.SPDiscussionNode sPDiscussionNode2 = sPDiscussionNode1.Children.FindNode(iID);
						if (sPDiscussionNode2 != null)
						{
							sPDiscussionNode = sPDiscussionNode2;
							return sPDiscussionNode;
						}
					}
					else
					{
						sPDiscussionNode = sPDiscussionNode1;
						return sPDiscussionNode;
					}
				}
				sPDiscussionNode = null;
				return sPDiscussionNode;
			}

			public IEnumerator GetEnumerator()
			{
				return this.m_list.GetEnumerator();
			}
		}
	}
}