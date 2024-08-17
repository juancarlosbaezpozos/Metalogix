using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.34230")]
    [Serializable]
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class AddSiteCollectionOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddWebOptions
    {
        private bool validateOwnersField;

        private string contentDatabaseField;

        private bool copySiteAdminsField;

        private bool copySiteQuotaField;

        private bool selfServiceCreateModeField;

        private int tenantPersonalSiteCreationWaitIntervalField;

        private int tenantPersonalSiteCreationRetryCountField;

        public string ContentDatabase
        {
            get { return this.contentDatabaseField; }
            set { this.contentDatabaseField = value; }
        }

        public bool CopySiteAdmins
        {
            get { return this.copySiteAdminsField; }
            set { this.copySiteAdminsField = value; }
        }

        public bool CopySiteQuota
        {
            get { return this.copySiteQuotaField; }
            set { this.copySiteQuotaField = value; }
        }

        public bool SelfServiceCreateMode
        {
            get { return this.selfServiceCreateModeField; }
            set { this.selfServiceCreateModeField = value; }
        }

        public int TenantPersonalSiteCreationRetryCount
        {
            get { return this.tenantPersonalSiteCreationRetryCountField; }
            set { this.tenantPersonalSiteCreationRetryCountField = value; }
        }

        public int TenantPersonalSiteCreationWaitInterval
        {
            get { return this.tenantPersonalSiteCreationWaitIntervalField; }
            set { this.tenantPersonalSiteCreationWaitIntervalField = value; }
        }

        public bool ValidateOwners
        {
            get { return this.validateOwnersField; }
            set { this.validateOwnersField = value; }
        }

        public AddSiteCollectionOptions()
        {
        }
    }
}