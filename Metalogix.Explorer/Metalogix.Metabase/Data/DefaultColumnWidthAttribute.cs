using System;

namespace Metalogix.Metabase.Data
{
    public sealed class DefaultColumnWidthAttribute : Attribute
    {
        private int m_iColumnWidth = 100;

        public int ColumnWidth
        {
            get { return this.m_iColumnWidth; }
            set { this.m_iColumnWidth = value; }
        }

        public DefaultColumnWidthAttribute(int iColumnWidth)
        {
            this.m_iColumnWidth = iColumnWidth;
        }
    }
}