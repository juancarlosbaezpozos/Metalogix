using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "MLSharePointSiteCollection")]
	public class RemoveSiteCollectionCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(DeleteSiteCollection);
			}
		}

		[Parameter(Mandatory=true, Position=1, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The parent Server object of the Site Collection, retrieve by using the Get-MLSharePointServer commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPBaseServer Server
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The Site Collection object retrieved by using the Get-MLSharePointSite commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPWeb SiteCollection
		{
			get;
			set;
		}

		public RemoveSiteCollectionCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters())
			{
				base.Target = this.Server.Sites.GetNodeByUrl(this.SiteCollection.Url);
				base.Source = null;
				if (base.Target == null || !string.Equals(base.Target.Url, this.SiteCollection.Url, StringComparison.CurrentCultureIgnoreCase))
				{
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The specified Site Collection could not be found in the specified Server."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
				}
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}