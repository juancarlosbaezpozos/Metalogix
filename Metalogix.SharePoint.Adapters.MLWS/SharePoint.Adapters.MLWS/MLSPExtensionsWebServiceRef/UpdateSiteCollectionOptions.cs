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
    public class
        UpdateSiteCollectionOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AdapterOptions
    {
        private bool updateSiteQuotaField;

        private bool updateSiteAuditSettingsField;

        private bool updateSiteAdminsField;

        private bool clearExistingSiteAdminsField;

        public bool ClearExistingSiteAdmins
        {
            get { return this.clearExistingSiteAdminsField; }
            set { this.clearExistingSiteAdminsField = value; }
        }

        public bool UpdateSiteAdmins
        {
            get { return this.updateSiteAdminsField; }
            set { this.updateSiteAdminsField = value; }
        }

        public bool UpdateSiteAuditSettings
        {
            get { return this.updateSiteAuditSettingsField; }
            set { this.updateSiteAuditSettingsField = value; }
        }

        public bool UpdateSiteQuota
        {
            get { return this.updateSiteQuotaField; }
            set { this.updateSiteQuotaField = value; }
        }

        public UpdateSiteCollectionOptions()
        {
        }
    }
}