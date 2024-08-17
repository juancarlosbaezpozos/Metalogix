using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddSiteCollectionOptions : AddWebOptions
    {
        private bool m_bValidateOwners;

        private string m_sContentDatabaseName;

        private bool m_bCopySiteAdmins;

        private bool m_bCopySiteQuota;

        private bool m_bSelfServiceCreateMode;

        [DataMember]
        public string ContentDatabase
        {
            get { return this.m_sContentDatabaseName; }
            set { this.m_sContentDatabaseName = value; }
        }

        [DataMember]
        public bool CopySiteAdmins
        {
            get { return this.m_bCopySiteAdmins; }
            set { this.m_bCopySiteAdmins = value; }
        }

        [DataMember]
        public bool CopySiteQuota
        {
            get { return this.m_bCopySiteQuota; }
            set { this.m_bCopySiteQuota = value; }
        }

        [DataMember]
        public bool SelfServiceCreateMode
        {
            get { return this.m_bSelfServiceCreateMode; }
            set { this.m_bSelfServiceCreateMode = value; }
        }

        [DataMember] public int TenantPersonalSiteCreationRetryCount { get; set; }

        [DataMember] public int TenantPersonalSiteCreationWaitInterval { get; set; }

        [DataMember]
        public bool ValidateOwners
        {
            get { return this.m_bValidateOwners; }
            set { this.m_bValidateOwners = value; }
        }

        public AddSiteCollectionOptions()
        {
        }
    }
}