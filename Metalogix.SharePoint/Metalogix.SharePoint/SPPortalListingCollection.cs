using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPPortalListingCollection : IEnumerable<SPPortalListing>, IEnumerable
	{
		private List<SPPortalListing> m_data = null;

		private SPWeb m_parentWeb;

		private ReadOnlyCollection<SPPortalListingGroup> m_groups = null;

		public int Count
		{
			get
			{
				int num;
				num = (this.m_data != null ? this.m_data.Count : -1);
				return num;
			}
		}

		public ReadOnlyCollection<SPPortalListingGroup> Groups
		{
			get
			{
				ReadOnlyCollection<SPPortalListingGroup> mGroups;
				if (this.m_groups == null)
				{
					string portalListingGroups = this.ParentWeb.Adapter.Reader.GetPortalListingGroups();
					if (string.IsNullOrEmpty(portalListingGroups))
					{
						this.m_groups = new ReadOnlyCollection<SPPortalListingGroup>(new List<SPPortalListingGroup>(0));
						mGroups = this.m_groups;
						return mGroups;
					}
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(portalListingGroups);
					XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("//Group");
					List<SPPortalListingGroup> sPPortalListingGroups = new List<SPPortalListingGroup>(xmlNodeLists.Count);
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						sPPortalListingGroups.Add(new SPPortalListingGroup(this, xmlNodes));
					}
					this.m_groups = new ReadOnlyCollection<SPPortalListingGroup>(sPPortalListingGroups);
				}
				mGroups = this.m_groups;
				return mGroups;
			}
		}

		public SPPortalListing this[int i]
		{
			get
			{
				SPPortalListing item;
				if (this.m_data != null)
				{
					item = this.m_data[i];
				}
				else
				{
					item = null;
				}
				return item;
			}
		}

		public SPPortalListing this[Guid listingID]
		{
			get
			{
				SPPortalListing sPPortalListing;
				if (this.m_data != null)
				{
					foreach (SPPortalListing mDatum in this.m_data)
					{
						if (mDatum.ListingID == listingID)
						{
							sPPortalListing = mDatum;
							return sPPortalListing;
						}
					}
					sPPortalListing = null;
				}
				else
				{
					sPPortalListing = null;
				}
				return sPPortalListing;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public SPPortalListingCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public void FetchTerseData()
		{
			this.FetchTerseData(false);
		}

		public void FetchTerseData(bool bForceRefetch)
		{
			if ((this.m_data == null ? true : bForceRefetch))
			{
				string portalListingIDs = this.ParentWeb.Adapter.Reader.GetPortalListingIDs();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(portalListingIDs);
				XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("//Listing");
				this.m_data = new List<SPPortalListing>(xmlNodeLists.Count);
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					this.m_data.Add(new SPPortalListing(this, xmlNodes));
				}
			}
		}
        
	    public IEnumerator<SPPortalListing> GetEnumerator()
	    {
	        IEnumerator<SPPortalListing> result;
	        if (this.m_data == null)
	        {
	            result = null;
	        }
	        else
	        {
	            result = this.m_data.GetEnumerator();
	        }
	        return result;
	    }

        public void Sort()
		{
			if (this.m_data == null)
			{
				throw new Exception("Cannot sort data until it is fetched.");
			}
			Comparison<SPPortalListing> comparison = new Comparison<SPPortalListing>(this.SortListingsCompare);
			this.m_data.Sort(comparison);
		}

		private int SortListingsCompare(SPPortalListing sourceListing, SPPortalListing targetListing)
		{
			int num;
			int groupOrder = sourceListing.GroupOrder;
			int groupOrder1 = targetListing.GroupOrder;
			num = (groupOrder == groupOrder1 ? sourceListing.Order.CompareTo(targetListing.Order) : groupOrder.CompareTo(groupOrder1));
			return num;
		}

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        IEnumerator result;
	        if (this.m_data == null)
	        {
	            result = null;
	        }
	        else
	        {
	            result = this.m_data.GetEnumerator();
	        }
	        return result;
	    }

    }
}