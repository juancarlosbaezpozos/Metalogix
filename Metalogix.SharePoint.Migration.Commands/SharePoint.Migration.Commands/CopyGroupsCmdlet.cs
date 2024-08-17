using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;
using System.Xml;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointGroups")]
	public class CopyGroupsCmdlet : SharePointActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyGroupsAction);
			}
		}

		[Parameter(HelpMessage="Sets user writing operations to use a direct database write when the user is no longer available in Active Directory. Requires that your environment settings be configured to allow DB writing.")]
		public new SwitchParameter AllowDBUserWriting
		{
			get
			{
				return this.CopyGroupsOptions.AllowDBUserWriting;
			}
			set
			{
				this.CopyGroupsOptions.AllowDBUserWriting = value;
			}
		}

		public virtual Metalogix.SharePoint.Options.Migration.CopyGroupsOptions CopyGroupsOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.CopyGroupsOptions;
			}
		}

		[Parameter(HelpMessage="Forces a refresh of the source and target nodes prior to copying to ensure that all cached data is up to date.")]
		public new SwitchParameter ForceRefresh
		{
			get
			{
				return this.CopyGroupsOptions.ForceRefresh;
			}
			set
			{
				this.CopyGroupsOptions.ForceRefresh = value;
			}
		}

		[Parameter(HelpMessage="A table of groups or users within groups to exclude from the group copying operation.")]
		public XmlNode GroupExclusions
		{
			get
			{
				return this.CopyGroupsOptions.GroupExclusions;
			}
			set
			{
				this.CopyGroupsOptions.GroupExclusions = value;
			}
		}

		[Parameter(HelpMessage="Indicates if mapping of SharePoint groups should be done by name, rather than membership.")]
		public new SwitchParameter MapGroupsByName
		{
			get
			{
				return this.CopyGroupsOptions.MapGroupsByName;
			}
			set
			{
				this.CopyGroupsOptions.MapGroupsByName = value;
			}
		}

		[Parameter(HelpMessage="Indicates if groups with matching names should be overwritten. Note that this only applies when mapping is being done by name.")]
		public new SwitchParameter OverwriteGroups
		{
			get
			{
				return this.CopyGroupsOptions.OverwriteGroups;
			}
			set
			{
				this.CopyGroupsOptions.OverwriteGroups = value;
			}
		}

		public CopyGroupsCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}