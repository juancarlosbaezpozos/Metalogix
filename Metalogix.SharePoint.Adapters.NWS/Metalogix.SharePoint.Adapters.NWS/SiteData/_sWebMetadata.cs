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
    public class _sWebMetadata
    {
        private string webIDField;

        private string titleField;

        private string descriptionField;

        private string authorField;

        private uint languageField;

        private DateTime lastModifiedField;

        private DateTime lastModifiedForceRecrawlField;

        private string noIndexField;

        private bool validSecurityInfoField;

        private bool inheritedSecurityField;

        private bool allowAnonymousAccessField;

        private bool anonymousViewListItemsField;

        private string permissionsField;

        private bool externalSecurityField;

        private string categoryIdField;

        private string categoryNameField;

        private string categoryIdPathField;

        private bool isBucketWebField;

        private bool usedInAutocatField;

        private string categoryBucketIDField;

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

        public string CategoryBucketID
        {
            get { return this.categoryBucketIDField; }
            set { this.categoryBucketIDField = value; }
        }

        public string CategoryId
        {
            get { return this.categoryIdField; }
            set { this.categoryIdField = value; }
        }

        public string CategoryIdPath
        {
            get { return this.categoryIdPathField; }
            set { this.categoryIdPathField = value; }
        }

        public string CategoryName
        {
            get { return this.categoryNameField; }
            set { this.categoryNameField = value; }
        }

        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        public bool ExternalSecurity
        {
            get { return this.externalSecurityField; }
            set { this.externalSecurityField = value; }
        }

        public bool InheritedSecurity
        {
            get { return this.inheritedSecurityField; }
            set { this.inheritedSecurityField = value; }
        }

        public bool IsBucketWeb
        {
            get { return this.isBucketWebField; }
            set { this.isBucketWebField = value; }
        }

        public uint Language
        {
            get { return this.languageField; }
            set { this.languageField = value; }
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

        public string NoIndex
        {
            get { return this.noIndexField; }
            set { this.noIndexField = value; }
        }

        public string Permissions
        {
            get { return this.permissionsField; }
            set { this.permissionsField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public bool UsedInAutocat
        {
            get { return this.usedInAutocatField; }
            set { this.usedInAutocatField = value; }
        }

        public bool ValidSecurityInfo
        {
            get { return this.validSecurityInfoField; }
            set { this.validSecurityInfoField = value; }
        }

        public string WebID
        {
            get { return this.webIDField; }
            set { this.webIDField = value; }
        }

        public _sWebMetadata()
        {
        }
    }
}