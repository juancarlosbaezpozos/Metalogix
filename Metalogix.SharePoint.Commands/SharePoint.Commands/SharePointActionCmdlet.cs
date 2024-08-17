using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	public abstract class SharePointActionCmdlet : RemoteActionCmdlet
	{
		private string m_sLinkCorrectionScope;

		private string m_sComparisonLevel;

		[Parameter(HelpMessage="Sets user writing operations to use a direct database write when the user is no longer available in Active Directory. Requires that your environment settings be configured to allow DB writing.")]
		public SwitchParameter AllowDBUserWriting
		{
			get
			{
				return this.SharePointOptions.AllowDBUserWriting;
			}
			set
			{
				this.SharePointOptions.AllowDBUserWriting = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether the user would like to use the Metalogix Comparison tool to compare the source and target.\nIf chosen, differences will be outputted as warnings to the PowerShell console.\nAll results for an operation can be seen by enabling verbose display.")]
		public SwitchParameter CheckResults
		{
			get
			{
				return this.SharePointOptions.CheckResults;
			}
			set
			{
				this.SharePointOptions.CheckResults = value;
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

		[Parameter(HelpMessage="Indicates whether the user would like to employ automatic link correction to metadata fields within their copy.")]
		public SwitchParameter CorrectingLinks
		{
			get
			{
				return this.SharePointOptions.CorrectingLinks;
			}
			set
			{
				this.SharePointOptions.CorrectingLinks = value;
			}
		}

		[Parameter(HelpMessage="Forces a refresh of the source and target nodes prior to copying to ensure that all cached data is up to date.")]
		public SwitchParameter ForceRefresh
		{
			get
			{
				return this.SharePointOptions.ForceRefresh;
			}
			set
			{
				this.SharePointOptions.ForceRefresh = value;
			}
		}

		[Parameter(HelpMessage="Specifies the scope at which links are corrected. The current options are 'SiteCollection' and 'MigrationOnly' level scope.")]
		public string LinkCorrectionScope
		{
			get
			{
				return this.m_sLinkCorrectionScope;
			}
			set
			{
				this.m_sLinkCorrectionScope = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether the user would like to employ automatic HTML link correction in text fields of items/documents.")]
		public SwitchParameter LinkCorrectTextFields
		{
			get
			{
				return this.SharePointOptions.LinkCorrectTextFields;
			}
			set
			{
				this.SharePointOptions.LinkCorrectTextFields = value;
			}
		}

		[Parameter(HelpMessage="Indicates that actions which have been skipped should not be logged at all.")]
		public SwitchParameter LogSkippedItems
		{
			get
			{
				return this.SharePointOptions.LogSkippedItems;
			}
			set
			{
				this.SharePointOptions.LogSkippedItems = value;
			}
		}

		[Parameter(HelpMessage="Enabled the mapping of audiences during a copy.")]
		public SwitchParameter MapAudiences
		{
			get
			{
				return this.SharePointOptions.MapAudiences;
			}
			set
			{
				this.SharePointOptions.MapAudiences = value;
			}
		}

		[Parameter(HelpMessage="Indicates if mapping of SharePoint groups should be done by name, rather than membership.")]
		public SwitchParameter MapGroupsByName
		{
			get
			{
				return this.SharePointOptions.MapGroupsByName;
			}
			set
			{
				this.SharePointOptions.MapGroupsByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Map all missing users to this Login Name.")]
		public string MapMissingUsersToLoginName
		{
			get
			{
				return this.SharePointOptions.MapMissingUsersToLoginName;
			}
			set
			{
				this.SharePointOptions.MapMissingUsers = !string.IsNullOrEmpty(value);
				this.SharePointOptions.MapMissingUsersToLoginName = value;
			}
		}

		[Parameter(HelpMessage="Indicates the migration Mode")]
		public Metalogix.SharePoint.Options.MigrationMode MigrationMode
		{
			get
			{
				return this.SharePointOptions.MigrationMode;
			}
			set
			{
				this.SharePointOptions.MigrationMode = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the checkout status of a pre-existing target file will be overridden.")]
		public SwitchParameter OverrideCheckouts
		{
			get
			{
				return this.SharePointOptions.OverrideCheckouts;
			}
			set
			{
				this.SharePointOptions.OverrideCheckouts = value;
			}
		}

		[Parameter(HelpMessage="Indicates if groups with matching names should be overwritten. Note that this only applies when mapping is being done by name.")]
		public SwitchParameter OverwriteGroups
		{
			get
			{
				return this.SharePointOptions.OverwriteGroups;
			}
			set
			{
				this.SharePointOptions.OverwriteGroups = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="When set, any dynamically created link correction or GUID mappings will be persisted to the global mappings store. If the global mappings already contain an entry for a particular source GUID, it will be overwritten with the value dynamically generated by the action.")]
		public SwitchParameter PersistMappings
		{
			get
			{
				return this.SharePointOptions.PersistMappings;
			}
			set
			{
				this.SharePointOptions.PersistMappings = value;
			}
		}

		public virtual SharePointActionOptions SharePointOptions
		{
			get
			{
				return base.Action.Options as SharePointActionOptions;
			}
		}

		[Parameter(HelpMessage="Indicates whether link correction mapping generation is performed before the migration or as the migration occurs.")]
		public SwitchParameter UseComprehensiveLinkCorrection
		{
			get
			{
				return this.SharePointOptions.UseComprehensiveLinkCorrection;
			}
			set
			{
				this.SharePointOptions.UseComprehensiveLinkCorrection = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Enables verbose logging.")]
		public SwitchParameter VerboseLog
		{
			get
			{
				return this.SharePointOptions.Verbose;
			}
			set
			{
				this.SharePointOptions.Verbose = value;
			}
		}

		protected SharePointActionCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			base.ProcessParameters();
			if (this.ComparisonLevel != null)
			{
				string upper = this.ComparisonLevel.ToUpper();
				string str = upper;
				if (upper != null)
				{
					if (str == "MODERATE")
					{
						this.SharePointOptions.CompareOptions.Level = CompareLevel.Moderate;
					}
					else if (str == "STRICT")
					{
						this.SharePointOptions.CompareOptions.Level = CompareLevel.Strict;
					}
				}
			}
			if (!string.IsNullOrEmpty(this.LinkCorrectionScope))
			{
				string upper1 = this.LinkCorrectionScope.ToUpper();
				string str1 = upper1;
				if (upper1 != null)
				{
					if (str1 == "SITECOLLECTION")
					{
						this.SharePointOptions.LinkCorrectionScope = Metalogix.SharePoint.Migration.LinkCorrectionScope.SiteCollection;
					}
					else if (str1 == "MIGRATIONONLY")
					{
						this.SharePointOptions.LinkCorrectionScope = Metalogix.SharePoint.Migration.LinkCorrectionScope.MigrationOnly;
					}
				}
			}
			return true;
		}
	}
}