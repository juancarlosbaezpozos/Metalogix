using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointAllChildLists")]
	public class CopySiteChildListsCmdlet : CopyListCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteLists);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether content types should be copied or not.")]
		public SwitchParameter CopyContentTypes
		{
			get
			{
				return this.PasteSiteListsOptions.CopyContentTypes;
			}
			set
			{
				this.PasteSiteListsOptions.CopyContentTypes = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteSiteListsOptions PasteSiteListsOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteSiteListsOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a site field should be copied.")]
		public IFilterExpression SiteFieldsFilterExpression
		{
			get
			{
				return this.PasteSiteListsOptions.SiteFieldsFilterExpression;
			}
			set
			{
				this.PasteSiteListsOptions.SiteFieldsFilterExpression = value;
			}
		}

		public CopySiteChildListsCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}