using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms
{
    public class AvailabilityChangedEventArgs : EventArgs
    {
        private SharePointObjectScope m_scope;

        private bool m_bAvailable;

        public bool ObjectAvailable
        {
            get
            {
                return this.m_bAvailable;
            }
        }

        public SharePointObjectScope ObjectType
        {
            get
            {
                return this.m_scope;
            }
        }

        public AvailabilityChangedEventArgs(SharePointObjectScope scope, bool available)
        {
            this.m_scope = scope;
            this.m_bAvailable = available;
        }
    }
}