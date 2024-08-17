using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointSite")]
	public class CopySiteCmdlet : CopySiteContentCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines the name of a new web template for the site being copied.")]
		public string WebTemplateName
		{
			get
			{
				if (!this.PasteSiteOptions.ChangeWebTemplate)
				{
					return null;
				}
				return this.PasteSiteOptions.WebTemplateName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PasteSiteOptions.ChangeWebTemplate = true;
					this.PasteSiteOptions.WebTemplateName = value;
				}
			}
		}

		public CopySiteCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (!base.ProcessParameters())
			{
				return false;
			}
			if (base.Source is SPWeb && base.Target is SPWeb)
			{
				if (this.PasteSiteOptions.WebTemplateName == null)
				{
					if ((base.Target as SPWeb).Templates[(base.Source as SPWeb).Template.Name] == null)
					{
						base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The web template: ", (base.Source as SPWeb).Template.Name, " was not found on the target site. Please select a different template to map to.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
					}
				}
				else if ((base.Target as SPWeb).Templates[this.PasteSiteOptions.WebTemplateName] == null)
				{
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The web template: ", this.PasteSiteOptions.WebTemplateName, " was not found on the target site. Please select a different template to map to.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
				}
			}
			return true;
		}
	}
}