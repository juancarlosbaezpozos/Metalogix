using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointListViews")]
	public class CopyListViewsCmdlet : SharePointActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteListViewsAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy closed web parts on the view page.")]
		public SwitchParameter CopyClosedWebParts
		{
			get
			{
				return this.ListViewsOptions.CopyClosedWebParts;
			}
			set
			{
				this.ListViewsOptions.CopyClosedWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy the web parts on the view page.")]
		public SwitchParameter CopyViewWebParts
		{
			get
			{
				return this.ListViewsOptions.CopyViewWebParts;
			}
			set
			{
				this.ListViewsOptions.CopyViewWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates how the operation should deal with existing web parts on the target view page.")]
		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.ListViewsOptions.ExistingWebPartsAction;
			}
			set
			{
				this.ListViewsOptions.ExistingWebPartsAction = value;
			}
		}

		protected virtual PasteListViewsOptions ListViewsOptions
		{
			get
			{
				return base.Action.Options as PasteListViewsOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should overwrite existing views on the target.")]
		public SwitchParameter OverwriteExistingViews
		{
			get
			{
				return this.ListViewsOptions.OverwriteExistingViews;
			}
			set
			{
				this.ListViewsOptions.OverwriteExistingViews = value;
			}
		}

		public CopyListViewsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !base.Source.GetType().IsAssignableFrom(typeof(SPList)))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null || !base.Source.GetType().IsAssignableFrom(typeof(SPList)))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target of the copy is null, please initialize a proper target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}