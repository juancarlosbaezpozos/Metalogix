using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointWebParts")]
	public class CopyWebPartsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteDefaultWebPartPageAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether closed web parts from the source are copied to the target or not.")]
		public SwitchParameter CopyClosedWebParts
		{
			get
			{
				return this.PasteWebPartOptions.CopyClosedWebParts;
			}
			set
			{
				this.PasteWebPartOptions.CopyClosedWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether content zone content should be updated on the target as part of the copy.")]
		public SwitchParameter CopyContentZoneContent
		{
			get
			{
				return this.PasteWebPartOptions.CopyContentZoneContent;
			}
			set
			{
				this.PasteWebPartOptions.CopyContentZoneContent = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether webparts should be copied recursively for each sub-site or not.")]
		public SwitchParameter CopyWebPartsRecursive
		{
			get
			{
				return this.PasteWebPartOptions.CopyWebPartsRecursive;
			}
			set
			{
				this.PasteWebPartOptions.CopyWebPartsRecursive = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates what action the copy should take on web parts that already exist on the target.")]
		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.PasteWebPartOptions.ExistingWebPartsAction;
			}
			set
			{
				this.PasteWebPartOptions.ExistingWebPartsAction = value;
			}
		}

		[Parameter(HelpMessage="Forces a refresh of the source and target nodes prior to copying to ensure that all cached data is up to date.")]
		public SwitchParameter ForceRefresh
		{
			get
			{
				return this.PasteWebPartOptions.ForceRefresh;
			}
			set
			{
				this.PasteWebPartOptions.ForceRefresh = value;
			}
		}

		[Parameter(HelpMessage="Enabled the mapping of audiences during a copy.")]
		public SwitchParameter MapAudiences
		{
			get
			{
				return this.PasteWebPartOptions.MapAudiences;
			}
			set
			{
				this.PasteWebPartOptions.MapAudiences = value;
			}
		}

		[Parameter(HelpMessage="Indicates if mapping of SharePoint groups should be done by name, rather than membership.")]
		public SwitchParameter MapGroupsByName
		{
			get
			{
				return this.PasteWebPartOptions.MapGroupsByName;
			}
			set
			{
				this.PasteWebPartOptions.MapGroupsByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Map all missing users to this Login Name.")]
		public string MapMissingUsersToLoginName
		{
			get
			{
				return this.PasteWebPartOptions.MapMissingUsersToLoginName;
			}
			set
			{
				this.PasteWebPartOptions.MapMissingUsers = !string.IsNullOrEmpty(value);
				this.PasteWebPartOptions.MapMissingUsersToLoginName = value;
			}
		}

		[Parameter(HelpMessage="Indicates if groups with matching names should be overwritten. Note that this only applies when mapping is being done by name.")]
		public SwitchParameter OverwriteGroups
		{
			get
			{
				return this.PasteWebPartOptions.OverwriteGroups;
			}
			set
			{
				this.PasteWebPartOptions.OverwriteGroups = value;
			}
		}

		protected virtual WebPartOptions PasteWebPartOptions
		{
			get
			{
				return base.Action.Options as WebPartOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a collection of transformation tasks, such as renaming, to be applied to the data as it is copied.")]
		public TransformationTaskCollection TaskCollection
		{
			get
			{
				return this.PasteWebPartOptions.TaskCollection;
			}
			set
			{
				this.PasteWebPartOptions.TaskCollection = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether link correction mapping generation is performed before the migration or as the migration occurs.")]
		public SwitchParameter UseComprehensiveLinkCorrection
		{
			get
			{
				return this.PasteWebPartOptions.UseComprehensiveLinkCorrection;
			}
			set
			{
				this.PasteWebPartOptions.UseComprehensiveLinkCorrection = value;
			}
		}

		public CopyWebPartsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target of the copy is null, please initialize a proper target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}
	}
}