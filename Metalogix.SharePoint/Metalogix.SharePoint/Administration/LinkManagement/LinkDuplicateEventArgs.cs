using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkDuplicateEventArgs : EventArgs
	{
		private readonly string m_sUriSource;

		private readonly LinkInfo m_linkDestinationLoaded;

		private readonly LinkInfo m_linkDestinationDuplicate;

		public LinkInfo LinkDuplicate
		{
			get
			{
				return this.m_linkDestinationDuplicate;
			}
		}

		public LinkInfo LinkLoaded
		{
			get
			{
				return this.m_linkDestinationLoaded;
			}
		}

		public string SourceUri
		{
			get
			{
				return this.m_sUriSource;
			}
		}

		public LinkDuplicateEventArgs(string sUriSource, LinkInfo linkDestinationLoaded, LinkInfo linkDestinationDuplicate)
		{
			this.m_sUriSource = sUriSource;
			this.m_linkDestinationLoaded = linkDestinationLoaded;
			this.m_linkDestinationDuplicate = linkDestinationDuplicate;
		}
	}
}