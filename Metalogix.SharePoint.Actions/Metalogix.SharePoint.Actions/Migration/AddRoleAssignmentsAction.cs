using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCBlogsAndWikis)]
	[Name("Add Role Assignments")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.Zero)]
	[SourceType(typeof(RoleAssignment))]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(ISecurableObject))]
	public class AddRoleAssignmentsAction : PasteAction<SharePointActionOptions>
	{
		public AddRoleAssignmentsAction()
		{
		}

		public void AddRoleAssignment(RoleAssignment roleAssignment, ISecurableObject target)
		{
			SecurityPrincipal securityPrincipal = null;
			if (base.CheckForAbort())
			{
				return;
			}
			try
			{
				securityPrincipal = target.Principals.MapSecurityPrincipal(roleAssignment.Principal);
				if (securityPrincipal == null)
				{
					LogItem logItem = new LogItem("Adding role assignments", roleAssignment.Principal.PrincipalName, roleAssignment.Principal.PrincipalName, "", ActionOperationStatus.Skipped)
					{
						SourceContent = roleAssignment.Principal.XML,
						Information = "Could not find principal on target"
					};
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem1 = new LogItem("Adding role assignments", roleAssignment.Principal.PrincipalName, roleAssignment.Principal.PrincipalName, "", ActionOperationStatus.Failed)
				{
					Exception = exception,
					SourceContent = roleAssignment.Principal.XML,
					Information = string.Concat("Could not find principal on target: ", exception.Message),
					Details = exception.StackTrace
				};
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
			}
			if (securityPrincipal == null)
			{
				return;
			}
			foreach (Role role in roleAssignment.Roles)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				LogItem stackTrace = null;
				string str = "";
				try
				{
					try
					{
						str = role.ToString();
						Role role1 = target.Roles.MapRole(role);
						stackTrace = new LogItem("Adding role assignment", role1.RoleName, role.RoleName, securityPrincipal.PrincipalName, ActionOperationStatus.Running);
						if (!typeof(SPRole).IsAssignableFrom(role1.GetType()) || !((SPRole)role1).Hidden)
						{
							base.FireOperationStarted(stackTrace);
							target.RoleAssignments.AddRoleAssignment(securityPrincipal, role1, base.SharePointOptions.AllowDBUserWriting);
							stackTrace.Status = ActionOperationStatus.Completed;
						}
						else
						{
							stackTrace.Status = ActionOperationStatus.Skipped;
							stackTrace.Information = "Role is hidden. Hidden roles such as Limited Access are automatically assigned by SharePoint when necessary";
						}
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						stackTrace.Exception = exception2;
						stackTrace.SourceContent = str;
						stackTrace.Status = ActionOperationStatus.Failed;
						string[] roleName = new string[] { "Could not assign ", role.RoleName, " to ", securityPrincipal.PrincipalName, ": ", exception2.Message };
						stackTrace.Information = string.Concat(roleName);
						stackTrace.Details = exception2.StackTrace;
					}
				}
				finally
				{
					if (stackTrace.Status != ActionOperationStatus.Skipped)
					{
						base.FireOperationFinished(stackTrace);
					}
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (ISecurableObject securableObject in target)
			{
				foreach (RoleAssignment roleAssignments in source)
				{
					this.AddRoleAssignment(roleAssignments, securableObject);
				}
			}
		}
	}
}