using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPPortalListingGroup
	{
		private string m_sName;

		private int m_iID;

		private int m_iOrder;

		private SPPortalListingCollection m_parentCollection;

		public int ID
		{
			get
			{
				return this.m_iID;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public int Order
		{
			get
			{
				return this.m_iOrder;
			}
		}

		public SPPortalListingCollection ParentCollection
		{
			get
			{
				return this.m_parentCollection;
			}
		}

		public SPPortalListingGroup(SPPortalListingCollection parentCollection, XmlNode node)
		{
			this.m_parentCollection = parentCollection;
			this.m_sName = node.Attributes["Name"].Value;
			if (!int.TryParse(node.Attributes["ID"].Value, out this.m_iID))
			{
				this.m_iID = -1;
			}
			if (!int.TryParse(node.Attributes["Order"].Value, out this.m_iOrder))
			{
				this.m_iOrder = -1;
			}
		}
	}
}