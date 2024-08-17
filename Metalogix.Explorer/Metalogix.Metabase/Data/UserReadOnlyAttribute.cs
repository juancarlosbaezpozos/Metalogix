using System;

namespace Metalogix.Metabase.Data
{
    public sealed class UserReadOnlyAttribute : Attribute
    {
        private readonly bool m_bReadOnly;

        public bool ReadOnly
        {
            get { return this.m_bReadOnly; }
        }

        public UserReadOnlyAttribute(bool bReadOnly)
        {
            this.m_bReadOnly = bReadOnly;
        }
    }
}