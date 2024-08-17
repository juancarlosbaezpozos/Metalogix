using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLAllListItems")]
	public class CopyAllListItemsCmdlet : CopyFolderCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteAllListItemsAction);
			}
		}

		protected override Metalogix.SharePoint.Options.Migration.PasteFolderOptions PasteFolderOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteFolderOptions;
			}
		}

		protected override PasteListItemOptions PasteItemOptions
		{
			get
			{
				return base.Action.Options as PasteListItemOptions;
			}
		}

		public CopyAllListItemsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !(base.Source is SPFolder))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source node isn't in the proper format. Please try again with a source that is a list or a folder."), "ArgumentError", ErrorCategory.InvalidArgument, base.Source));
			}
			if (base.Target == null || !(base.Target is SPFolder))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target node isn't in the proper format. Please try again with a target that is a list or a folder."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			SPList parentList = (base.Source as SPFolder).ParentList;
			SPList sPList = (base.Target as SPFolder).ParentList;
			if ((parentList.BaseType != ListType.DiscussionForum ? parentList.BaseType != sPList.BaseType : parentList.BaseTemplate != sPList.BaseTemplate))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The base types of the source and target do not match. You cannot copy list items between lists with different base types. Please choose a different source and target."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}