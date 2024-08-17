using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPRoleAssignmentCollection : RoleAssignmentCollection
	{
		public SPRoleAssignmentCollection(ISecurableObject securableObject, XmlNode node) : base(securableObject, node)
		{
		}

		public override void Add(RoleAssignment item)
		{
			foreach (Role role in item.Roles)
			{
				this.AddRoleAssignment(item.Principal, role, false);
			}
		}

		public override RoleAssignment AddRoleAssignment(SecurityPrincipal newPrincipal, Role newRole, bool bAllowDBWrite)
		{
			SecurityPrincipal securityPrincipal;
			RoleAssignment roleAssignments;
			Type type = this.m_securableObject.GetType();
			if (!typeof(SPNode).IsAssignableFrom(type))
			{
				throw new Exception("The securable object attached to this collection is not a SharePoint securable object");
			}
			SPNode mSecurableObject = (SPNode)this.m_securableObject;
			if ((mSecurableObject.WriteVirtually ? false : mSecurableObject.Adapter.Writer == null))
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			string d = null;
			int itemID = -1;
			SPWeb parentWeb = null;
			if (typeof(SPWeb).IsAssignableFrom(type))
			{
				parentWeb = (SPWeb)mSecurableObject;
			}
			else if (typeof(SPFolder).IsAssignableFrom(type))
			{
				d = ((SPFolder)mSecurableObject).ParentList.ID;
				itemID = ((SPFolder)mSecurableObject).ItemID;
				parentWeb = ((SPFolder)mSecurableObject).ParentList.ParentWeb;
			}
			else if (typeof(SPListItem).IsAssignableFrom(type))
			{
				d = ((SPListItem)mSecurableObject).ParentList.ID;
				itemID = ((SPListItem)mSecurableObject).ID;
				parentWeb = ((SPListItem)mSecurableObject).ParentList.ParentWeb;
			}
			if (parentWeb == null)
			{
				roleAssignments = null;
			}
			else
			{
				string principalName = newPrincipal.PrincipalName;
				bool flag = newPrincipal is SPGroup;
				Role item = parentWeb.GetRoles(false)[newRole.RoleName];
				if (item == null)
				{
					throw new Exception("The given role does not exist on this web.");
				}
				if (!flag)
				{
					SPUserCollection siteUsers = parentWeb.GetSiteUsers(false);
					SPUser byLoginName = siteUsers.GetByLoginName(principalName);
					if ((byLoginName != null ? false : newPrincipal is SPUser))
					{
						byLoginName = siteUsers.AddUser((SPUser)newPrincipal, bAllowDBWrite);
						if (byLoginName == null)
						{
							throw new Exception("The given user could not be added to this web.");
						}
					}
					securityPrincipal = byLoginName;
				}
				else
				{
					SPGroup sPGroup = (SPGroup)parentWeb.GetGroups(false)[principalName];
					if (sPGroup == null)
					{
						throw new Exception("The given group does not exist on this web.");
					}
					securityPrincipal = sPGroup;
				}
				string xML = null;
				string str = null;
				if (!mSecurableObject.WriteVirtually)
				{
					str = parentWeb.Adapter.Writer.AddRoleAssignment(principalName, flag, item.RoleName, d, itemID);
				}
				else
				{
					xML = base.ToXML();
					str = string.Format("<RoleAssignment RoleName=\"{0}\" PrincipalName=\"{1}\" />", item.RoleName, principalName);
				}
				RoleAssignment roleAssignmentsByPrincipalName = base.GetRoleAssignmentsByPrincipalName(securityPrincipal.PrincipalName);
				if (item != null)
				{
					if (roleAssignmentsByPrincipalName == null)
					{
						if (securityPrincipal != null)
						{
							roleAssignmentsByPrincipalName = new RoleAssignment(securityPrincipal, item);
							this.m_collection.Add(roleAssignmentsByPrincipalName);
						}
					}
					else if (!roleAssignmentsByPrincipalName.Roles.Contains(item))
					{
						roleAssignmentsByPrincipalName.Roles.Add(item);
					}
				}
				if (mSecurableObject.WriteVirtually)
				{
					string xML1 = base.ToXML();
					mSecurableObject.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(xML1), "RoleAssignments");
				}
				base.FireCollectionChanged(CollectionChangeAction.Add, roleAssignmentsByPrincipalName);
				roleAssignments = roleAssignmentsByPrincipalName;
			}
			return roleAssignments;
		}

		public override bool Remove(RoleAssignment item)
		{
			bool flag;
			foreach (Role role in new CommonSerializableSet<Role>((CommonSerializableSet<Role>)item.Roles))
			{
				if (!this.RemoveRoleAssignment(item.Principal, role))
				{
					flag = false;
					return flag;
				}
			}
			flag = true;
			return flag;
		}

		public override bool RemoveRoleAssignment(SecurityPrincipal principal, Role role)
		{
			string roleName;
			Type type = this.m_securableObject.GetType();
			if (!typeof(SPNode).IsAssignableFrom(type))
			{
				throw new Exception("The securable object attached to this collection is not a SharePoint securable object");
			}
			SPNode mSecurableObject = (SPNode)this.m_securableObject;
			if ((mSecurableObject.WriteVirtually ? false : mSecurableObject.Adapter.Writer == null))
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			string d = null;
			int itemID = -1;
			SPWeb parentWeb = null;
			if (typeof(SPWeb).IsAssignableFrom(type))
			{
				parentWeb = (SPWeb)mSecurableObject;
			}
			else if (typeof(SPFolder).IsAssignableFrom(type))
			{
				d = ((SPFolder)mSecurableObject).ParentList.ID;
				itemID = ((SPFolder)mSecurableObject).ItemID;
				parentWeb = ((SPFolder)mSecurableObject).ParentList.ParentWeb;
			}
			else if (typeof(SPListItem).IsAssignableFrom(type))
			{
				d = ((SPListItem)mSecurableObject).ParentList.ID;
				itemID = ((SPListItem)mSecurableObject).ID;
				parentWeb = ((SPListItem)mSecurableObject).ParentList.ParentWeb;
			}
			bool flag = false;
			if (parentWeb != null)
			{
				bool flag1 = principal is SPGroup;
				if (!mSecurableObject.WriteVirtually)
				{
					ISharePointWriter writer = parentWeb.Adapter.Writer;
					string principalName = principal.PrincipalName;
					bool flag2 = flag1;
					if (role != null)
					{
						roleName = role.RoleName;
					}
					else
					{
						roleName = null;
					}
					writer.DeleteRoleAssignment(principalName, flag2, roleName, d, itemID);
					flag = base.RemoveRoleAssignment(principal, role);
				}
				else
				{
					string xML = base.ToXML();
					flag = base.RemoveRoleAssignment(principal, role);
					if (flag)
					{
						string str = base.ToXML();
						mSecurableObject.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(str), "RoleAssignments");
					}
				}
			}
			return flag;
		}
	}
}