using System.Drawing;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms
{
    [UsesGroupBox(true)]
    public class ScopableTabbableControl : TabbableControl
    {
        private SharePointObjectScope m_scope = SharePointObjectScope.SiteCollection;

        private bool m_bSiteCollectionsAvailable = true;

        private bool m_bSitesAvailable = true;

        private bool m_bListsAvailable = true;

        private bool m_bFoldersAvailable = true;

        private bool m_bItemsAvailable = true;

        private bool m_bMultiSelectUI;

        private NodeCollection m_sourceNodes;

        private NodeCollection m_targetNodes;

        public bool FoldersAvailable
        {
            get
            {
                return m_bFoldersAvailable;
            }
            set
            {
                m_bFoldersAvailable = value;
                UpdateEnabledState();
            }
        }

        public bool ItemsAvailable
        {
            get
            {
                return m_bItemsAvailable;
            }
            set
            {
                m_bItemsAvailable = value;
                UpdateEnabledState();
            }
        }

        public bool ListsAvailable
        {
            get
            {
                return m_bListsAvailable;
            }
            set
            {
                m_bListsAvailable = value;
                UpdateEnabledState();
            }
        }

        public bool MultiSelectUI
        {
            get
            {
                return m_bMultiSelectUI;
            }
            set
            {
                m_bMultiSelectUI = value;
                MultiSelectUISetup();
            }
        }

        public SharePointObjectScope Scope
        {
            get
            {
                return m_scope;
            }
            set
            {
                m_scope = value;
                SetAvailability();
                UpdateScope();
            }
        }

        public bool SiteCollectionsAvailable
        {
            get
            {
                return m_bSiteCollectionsAvailable;
            }
            set
            {
                m_bSiteCollectionsAvailable = value;
                UpdateEnabledState();
            }
        }

        public bool SitesAvailable
        {
            get
            {
                return m_bSitesAvailable;
            }
            set
            {
                m_bSitesAvailable = value;
                UpdateEnabledState();
            }
        }

        public virtual NodeCollection SourceNodes
        {
            get
            {
                return m_sourceNodes;
            }
            set
            {
                m_sourceNodes = value;
                UpdateEnabledState();
            }
        }

        public virtual NodeCollection TargetNodes
        {
            get
            {
                return m_targetNodes;
            }
            set
            {
                m_targetNodes = value;
                UpdateEnabledState();
            }
        }

        public event AvailabilityChangedEventHandler AvailabilityChanged;

        protected void FireAvailabilityChanged(AvailabilityChangedEventArgs e)
        {
            if (this.AvailabilityChanged != null)
            {
                this.AvailabilityChanged(e, this);
            }
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            this.BackColor = System.Drawing.Color.White;
            base.Name = "ScopableTabbableControl";
            base.ResumeLayout(false);
        }

        protected virtual void MultiSelectUISetup()
        {
        }

        private void SetAvailability()
        {
            m_bSiteCollectionsAvailable = true;
            m_bSitesAvailable = true;
            m_bListsAvailable = true;
            m_bFoldersAvailable = true;
            m_bItemsAvailable = true;
            if (Scope != SharePointObjectScope.SiteCollection)
            {
                m_bSiteCollectionsAvailable = false;
                if (Scope != SharePointObjectScope.Site)
                {
                    m_bSitesAvailable = false;
                    if (Scope != SharePointObjectScope.List)
                    {
                        m_bListsAvailable = false;
                        if (Scope != SharePointObjectScope.Folder)
                        {
                            m_bFoldersAvailable = false;
                            if (Scope != SharePointObjectScope.Item)
                            {
                                m_bItemsAvailable = false;
                            }
                        }
                    }
                }
            }
            UpdateEnabledState();
        }

        protected virtual void UpdateEnabledState()
        {
        }

        protected virtual void UpdateScope()
        {
        }
    }
}
