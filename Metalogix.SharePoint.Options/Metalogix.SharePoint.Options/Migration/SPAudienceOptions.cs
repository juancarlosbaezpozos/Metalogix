using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPAudienceOptions : OptionsBase
	{
		private CopyAudiencesOptions.PasteAudienceStyles m_pasteStyle = CopyAudiencesOptions.PasteAudienceStyles.PreserveExisting;

		private bool m_bStartAudienceCompilation;

		public CopyAudiencesOptions.PasteAudienceStyles PasteStyle
		{
			get
			{
				return this.m_pasteStyle;
			}
			set
			{
				this.m_pasteStyle = value;
			}
		}

		public bool StartAudienceCompilation
		{
			get
			{
				return this.m_bStartAudienceCompilation;
			}
			set
			{
				this.m_bStartAudienceCompilation = value;
			}
		}

		public SPAudienceOptions()
		{
		}
	}
}