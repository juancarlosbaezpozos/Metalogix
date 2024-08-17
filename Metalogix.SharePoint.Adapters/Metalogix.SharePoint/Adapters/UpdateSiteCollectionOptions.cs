using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class UpdateSiteCollectionOptions : AdapterOptions
    {
        private bool m_bUpdateSiteQuota;

        private bool m_bUpdateSiteAuditSettings;

        private bool m_bUpdateSiteAdmins;

        private bool m_bClearExistingSiteAdmins;

        [DataMember]
        public bool ClearExistingSiteAdmins
        {
            get { return this.m_bClearExistingSiteAdmins; }
            set { this.m_bClearExistingSiteAdmins = value; }
        }

        [DataMember]
        public bool UpdateSiteAdmins
        {
            get { return this.m_bUpdateSiteAdmins; }
            set { this.m_bUpdateSiteAdmins = value; }
        }

        [DataMember]
        public bool UpdateSiteAuditSettings
        {
            get { return this.m_bUpdateSiteAuditSettings; }
            set { this.m_bUpdateSiteAuditSettings = value; }
        }

        [DataMember]
        public bool UpdateSiteQuota
        {
            get { return this.m_bUpdateSiteQuota; }
            set { this.m_bUpdateSiteQuota = value; }
        }

        public UpdateSiteCollectionOptions()
        {
        }
    }
}