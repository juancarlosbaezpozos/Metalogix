using System;

namespace Metalogix.Metabase.Data
{
    public sealed class ColumnOrdinalAttribute : Attribute
    {
        private int m_iOrdinal;

        public int Ordinal
        {
            get { return this.m_iOrdinal; }
            set { this.m_iOrdinal = value; }
        }

        public ColumnOrdinalAttribute(int iOrdinal)
        {
            this.m_iOrdinal = iOrdinal;
        }
    }
}