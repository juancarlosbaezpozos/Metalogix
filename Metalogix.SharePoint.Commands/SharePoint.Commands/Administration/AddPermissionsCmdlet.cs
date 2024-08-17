using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Add", "Permissions")]
	public class AddPermissionsCmdlet : ActionCmdlet
	{
		private string m_sPrincipalName;

		private string m_sRoleName;

		private bool m_bRecursive;

		private RoleAssignmentCollection m_sourceRACollection;

		protected override Type ActionType
		{
			get
			{
				return typeof(AddRoleAssignmentsAction);
			}
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The name of the user or group you wish to assign a permission level to.")]
		public string Name
		{
			get
			{
				return this.m_sPrincipalName;
			}
			set
			{
				this.m_sPrincipalName = value;
			}
		}

		[Parameter(Mandatory=true, Position=1, HelpMessage="The name of the permission level you want to grant the specified user or group.")]
		public string PermissionLevel
		{
			get
			{
				return this.m_sRoleName;
			}
			set
			{
				this.m_sRoleName = value;
			}
		}

		[Parameter(HelpMessage="A flag that indicates the given permissions level should be granted to the given user for each item below the target which has unique permissions.")]
		public SwitchParameter Recurse
		{
			get
			{
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		protected override IXMLAbleList SourceCollection
		{
			get
			{
				return this.m_sourceRACollection;
			}
		}

		protected override IXMLAbleList TargetCollection
		{
			get
			{
				if (base.SourceCollection.Count != 0)
				{
					return base.SourceCollection;
				}
				return base.TargetCollection;
			}
		}

		public AddPermissionsCmdlet()
		{
		}

		private void ModifyUniquePermissions(Node node)
		{
			PropertyInfo property = node.GetType().GetProperty("HasUniquePermissions", typeof(bool));
			bool value = false;
			if (property != null)
			{
				value = (bool)property.GetValue(node, null);
			}
			if (value)
			{
				Metalogix.Actions.Action action = base.Action;
				IXMLAbleList sourceCollection = this.SourceCollection;
				Node[] nodeArray = new Node[] { node };
				action.Run(sourceCollection, new NodeCollection(nodeArray));
			}
			this.ModifyUniquePermissionsChildren(node);
		}

		private void ModifyUniquePermissionsChildren(Node node)
		{
			foreach (Node child in node.Children)
			{
				this.ModifyUniquePermissions(child);
			}
			if (typeof(SPFolder).IsAssignableFrom(node.GetType()))
			{
				foreach (SPListItem item in ((SPFolder)node).GetItems(false, ListItemQueryType.ListItem, null))
				{
					this.ModifyUniquePermissions(item);
				}
			}
		}

		protected override bool ProcessParameters()
		{
			ISecurableObject target = base.Target as ISecurableObject;
			if (target == null)
			{
				base.WriteVerbose("Target is not securable");
				return false;
			}
			SecurityPrincipal item = target.Principals[this.Name];
			if (item == null)
			{
				base.WriteVerbose("Principal name cannot be found");
				return false;
			}
			Role role = target.Roles[this.PermissionLevel];
			if (role == null)
			{
				base.WriteVerbose("Permission level name cannot be found");
				return false;
			}
			this.m_sourceRACollection = new RoleAssignmentCollection(base.Target as ISecurableObject, null);
			this.m_sourceRACollection.AddRoleAssignment(item, role, false);
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}

		protected override void Run()
		{
			if (!this.Recurse)
			{
				base.Run();
			}
			else
			{
				foreach (Node targetCollection in this.TargetCollection)
				{
					Metalogix.Actions.Action action = base.Action;
					IXMLAbleList sourceCollection = this.SourceCollection;
					Node[] nodeArray = new Node[] { targetCollection };
					action.Run(sourceCollection, new NodeCollection(nodeArray));
					this.ModifyUniquePermissionsChildren(targetCollection);
				}
			}
		}
	}
}