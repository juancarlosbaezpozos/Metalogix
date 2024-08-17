using System;

namespace Metalogix.Actions
{
    public class NavigationRequestEventArgs : EventArgs
    {
        private string m_sXmlLocation;

        private NavigationPreference m_navPref;

        public NavigationPreference NavPreference
        {
            get { return this.m_navPref; }
            set { this.m_navPref = value; }
        }

        public string XmlLocation
        {
            get { return this.m_sXmlLocation; }
            set { this.m_sXmlLocation = value; }
        }

        public NavigationRequestEventArgs()
        {
        }
    }
}