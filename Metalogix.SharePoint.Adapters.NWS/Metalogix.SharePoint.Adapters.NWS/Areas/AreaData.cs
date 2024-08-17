using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/")]
    public class AreaData
    {
        private Guid areaIDField;

        private Guid parentIDField;

        private string areaNameField;

        private string smallIconUrlField;

        private int listingCountField;

        private string urlOverrideField;

        private string urlNavigationField;

        private string webUrlField;

        private AreaNavigation navigationField;

        private bool systemField;

        private DateTime appearanceDateField;

        private DateTime expirationDateField;

        private DateTime creationDateField;

        private DateTime lastModifiedField;

        private string largeIconUrlField;

        private string descriptionField;

        private bool isPublicNavField;

        private bool honorOrderField;

        private int orderField;

        private int depthField;

        private byte inheritUrlField;

        private Guid ownerGuidField;

        private string ownerField;

        private string ownerEmailField;

        private string ownerPictureField;

        private string[] synonymsField;

        private bool bool1Field;

        private bool bool2Field;

        private bool bool3Field;

        private string nVarChar1Field;

        private string nVarChar2Field;

        private string nVarChar3Field;

        private string nVarChar4Field;

        private int int1Field;

        private int int2Field;

        private int int3Field;

        private DateTime datetime1Field;

        private string nText1Field;

        public DateTime AppearanceDate
        {
            get { return this.appearanceDateField; }
            set { this.appearanceDateField = value; }
        }

        public Guid AreaID
        {
            get { return this.areaIDField; }
            set { this.areaIDField = value; }
        }

        public string AreaName
        {
            get { return this.areaNameField; }
            set { this.areaNameField = value; }
        }

        public bool Bool1
        {
            get { return this.bool1Field; }
            set { this.bool1Field = value; }
        }

        public bool Bool2
        {
            get { return this.bool2Field; }
            set { this.bool2Field = value; }
        }

        public bool Bool3
        {
            get { return this.bool3Field; }
            set { this.bool3Field = value; }
        }

        public DateTime CreationDate
        {
            get { return this.creationDateField; }
            set { this.creationDateField = value; }
        }

        public DateTime Datetime1
        {
            get { return this.datetime1Field; }
            set { this.datetime1Field = value; }
        }

        public int Depth
        {
            get { return this.depthField; }
            set { this.depthField = value; }
        }

        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        public DateTime ExpirationDate
        {
            get { return this.expirationDateField; }
            set { this.expirationDateField = value; }
        }

        public bool HonorOrder
        {
            get { return this.honorOrderField; }
            set { this.honorOrderField = value; }
        }

        public byte InheritUrl
        {
            get { return this.inheritUrlField; }
            set { this.inheritUrlField = value; }
        }

        public int Int1
        {
            get { return this.int1Field; }
            set { this.int1Field = value; }
        }

        public int Int2
        {
            get { return this.int2Field; }
            set { this.int2Field = value; }
        }

        public int Int3
        {
            get { return this.int3Field; }
            set { this.int3Field = value; }
        }

        public bool IsPublicNav
        {
            get { return this.isPublicNavField; }
            set { this.isPublicNavField = value; }
        }

        public string LargeIconUrl
        {
            get { return this.largeIconUrlField; }
            set { this.largeIconUrlField = value; }
        }

        public DateTime LastModified
        {
            get { return this.lastModifiedField; }
            set { this.lastModifiedField = value; }
        }

        public int ListingCount
        {
            get { return this.listingCountField; }
            set { this.listingCountField = value; }
        }

        public AreaNavigation Navigation
        {
            get { return this.navigationField; }
            set { this.navigationField = value; }
        }

        public string NText1
        {
            get { return this.nText1Field; }
            set { this.nText1Field = value; }
        }

        public string NVarChar1
        {
            get { return this.nVarChar1Field; }
            set { this.nVarChar1Field = value; }
        }

        public string NVarChar2
        {
            get { return this.nVarChar2Field; }
            set { this.nVarChar2Field = value; }
        }

        public string NVarChar3
        {
            get { return this.nVarChar3Field; }
            set { this.nVarChar3Field = value; }
        }

        public string NVarChar4
        {
            get { return this.nVarChar4Field; }
            set { this.nVarChar4Field = value; }
        }

        public int Order
        {
            get { return this.orderField; }
            set { this.orderField = value; }
        }

        public string Owner
        {
            get { return this.ownerField; }
            set { this.ownerField = value; }
        }

        public string OwnerEmail
        {
            get { return this.ownerEmailField; }
            set { this.ownerEmailField = value; }
        }

        public Guid OwnerGuid
        {
            get { return this.ownerGuidField; }
            set { this.ownerGuidField = value; }
        }

        public string OwnerPicture
        {
            get { return this.ownerPictureField; }
            set { this.ownerPictureField = value; }
        }

        public Guid ParentID
        {
            get { return this.parentIDField; }
            set { this.parentIDField = value; }
        }

        public string SmallIconUrl
        {
            get { return this.smallIconUrlField; }
            set { this.smallIconUrlField = value; }
        }

        public string[] Synonyms
        {
            get { return this.synonymsField; }
            set { this.synonymsField = value; }
        }

        public bool System
        {
            get { return this.systemField; }
            set { this.systemField = value; }
        }

        public string UrlNavigation
        {
            get { return this.urlNavigationField; }
            set { this.urlNavigationField = value; }
        }

        public string UrlOverride
        {
            get { return this.urlOverrideField; }
            set { this.urlOverrideField = value; }
        }

        public string WebUrl
        {
            get { return this.webUrlField; }
            set { this.webUrlField = value; }
        }

        public AreaData()
        {
        }
    }
}