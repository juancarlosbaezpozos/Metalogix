using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CredentialEntryStyle : Attribute
    {
        private EntryStyles m_value;

        public EntryStyles Style
        {
            get { return this.m_value; }
        }

        public CredentialEntryStyle(EntryStyles value)
        {
            this.m_value = value;
        }
    }
}