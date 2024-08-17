using Metalogix.Explorer;
using System;

namespace Metalogix.Metabase
{
    public class WorkspaceField : ListField
    {
        private bool m_bIsDisplayed;

        public bool IsDisplayed
        {
            get { return this.m_bIsDisplayed; }
        }

        public WorkspaceField(string sName, string sDisplayName, bool bIsDisplayed) : base(sName, sDisplayName, null)
        {
            this.m_bIsDisplayed = bIsDisplayed;
        }
    }
}