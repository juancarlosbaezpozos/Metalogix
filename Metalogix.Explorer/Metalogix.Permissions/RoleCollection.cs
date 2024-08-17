using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class RoleCollection : SerializableList<Role>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public virtual Role this[string sRoleName]
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

        public override Role this[Role key]
        {
            get { return this[key.RoleName]; }
        }

        public RoleCollection(Role[] roles)
        {
            if (roles != null)
            {
                base.AddRangeToCollection(roles);
            }
        }

        public RoleCollection()
        {
        }

        public RoleCollection(XmlNode xml)
        {
            this.FromXML(xml);
        }

        public virtual Role AddOrUpdateRole(Role role)
        {
            if (this[role.RoleName] != null)
            {
                base.RemoveFromCollection(this[role.RoleName]);
            }

            base.AddToCollection(role);
            return role;
        }

        public virtual bool DeleteRole(string sRoleName)
        {
            if (this[sRoleName] == null)
            {
                return false;
            }

            return base.RemoveFromCollection(this[sRoleName]);
        }

        public virtual Role MapRole(Role role)
        {
            Role role1 = RoleConverter.ConvertRole(role, this.CollectionType);
            if (role1 == null)
            {
                object[] fullName = new object[] { role.GetType().FullName, this.CollectionType.FullName };
                throw new ArgumentException(string.Format(
                    "Cannot map role: No conversion is defined between roles of type {0} and {1}", fullName));
            }

            float single = 0f;
            Role role2 = null;
            foreach (Role role3 in this)
            {
                float similarity = role1.GetSimilarity(role3);
                if (similarity <= single)
                {
                    continue;
                }

                single = similarity;
                role2 = role3;
            }

            return role2;
        }
    }
}