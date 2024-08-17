using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointPermissions")]
	public class CopyPermissionsCmdlet : ActionCmdlet
	{
		private string m_sComparisonLevel;

		protected override Type ActionType
		{
			get
			{
				return typeof(CopyPermissionsAction);
			}
		}

		[Parameter(HelpMessage="Sets user writing operations to use a direct database write when the user is no longer available in Active Directory. Requires that your environment settings be configured to allow DB writing.")]
		public SwitchParameter AllowDBUserWriting
		{
			get
			{
				return this.CopyPermissionsOptions.AllowDBUserWriting;
			}
			set
			{
				this.CopyPermissionsOptions.AllowDBUserWriting = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether the user would like to use the Metalogix Comparison tool to compare the source and target.\nIf chosen, differences will be outputted as warnings to the PowerShell console.\nAll results for an operation can be seen by enabling verbose display.")]
		public SwitchParameter CheckResults
		{
			get
			{
				return this.CopyPermissionsOptions.CheckResults;
			}
			set
			{
				this.CopyPermissionsOptions.CheckResults = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that permissions that previously existed on the source should be deleted. This options should be set except in the case of merging permissions from distinct sources.")]
		public SwitchParameter ClearRoleAssignments
		{
			get
			{
				return this.CopyPermissionsOptions.ClearRoleAssignments;
			}
			set
			{
				this.CopyPermissionsOptions.ClearRoleAssignments = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether a strict or moderate comparison is desired. Note that this has no effect if CheckResults is false.")]
		public string ComparisonLevel
		{
			get
			{
				return this.m_sComparisonLevel;
			}
			set
			{
				this.m_sComparisonLevel = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.CopyPermissionsOptions CopyPermissionsOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.CopyPermissionsOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the permissions for the top-level node specified should be migrated even if that object inherits its permissions.")]
		public SwitchParameter CopyRootPermissions
		{
			get
			{
				return this.CopyPermissionsOptions.CopyRootPermissions;
			}
			set
			{
				this.CopyPermissionsOptions.CopyRootPermissions = value;
			}
		}

		[Parameter(HelpMessage="Forces a refresh of the source and target nodes prior to copying to ensure that all cached data is up to date.")]
		public SwitchParameter ForceRefresh
		{
			get
			{
				return this.CopyPermissionsOptions.ForceRefresh;
			}
			set
			{
				this.CopyPermissionsOptions.ForceRefresh = value;
			}
		}

		[Parameter(HelpMessage="Indicates that actions which have been skipped should not be logged at all.")]
		public SwitchParameter LogSkippedItems
		{
			get
			{
				return this.CopyPermissionsOptions.LogSkippedItems;
			}
			set
			{
				this.CopyPermissionsOptions.LogSkippedItems = value;
			}
		}

		[Parameter(HelpMessage="Indicates if mapping of SharePoint groups should be done by name, rather than membership.")]
		public SwitchParameter MapGroupsByName
		{
			get
			{
				return this.CopyPermissionsOptions.MapGroupsByName;
			}
			set
			{
				this.CopyPermissionsOptions.MapGroupsByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Map all missing users to this Login Name.")]
		public string MapMissingUsersToLoginName
		{
			get
			{
				return this.CopyPermissionsOptions.MapMissingUsersToLoginName;
			}
			set
			{
				this.CopyPermissionsOptions.MapMissingUsers = !string.IsNullOrEmpty(value);
				this.CopyPermissionsOptions.MapMissingUsersToLoginName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should map permission levels by name rather than by mapping to the level with the set of enabled rights most similar to the source.")]
		public SwitchParameter MapRolesByName
		{
			get
			{
				return this.CopyPermissionsOptions.MapRolesByName;
			}
			set
			{
				this.CopyPermissionsOptions.MapRolesByName = value;
			}
		}

		[Parameter(HelpMessage="Indicates if groups with matching names should be overwritten. Note that this only applies when mapping is being done by name.")]
		public SwitchParameter OverwriteGroups
		{
			get
			{
				return this.CopyPermissionsOptions.OverwriteGroups;
			}
			set
			{
				this.CopyPermissionsOptions.OverwriteGroups = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="When set, any dynamically created link correction or GUID mappings will be persisted to the global mappings store. If the global mappings already contain an entry for a particular source GUID, it will be overwritten with the value dynamically generated by the action.")]
		public SwitchParameter PersistMappings
		{
			get
			{
				return this.CopyPermissionsOptions.PersistMappings;
			}
			set
			{
				this.CopyPermissionsOptions.PersistMappings = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a set of custom mappings to use for permission levels.")]
		public ConditionalMappingCollection RoleAssignmentMappings
		{
			get
			{
				return this.CopyPermissionsOptions.RoleAssignmentMappings;
			}
			set
			{
				this.CopyPermissionsOptions.RoleAssignmentMappings = value;
				this.CopyPermissionsOptions.OverrideRoleMappings = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Enables verbose logging.")]
		public SwitchParameter VerboseLog
		{
			get
			{
				return this.CopyPermissionsOptions.Verbose;
			}
			set
			{
				this.CopyPermissionsOptions.Verbose = value;
			}
		}

		public CopyPermissionsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			bool flag = base.ProcessParameters();
			if (flag && this.ComparisonLevel != null)
			{
				string upper = this.ComparisonLevel.ToUpper();
				string str = upper;
				if (upper != null)
				{
					if (str == "MODERATE")
					{
						this.CopyPermissionsOptions.CompareOptions.Level = CompareLevel.Moderate;
					}
					else if (str == "STRICT")
					{
						this.CopyPermissionsOptions.CompareOptions.Level = CompareLevel.Strict;
					}
				}
			}
			return flag;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}