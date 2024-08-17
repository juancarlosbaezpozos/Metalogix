using System;

namespace Metalogix.Metabase.Data
{
    public sealed class DefaultDisplaySettingAttribute : Attribute
    {
        private bool m_bShowInGridView;

        private bool m_bShowInPropertyExplorer;

        public bool ShowInGridView
        {
            get { return this.m_bShowInGridView; }
        }

        public bool ShowInPropertyExplorer
        {
            get { return this.m_bShowInPropertyExplorer; }
        }

        public DefaultDisplaySettingAttribute(bool bShowInGridView, bool bShowInPropertyExplorer)
        {
            this.m_bShowInGridView = bShowInGridView;
            this.m_bShowInPropertyExplorer = bShowInPropertyExplorer;
        }
    }
}