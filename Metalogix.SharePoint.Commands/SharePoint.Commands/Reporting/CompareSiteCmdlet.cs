using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Options.Reporting;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Reporting
{
	[Cmdlet("Compare", "MLSharePointSite")]
	public class CompareSiteCmdlet : CompareListCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CompareSiteAction);
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare lists.")]
		public SwitchParameter CompareLists
		{
			get
			{
				return this.CompareSiteOptions.CompareLists;
			}
			set
			{
				this.CompareSiteOptions.CompareLists = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Reporting.CompareSiteOptions CompareSiteOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Reporting.CompareSiteOptions;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare sub sites.")]
		public SwitchParameter CompareSubSites
		{
			get
			{
				return this.CompareSiteOptions.Recursive;
			}
			set
			{
				this.CompareSiteOptions.Recursive = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to filter sub sites.")]
		public SwitchParameter FilterSubSites
		{
			get
			{
				return this.CompareSiteOptions.FilterSites;
			}
			set
			{
				this.CompareSiteOptions.FilterSites = value;
			}
		}

		[Parameter(HelpMessage="The filter expression applied to sub sites.")]
		public string SubSiteFilterExpression
		{
			get
			{
				return this.CompareSiteOptions.SiteFilterExpression;
			}
			set
			{
				this.CompareSiteOptions.SiteFilterExpression = value;
			}
		}

		public CompareSiteCmdlet()
		{
		}
	}
}