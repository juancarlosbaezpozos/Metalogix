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
    public class _sList
    {
        private string internalNameField;

        private string titleField;

        private string descriptionField;

        private string baseTypeField;

        private string baseTemplateField;

        private string defaultViewUrlField;

        private string lastModifiedField;

        private string permIdField;

        private bool inheritedSecurityField;

        private bool allowAnonymousAccessField;

        private bool anonymousViewListItemsField;

        private int readSecurityField;

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

        public string InternalName
        {
            get { return this.internalNameField; }
            set { this.internalNameField = value; }
        }

        public string LastModified
        {
            get { return this.lastModifiedField; }
            set { this.lastModifiedField = value; }
        }

        public string PermId
        {
            get { return this.permIdField; }
            set { this.permIdField = value; }
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

        public _sList()
        {
        }
    }
}