using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPSearchResultsCollection : ListItemCollection
	{
		private SPSearchParameters m_parameters = new SPSearchParameters();

		private string m_searchTerm = null;

		private SPNode m_searchNode = null;

		public override Type CollectionType
		{
			get
			{
				Type type;
				if ((base.Count == 0 ? false : this[0] != null))
				{
					type = this[0].GetType();
				}
				else
				{
					type = null;
				}
				return type;
			}
		}

		public SPSearchParameters Parameters
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				value.ListID = this.m_parameters.ListID;
				this.m_parameters = value;
			}
		}

		public ResultsFieldCollection ResultFields
		{
			get
			{
				return new ResultsFieldCollection();
			}
		}

		public SPNode SearchNode
		{
			get
			{
				return this.m_searchNode;
			}
		}

		public string SearchTerm
		{
			get
			{
				return this.m_searchTerm;
			}
		}

		public SPSearchResultsCollection(SPNode searchNode, string sSearchTerm) : base(null, null, null)
		{
			if (searchNode == null)
			{
				throw new Exception("Cannot create a search results collection with no node to search");
			}
			this.m_searchNode = searchNode;
			if (typeof(SPFolder).IsAssignableFrom(searchNode.GetType()))
			{
				this.Parameters.ListID = ((SPFolder)searchNode).ParentList.ID;
			}
			this.m_searchTerm = (sSearchTerm != null ? sSearchTerm : "");
		}

		public SPSearchResultsCollection(ListItemCollection collection) : base(null, null, null)
		{
			foreach (ListItem listItem in collection)
			{
				SPSearchResult sPSearchResult = listItem as SPSearchResult;
				if (sPSearchResult != null)
				{
					sPSearchResult.Changed += new PropertyChangedEventHandler(this.SearchResultChanged);
					base.AddToCollection(sPSearchResult);
				}
			}
		}

		public override void Add(Node item)
		{
			if (!(item is SPSearchResult))
			{
				throw new Exception("The node being added is not a SPSearchResult");
			}
			base.Add(item);
		}

		private void AppendFromXML(XmlNode xmlNode)
		{
			this.AppendFromXML(xmlNode, this.m_searchNode);
		}

		private void AppendFromXML(XmlNode xmlNode, SPNode baseNode)
		{
			if (xmlNode != null)
			{
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//Result"))
				{
					SPSearchResult sPSearchResult = new SPSearchResult(baseNode, xmlNodes);
					sPSearchResult.Changed += new PropertyChangedEventHandler(this.SearchResultChanged);
					base.AddToCollection(sPSearchResult);
				}
			}
		}

		public override void ClearCollection()
		{
			foreach (SPSearchResult sPSearchResult in this)
			{
				sPSearchResult.Changed -= new PropertyChangedEventHandler(this.SearchResultChanged);
			}
			base.ClearCollection();
		}

		public bool ExecuteSearch()
		{
			string str;
			XmlDocument xmlDocument;
			bool flag;
			if (!(this.m_searchNode is SPServer))
			{
				bool connectedAsSiteAdmin = true;
				if (typeof(SPWeb).IsAssignableFrom(this.m_searchNode.GetType()))
				{
					connectedAsSiteAdmin = ((SPWeb)this.m_searchNode).ConnectedAsSiteAdmin;
				}
				else if (typeof(SPFolder).IsAssignableFrom(this.m_searchNode.GetType()))
				{
					connectedAsSiteAdmin = ((SPFolder)this.m_searchNode).ParentList.ParentWeb.ConnectedAsSiteAdmin;
				}
				if (connectedAsSiteAdmin)
				{
					str = this.m_searchNode.Adapter.Reader.SearchForDocument(this.SearchTerm, this.Parameters.ToXML());
					xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(str);
					this.FromXML(xmlDocument.SelectSingleNode("//ResultsCollection"));
				}
				flag = connectedAsSiteAdmin;
			}
			else
			{
				bool flag1 = true;
				this.ClearCollection();
				foreach (SPSite site in ((SPServer)this.m_searchNode).Sites)
				{
					try
					{
						if (!site.ConnectedAsSiteAdmin)
						{
							flag1 = false;
						}
						else
						{
							str = site.Adapter.Reader.SearchForDocument(this.SearchTerm, this.Parameters.ToXML());
							xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(str);
							this.AppendFromXML(xmlDocument.SelectSingleNode("//ResultsCollection"), site);
						}
					}
					catch
					{
						flag1 = false;
					}
				}
				flag = flag1;
			}
			return flag;
		}

		public override void FromXML(XmlNode xmlNode)
		{
			this.ClearCollection();
			this.AppendFromXML(xmlNode);
		}

		public override bool Remove(Node item)
		{
			if (!(item is SPSearchResult))
			{
				throw new Exception("The node being removed is not a SPSearchResult");
			}
			return base.Remove(item);
		}

		private void SearchResultChanged(object sender, PropertyChangedEventArgs e)
		{
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeChanged, (this.m_searchNode != null || !((SPSearchResult)sender).HasNode ? (SPNode)sender : ((SPSearchResult)sender).Node));
		}
	}
}