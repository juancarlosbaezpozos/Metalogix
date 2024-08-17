using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointAllSubsites")]
	public class CopySubSitesCmdlet : CopySiteContentCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteAllSubSitesAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines the name of a web template for the site being copied.")]
		public string WebTemplateName
		{
			get
			{
				return this.PasteSiteOptions.WebTemplateName;
			}
			set
			{
				this.PasteSiteOptions.WebTemplateName = value;
			}
		}

		public CopySubSitesCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}