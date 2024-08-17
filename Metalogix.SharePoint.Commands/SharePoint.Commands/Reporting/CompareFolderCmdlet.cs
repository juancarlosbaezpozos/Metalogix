using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Reporting;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Reporting
{
	[Cmdlet("Compare", "MLSharePointFolder")]
	public class CompareFolderCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CompareFolderAction);
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare folders.")]
		public SwitchParameter CompareFolders
		{
			get
			{
				return this.CompareListOptions.CompareFolders;
			}
			set
			{
				this.CompareListOptions.CompareFolders = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare items.")]
		public SwitchParameter CompareItems
		{
			get
			{
				return this.CompareListOptions.CompareItems;
			}
			set
			{
				this.CompareListOptions.CompareItems = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Reporting.CompareListOptions CompareListOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Reporting.CompareListOptions;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare object metadata.")]
		public SwitchParameter CompareMetadata
		{
			get
			{
				return this.CompareListOptions.CheckResults;
			}
			set
			{
				this.CompareListOptions.CheckResults = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare item versions.")]
		public SwitchParameter CompareVersions
		{
			get
			{
				return this.CompareListOptions.CompareVersions;
			}
			set
			{
				this.CompareListOptions.CompareVersions = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to filter list items and their versions.")]
		public SwitchParameter FilterItemsAndVersions
		{
			get
			{
				return this.CompareListOptions.FilterItems;
			}
			set
			{
				this.CompareListOptions.FilterItems = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to filter lists and folders.")]
		public SwitchParameter FilterListsAndFolders
		{
			get
			{
				return this.CompareListOptions.FilterLists;
			}
			set
			{
				this.CompareListOptions.FilterLists = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to halt the comparison if a difference is encountered.")]
		public SwitchParameter HaltIfDifferent
		{
			get
			{
				return this.CompareListOptions.HaltIfDifferent;
			}
			set
			{
				this.CompareListOptions.HaltIfDifferent = value;
			}
		}

		[Parameter(HelpMessage="The filter expression applied to list items and versions.")]
		public string ItemAndVersionFilterExpression
		{
			get
			{
				return this.CompareListOptions.ItemFilterExpression;
			}
			set
			{
				this.CompareListOptions.ItemFilterExpression = value;
			}
		}

		[Parameter(HelpMessage="The filter expression applied to lists and folders.")]
		public string ListAndFolderFilterExpression
		{
			get
			{
				return this.CompareListOptions.ListFilterExpression;
			}
			set
			{
				this.CompareListOptions.ListFilterExpression = value;
			}
		}

		[Parameter(HelpMessage="Indicates whether or not to compare object metadata.")]
		public SwitchParameter VerboseLog
		{
			get
			{
				return this.CompareListOptions.Verbose;
			}
			set
			{
				this.CompareListOptions.Verbose = value;
			}
		}

		public CompareFolderCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Source));
			}
			else if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target of the copy is null, please initialize a proper target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			this.CompareListOptions.Configured = true;
			return base.ProcessParameters();
		}
	}
}