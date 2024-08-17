using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class Role : Member
    {
        public abstract string[] AvailableRights { get; }

        public abstract string[] Rights { get; }

        public abstract string RoleName { get; }

        public Role() : base(null)
        {
        }

        public Role(XmlNode xml) : base(xml)
        {
        }

        public bool ContainsRight(string sRightName)
        {
            string[] rights = this.Rights;
            for (int i = 0; i < (int)rights.Length; i++)
            {
                if (rights[i] == sRightName)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual float GetSimilarity(Role targetRole)
        {
            if (base.GetType() != targetRole.GetType())
            {
                return 0f;
            }

            int num = 0;
            int length = (int)this.AvailableRights.Length * 2 + 1;
            List<string> strs = new List<string>((int)targetRole.Rights.Length);
            strs.AddRange(targetRole.Rights);
            List<string> strs1 = new List<string>((int)this.Rights.Length);
            strs1.AddRange(this.Rights);
            string[] availableRights = this.AvailableRights;
            for (int i = 0; i < (int)availableRights.Length; i++)
            {
                string str = availableRights[i];
                if (strs1.Contains(str) == strs.Contains(str))
                {
                    num += 2;
                }
            }

            if (this.RoleName == targetRole.RoleName)
            {
                num++;
            }

            return (float)((float)num / (float)length);
        }

        public override string ToString()
        {
            return string.Concat(base.GetType().Name, ":", this.RoleName);
        }
    }
}