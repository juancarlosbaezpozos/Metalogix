using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.Permissions
{
    public class RoleAssignmentCollection : SerializableList<RoleAssignment>
    {
        protected ISecurableObject m_securableObject;

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return true; }
        }

        public override RoleAssignment this[RoleAssignment key]
        {
            get { return this.GetRoleAssignmentsByPrincipalName(key.Principal.PrincipalName); }
        }

        public int TotalAssignments
        {
            get
            {
                int count = 0;
                foreach (RoleAssignment roleAssignments in this)
                {
                    count += roleAssignments.Roles.Count;
                }

                return count;
            }
        }

        public RoleAssignmentCollection(XmlNode xml)
        {
            this.FromXML(xml);
        }

        public RoleAssignmentCollection(ISecurableObject securableObject, XmlNode node)
        {
            this.m_securableObject = securableObject;
            this.FromXML(node);
        }

        public virtual RoleAssignment AddRoleAssignment(SecurityPrincipal newPrincipal, Role newRole,
            bool bAllowDBWrite)
        {
            SecurityPrincipal securityPrincipal = this.m_securableObject.Principals.MapSecurityPrincipal(newPrincipal);
            Role role = this.m_securableObject.Roles.MapRole(newRole);
            RoleAssignment roleAssignmentsByPrincipalName =
                this.GetRoleAssignmentsByPrincipalName(securityPrincipal.PrincipalName);
            if (role != null)
            {
                if (roleAssignmentsByPrincipalName == null)
                {
                    if (securityPrincipal != null)
                    {
                        roleAssignmentsByPrincipalName = new RoleAssignment(securityPrincipal, role);
                        this.m_collection.Add(roleAssignmentsByPrincipalName);
                    }
                }
                else if (!roleAssignmentsByPrincipalName.Roles.Contains(role))
                {
                    roleAssignmentsByPrincipalName.Roles.Add(role);
                }
            }

            this.FireCollectionChanged(CollectionChangeAction.Add, roleAssignmentsByPrincipalName);
            return roleAssignmentsByPrincipalName;
        }

        public override void ClearCollection()
        {
            this.m_collection.Clear();
            this.FireCollectionChanged(CollectionChangeAction.Refresh, null);
        }

        protected void FireCollectionChanged(CollectionChangeAction action, RoleAssignment changedAssignment)
        {
            if (this.RoleAssignmentCollectionChanged != null)
            {
                this.RoleAssignmentCollectionChanged(this, new CollectionChangeEventArgs(action, changedAssignment));
            }
        }

        public override void FromXML(XmlNode xmlNode)
        {
            this.ClearCollection();
            SecurityPrincipalCollection principals = this.m_securableObject.Principals;
            RoleCollection roles = this.m_securableObject.Roles;
            if (xmlNode != null)
            {
                foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//RoleAssignment"))
                {
                    string value = xmlNodes.Attributes["PrincipalName"].Value;
                    string str = xmlNodes.Attributes["RoleName"].Value;
                    if (str.Contains("\\"))
                    {
                        str = str.Replace("\\", "%5C");
                    }

                    Role item = roles[str];
                    if (item == null)
                    {
                        continue;
                    }

                    RoleAssignment roleAssignmentsByPrincipalName = this.GetRoleAssignmentsByPrincipalName(value);
                    if (roleAssignmentsByPrincipalName != null)
                    {
                        roleAssignmentsByPrincipalName.Roles.Add(item);
                    }
                    else
                    {
                        SecurityPrincipal securityPrincipal = principals[value];
                        if (securityPrincipal == null)
                        {
                            continue;
                        }

                        base.AddToCollection(new RoleAssignment(securityPrincipal, item));
                    }
                }
            }
        }

        public RoleAssignment GetRoleAssignmentsByPrincipalName(string sPrincipalName)
        {
            RoleAssignment roleAssignments;
            using (IEnumerator<RoleAssignment> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    RoleAssignment current = enumerator.Current;
                    if (current.Principal.PrincipalName != sPrincipalName)
                    {
                        continue;
                    }

                    roleAssignments = current;
                    return roleAssignments;
                }

                return null;
            }

            return roleAssignments;
        }

        public virtual bool RemoveRoleAssignment(SecurityPrincipal principal, Role role)
        {
            bool flag = false;
            RoleAssignment roleAssignmentsByPrincipalName =
                this.GetRoleAssignmentsByPrincipalName(principal.PrincipalName);
            if (roleAssignmentsByPrincipalName != null)
            {
                if (role != null)
                {
                    Role item = roleAssignmentsByPrincipalName[role.RoleName];
                    if (item != null)
                    {
                        flag = roleAssignmentsByPrincipalName.Roles.Remove(item);
                        if (roleAssignmentsByPrincipalName.Roles.Count == 0)
                        {
                            this.m_collection.Remove(roleAssignmentsByPrincipalName);
                        }
                    }
                }
                else
                {
                    this.m_collection.Remove(roleAssignmentsByPrincipalName);
                    flag = true;
                }
            }

            this.FireCollectionChanged(CollectionChangeAction.Remove, roleAssignmentsByPrincipalName);
            return flag;
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("RoleAssignments");
            foreach (RoleAssignment roleAssignments in this)
            {
                foreach (Role role in roleAssignments.Roles)
                {
                    xmlWriter.WriteStartElement("RoleAssignment");
                    xmlWriter.WriteAttributeString("RoleName", role.RoleName);
                    xmlWriter.WriteAttributeString("PrincipalName", roleAssignments.Principal.PrincipalName);
                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
        }

        public event CollectionChangeEventHandler RoleAssignmentCollectionChanged;
    }
}