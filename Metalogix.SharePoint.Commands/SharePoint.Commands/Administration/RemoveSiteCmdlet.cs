using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "MLSharePointSite")]
	public class RemoveSiteCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(DeleteSite);
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The site object retrieved by using the Get-MLSharePointSite commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPWeb Site
		{
			get;
			set;
		}

		public RemoveSiteCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters())
			{
				if (this.Site.IsRootWeb)
				{
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The Site object provided is a Site Collection, please use the Remove-MLSharePointSiteCollection commandlet to delete this object."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
				}
				base.Target = this.Site;
				base.Source = null;
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}