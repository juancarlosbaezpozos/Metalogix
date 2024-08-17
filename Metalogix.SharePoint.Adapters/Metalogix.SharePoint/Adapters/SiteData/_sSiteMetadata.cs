using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class _sSiteMetadata
    {
        private DateTime lastModifiedField;

        private DateTime lastModifiedForceRecrawlField;

        private bool smallSiteField;

        private string portalUrlField;

        private string userProfileGUIDField;

        private bool validSecurityInfoField;

        public DateTime LastModified
        {
            get { return this.lastModifiedField; }
            set { this.lastModifiedField = value; }
        }

        public DateTime LastModifiedForceRecrawl
        {
            get { return this.lastModifiedForceRecrawlField; }
            set { this.lastModifiedForceRecrawlField = value; }
        }

        public string PortalUrl
        {
            get { return this.portalUrlField; }
            set { this.portalUrlField = value; }
        }

        public bool SmallSite
        {
            get { return this.smallSiteField; }
            set { this.smallSiteField = value; }
        }

        public string UserProfileGUID
        {
            get { return this.userProfileGUIDField; }
            set { this.userProfileGUIDField = value; }
        }

        public bool ValidSecurityInfo
        {
            get { return this.validSecurityInfoField; }
            set { this.validSecurityInfoField = value; }
        }

        public _sSiteMetadata()
        {
        }
    }
}