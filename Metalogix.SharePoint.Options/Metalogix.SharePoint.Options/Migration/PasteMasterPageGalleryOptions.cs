using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteMasterPageGalleryOptions : PasteListOptions
	{
		private bool m_bCopyMasterPages = true;

		private bool m_bCopyPageLayouts = true;

		private bool m_bCopyOtherResources = true;

		private bool m_bCorrectLinks;

		public bool CopyMasterPages
		{
			get
			{
				return this.m_bCopyMasterPages;
			}
			set
			{
				this.m_bCopyMasterPages = value;
			}
		}

		public bool CopyOtherResources
		{
			get
			{
				return this.m_bCopyOtherResources;
			}
			set
			{
				this.m_bCopyOtherResources = value;
			}
		}

		public bool CopyPageLayouts
		{
			get
			{
				return this.m_bCopyPageLayouts;
			}
			set
			{
				this.m_bCopyPageLayouts = value;
			}
		}

		public bool CorrectMasterPageLinks
		{
			get
			{
				return this.m_bCorrectLinks;
			}
			set
			{
				this.m_bCorrectLinks = value;
			}
		}

		public PasteMasterPageGalleryOptions()
		{
		}
	}
}