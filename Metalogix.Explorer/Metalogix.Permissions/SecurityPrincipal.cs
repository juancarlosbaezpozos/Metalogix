using System;
using System.Xml;

namespace Metalogix.Permissions
{
    public abstract class SecurityPrincipal : Member
    {
        private SecurityPrincipalType m_type = SecurityPrincipalType.Unknown;

        public abstract string PrincipalName { get; }

        internal string PrincipalNameInvariant
        {
            get { return this.PrincipalName.Trim().ToLowerInvariant(); }
        }

        public virtual SecurityPrincipalType Type
        {
            get { return this.m_type; }
        }

        public SecurityPrincipal(XmlNode xml) : base(xml)
        {
        }

        public virtual float GetSimilarity(SecurityPrincipal principal)
        {
            if (principal.GetType() != base.GetType())
            {
                return 0f;
            }

            if (this.PrincipalName == principal.PrincipalName)
            {
                return 1f;
            }

            return 0f;
        }
    }
}