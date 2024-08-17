using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteListViewsOptions : SharePointActionOptions
	{
		private bool m_bOverwriteExistingViews;

		private bool m_bCopyViewWebParts = true;

		private bool m_bCopyClosedWebParts;

		private ExistingWebPartsProtocol m_ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;

		public bool CopyClosedWebParts
		{
			get
			{
				return this.m_bCopyClosedWebParts;
			}
			set
			{
				this.m_bCopyClosedWebParts = value;
			}
		}

		public bool CopyViewWebParts
		{
			get
			{
				return this.m_bCopyViewWebParts;
			}
			set
			{
				this.m_bCopyViewWebParts = value;
			}
		}

		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.m_ExistingWebPartsAction;
			}
			set
			{
				this.m_ExistingWebPartsAction = value;
			}
		}

		public bool OverwriteExistingViews
		{
			get
			{
				return this.m_bOverwriteExistingViews;
			}
			set
			{
				this.m_bOverwriteExistingViews = value;
			}
		}

		public PasteListViewsOptions()
		{
		}
	}
}