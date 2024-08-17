using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointAudiences")]
	public class CopyAudiencesCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteAudiencesAction);
			}
		}

		[Parameter(Mandatory=true, HelpMessage="Indicates what action the copy should take on audiences that already exist on the target.")]
		public CopyAudiencesOptions.PasteAudienceStyles ExistingAudiencesAction
		{
			get
			{
				return this.PasteAudiencesOptions.PasteStyle;
			}
			set
			{
				this.PasteAudiencesOptions.PasteStyle = value;
			}
		}

		[Parameter(HelpMessage="Forces a refresh of the source and target nodes prior to copying to ensure that all cached data is up to date.")]
		public SwitchParameter ForceRefresh
		{
			get
			{
				return this.PasteAudiencesOptions.ForceRefresh;
			}
			set
			{
				this.PasteAudiencesOptions.ForceRefresh = value;
			}
		}

		[Parameter(HelpMessage="Indicates that actions which have been skipped should not be logged at all.")]
		public SwitchParameter LogSkippedItems
		{
			get
			{
				return this.PasteAudiencesOptions.LogSkippedItems;
			}
			set
			{
				this.PasteAudiencesOptions.LogSkippedItems = value;
			}
		}

		protected virtual CopyAudiencesOptions PasteAudiencesOptions
		{
			get
			{
				return base.Action.Options as CopyAudiencesOptions;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to automatically trigger audience compilation after the copy.")]
		public SwitchParameter StartAudienceCompilation
		{
			get
			{
				return this.PasteAudiencesOptions.StartAudienceCompilation;
			}
			set
			{
				this.PasteAudiencesOptions.StartAudienceCompilation = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Enables verbose logging.")]
		public SwitchParameter VerboseLog
		{
			get
			{
				return this.PasteAudiencesOptions.Verbose;
			}
			set
			{
				this.PasteAudiencesOptions.Verbose = value;
			}
		}

		public CopyAudiencesCmdlet()
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