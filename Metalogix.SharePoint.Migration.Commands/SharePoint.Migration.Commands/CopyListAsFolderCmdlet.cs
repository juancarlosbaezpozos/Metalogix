using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointListAsFolder")]
	public class CopyListAsFolderCmdlet : CopyFolderCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteListAsFolderAction);
			}
		}

		public CopyListAsFolderCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !(base.Source is SPList))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null || !(base.Target is SPList) && !(base.Target is SPFolder))
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