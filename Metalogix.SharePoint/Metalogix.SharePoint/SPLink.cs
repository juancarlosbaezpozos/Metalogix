using System;

namespace Metalogix.SharePoint
{
	public class SPLink
	{
		private string m_sLinkValue;

		private LinkType m_sLinkType;

		private string m_sLinkSourceUrl;

		private SPNode m_Parent;

		private string m_sPropertyNameWithinParent;

		public LinkType LinkSourceType
		{
			get
			{
				return this.m_sLinkType;
			}
			protected set
			{
				this.m_sLinkType = value;
			}
		}

		public string LinkSourceUrl
		{
			get
			{
				return this.m_sLinkSourceUrl;
			}
			protected set
			{
				this.m_sLinkSourceUrl = value;
			}
		}

		public string LinkValue
		{
			get
			{
				return this.m_sLinkValue;
			}
			protected set
			{
				this.m_sLinkValue = value;
			}
		}

		public SPNode Parent
		{
			get
			{
				return this.m_Parent;
			}
			protected set
			{
				this.m_Parent = value;
			}
		}

		public string PropertyNameWithinParent
		{
			get
			{
				return this.m_sPropertyNameWithinParent;
			}
			protected set
			{
				this.m_sPropertyNameWithinParent = value;
			}
		}

		public SPLink(string sLinkValue, LinkType linkSourceType, SPNode parentNode, string sPropertyNameWithinParent)
		{
			this.LinkValue = sLinkValue;
			this.LinkSourceType = linkSourceType;
			this.Parent = parentNode;
			this.PropertyNameWithinParent = sPropertyNameWithinParent;
		}

		public SPLink(string sLinkValue, LinkType linkSourceType, string sLinkSourceUrl, string sPropertyNameWithinParent)
		{
			this.LinkValue = sLinkValue;
			this.LinkSourceType = linkSourceType;
			this.LinkSourceUrl = sLinkSourceUrl;
			this.PropertyNameWithinParent = sPropertyNameWithinParent;
		}
	}
}