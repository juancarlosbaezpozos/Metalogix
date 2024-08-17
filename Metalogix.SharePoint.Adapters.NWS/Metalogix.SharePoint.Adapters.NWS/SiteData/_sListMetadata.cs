using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.SiteData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class _sListMetadata
    {
        private string titleField;

        private string descriptionField;

        private string baseTypeField;

        private string baseTemplateField;

        private string defaultViewUrlField;

        private DateTime lastModifiedField;

        private DateTime lastModifiedForceRecrawlField;

        private string authorField;

        private bool validSecurityInfoField;

        private bool inheritedSecurityField;

        private bool allowAnonymousAccessField;

        private bool anonymousViewListItemsField;

        private int readSecurityField;

        private string permissionsField;

        public bool AllowAnonymousAccess
        {
            get { return this.allowAnonymousAccessField; }
            set { this.allowAnonymousAccessField = value; }
        }

        public bool AnonymousViewListItems
        {
            get { return this.anonymousViewListItemsField; }
            set { this.anonymousViewListItemsField = value; }
        }

        public string Author
        {
            get { return this.authorField; }
            set { this.authorField = value; }
        }

        public string BaseTemplate
        {
            get { return this.baseTemplateField; }
            set { this.baseTemplateField = value; }
        }

        public string BaseType
        {
            get { return this.baseTypeField; }
            set { this.baseTypeField = value; }
        }

        public string DefaultViewUrl
        {
            get { return this.defaultViewUrlField; }
            set { this.defaultViewUrlField = value; }
        }

        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        public bool InheritedSecurity
        {
            get { return this.inheritedSecurityField; }
            set { this.inheritedSecurityField = value; }
        }

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

        public string Permissions
        {
            get { return this.permissionsField; }
            set { this.permissionsField = value; }
        }

        public int ReadSecurity
        {
            get { return this.readSecurityField; }
            set { this.readSecurityField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public bool ValidSecurityInfo
        {
            get { return this.validSecurityInfoField; }
            set { this.validSecurityInfoField = value; }
        }

        public _sListMetadata()
        {
        }
    }
}