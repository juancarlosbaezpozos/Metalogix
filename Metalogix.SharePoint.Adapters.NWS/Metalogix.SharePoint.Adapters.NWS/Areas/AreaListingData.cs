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
    public class AreaListingData
    {
        private Guid listingIDField;

        private Guid areaIDField;

        private string titleField;

        private string smallIconUrlField;

        private string urlField;

        private ListingStatus statusField;

        private string descriptionField;

        private ListingType typeField;

        private byte[] personSIDField;

        private Guid personGuidField;

        private string personEmailField;

        private string personPreferredNameField;

        private DateTime appearanceDateField;

        private DateTime expirationDateField;

        private DateTime creationDateField;

        private DateTime lastModifiedField;

        private string largeIconUrlField;

        private int orderField;

        private int groupIDField;

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

        private bool bestBetField;

        private string lastModifiedByField;

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

        public bool BestBet
        {
            get { return this.bestBetField; }
            set { this.bestBetField = value; }
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

        public int GroupID
        {
            get { return this.groupIDField; }
            set { this.groupIDField = value; }
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

        public string LastModifiedBy
        {
            get { return this.lastModifiedByField; }
            set { this.lastModifiedByField = value; }
        }

        public Guid ListingID
        {
            get { return this.listingIDField; }
            set { this.listingIDField = value; }
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

        public string PersonEmail
        {
            get { return this.personEmailField; }
            set { this.personEmailField = value; }
        }

        public Guid PersonGuid
        {
            get { return this.personGuidField; }
            set { this.personGuidField = value; }
        }

        public string PersonPreferredName
        {
            get { return this.personPreferredNameField; }
            set { this.personPreferredNameField = value; }
        }

        [XmlElement(DataType = "base64Binary")]
        public byte[] PersonSID
        {
            get { return this.personSIDField; }
            set { this.personSIDField = value; }
        }

        public string SmallIconUrl
        {
            get { return this.smallIconUrlField; }
            set { this.smallIconUrlField = value; }
        }

        public ListingStatus Status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public ListingType Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public string url
        {
            get { return this.urlField; }
            set { this.urlField = value; }
        }

        public AreaListingData()
        {
        }
    }
}