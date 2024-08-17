using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public class MappableRoleCollection : RoleCollection
    {
        public override Role this[string sRoleName]
        {
            get
            {
                Role role;
                using (IEnumerator<Role> enumerator = base.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Role current = enumerator.Current;
                        if (current.RoleName != sRoleName)
                        {
                            continue;
                        }

                        role = current;
                        return role;
                    }

                    return null;
                }

                return role;
            }
        }

        public MappableRoleCollection()
        {
        }

        public MappableRoleCollection(MappableRole[] roles) : base(roles)
        {
        }

        public MappableRoleCollection(XmlNode xml) : base(xml)
        {
        }

        public override Role AddOrUpdateRole(Role role)
        {
            if (this[role.RoleName] != null)
            {
                base.RemoveFromCollection(this[role.RoleName]);
            }

            base.AddToCollection(role);
            return role;
        }

        public override bool DeleteRole(string sRoleName)
        {
            if (this[sRoleName] == null)
            {
                return false;
            }

            return base.RemoveFromCollection(this[sRoleName]);
        }
    }
}