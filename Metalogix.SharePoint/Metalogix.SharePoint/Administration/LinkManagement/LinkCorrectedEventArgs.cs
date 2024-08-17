using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkCorrectedEventArgs : EventArgs
	{
		private readonly string m_sOldLink;

		private readonly LinkInfo m_linkNew;

		private readonly LinkInfo m_linkContentLocation;

		private readonly LinkInfo m_linkContentItem;

		private readonly string m_sContentItemDetail;

		private readonly string m_sLinkText;

		public LinkInfo ContentItem
		{
			get
			{
				return this.m_linkContentItem;
			}
		}

		public string ContentItemDetail
		{
			get
			{
				return this.m_sContentItemDetail;
			}
		}

		public LinkInfo ContentLocation
		{
			get
			{
				return this.m_linkContentLocation;
			}
		}

		public string LinkText
		{
			get
			{
				return this.m_sLinkText;
			}
		}

		public LinkInfo NewLink
		{
			get
			{
				return this.m_linkNew;
			}
		}

		public string OldLink
		{
			get
			{
				return this.m_sOldLink;
			}
		}

		public LinkCorrectedEventArgs(string sOldLink, LinkInfo linkNew, string sLinkText) : this(sOldLink, linkNew, sLinkText, null, null, null)
		{
		}

		public LinkCorrectedEventArgs(string sOldLink, LinkInfo linkNew, string sLinkText, LinkInfo linkContentLocation) : this(sOldLink, linkNew, sLinkText, linkContentLocation, null, null)
		{
		}

		public LinkCorrectedEventArgs(string sOldLink, LinkInfo linkNew, string sLinkText, LinkInfo linkContentLocation, LinkInfo linkContentItem) : this(sOldLink, linkNew, sLinkText, linkContentLocation, linkContentItem, null)
		{
		}

		public LinkCorrectedEventArgs(string sOldLink, LinkInfo linkNew, string sLinkText, LinkInfo linkContentLocation, LinkInfo linkContentItem, string sContentItemDetail)
		{
			this.m_sOldLink = sOldLink;
			this.m_linkNew = linkNew;
			this.m_linkContentLocation = linkContentLocation;
			this.m_linkContentItem = linkContentItem;
			this.m_sContentItemDetail = sContentItemDetail;
			this.m_sLinkText = sLinkText;
		}
	}
}