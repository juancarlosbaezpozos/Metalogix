using System;

namespace Metalogix.Metabase.Data
{
    public sealed class IndexedAttribute : Attribute
    {
        private readonly bool m_bIndexed;

        public bool Indexed
        {
            get { return this.m_bIndexed; }
        }

        public IndexedAttribute(bool bIndexed)
        {
            this.m_bIndexed = bIndexed;
        }
    }
}