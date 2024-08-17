using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteSiteContentOptions : PasteSiteOptions
	{
		[CmdletEnabledParameter(false)]
		public new string WebTemplateName
		{
			get
			{
				return base.WebTemplateName;
			}
			set
			{
				base.WebTemplateName = value;
			}
		}

		public PasteSiteContentOptions()
		{
		}
	}
}