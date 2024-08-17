using Metalogix.SharePoint.Actions.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "Permissions")]
	public class RemoveRoleAssignmentCmdlet : AddPermissionsCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(RemoveRoleAssignmentAction);
			}
		}

		public RemoveRoleAssignmentCmdlet()
		{
		}
	}
}