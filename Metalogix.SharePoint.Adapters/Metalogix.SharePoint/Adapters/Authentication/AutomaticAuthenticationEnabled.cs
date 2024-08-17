using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AutomaticAuthenticationEnabled : Attribute
    {
        private bool m_bValue;

        public bool Value
        {
            get { return this.m_bValue; }
        }

        public AutomaticAuthenticationEnabled(bool bValue)
        {
            this.m_bValue = bValue;
        }
    }
}