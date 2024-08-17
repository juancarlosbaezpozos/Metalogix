using System;

namespace Metalogix.Metabase.Data
{
    public sealed class DataGridBrowseableAttribute : Attribute
    {
        private readonly bool m_bBrowseAble;

        public bool DataGridBrowseable
        {
            get { return this.m_bBrowseAble; }
        }

        public DataGridBrowseableAttribute(bool bBrowseAble)
        {
            this.m_bBrowseAble = bBrowseAble;
        }
    }
}