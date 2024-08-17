using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPWebPartOptions : OptionsBase
	{
		private bool m_bCopySiteWebParts = true;

		private bool m_bCopyDocumentWebParts = true;

		private bool m_bCopyViewWebParts;

		private bool m_bCopyFormWebParts;

		private bool m_bCopyClosedWebParts;

		private bool m_bCopyContentZoneContent = true;

		private bool m_bCopyWebPartsRecursive;

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

		public bool CopyContentZoneContent
		{
			get
			{
				return this.m_bCopyContentZoneContent;
			}
			set
			{
				this.m_bCopyContentZoneContent = value;
			}
		}

		public bool CopyDocumentWebParts
		{
			get
			{
				return this.m_bCopyDocumentWebParts;
			}
			set
			{
				this.m_bCopyDocumentWebParts = value;
			}
		}

		public bool CopyFormWebParts
		{
			get
			{
				return this.m_bCopyFormWebParts;
			}
			set
			{
				this.m_bCopyFormWebParts = value;
			}
		}

		public bool CopySiteWebParts
		{
			get
			{
				return this.m_bCopySiteWebParts;
			}
			set
			{
				this.m_bCopySiteWebParts = value;
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

		public bool CopyWebPartsRecursive
		{
			get
			{
				return this.m_bCopyWebPartsRecursive;
			}
			set
			{
				this.m_bCopyWebPartsRecursive = value;
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

		public SPWebPartOptions()
		{
		}
	}
}