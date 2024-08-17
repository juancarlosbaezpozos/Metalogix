using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Sites
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public class Template
    {
        private int idField;

        private string titleField;

        private string nameField;

        private bool isUniqueField;

        private bool isHiddenField;

        private string descriptionField;

        private string imageUrlField;

        private bool isCustomField;

        private bool isSubWebOnlyField;

        private bool isRootWebOnlyField;

        private string displayCategoryField;

        private string filterCategoriesField;

        private bool hasProvisionClassField;

        [XmlAttribute]
        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        [XmlAttribute]
        public string DisplayCategory
        {
            get { return this.displayCategoryField; }
            set { this.displayCategoryField = value; }
        }

        [XmlAttribute]
        public string FilterCategories
        {
            get { return this.filterCategoriesField; }
            set { this.filterCategoriesField = value; }
        }

        [XmlAttribute]
        public bool HasProvisionClass
        {
            get { return this.hasProvisionClassField; }
            set { this.hasProvisionClassField = value; }
        }

        [XmlAttribute]
        public int ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        [XmlAttribute]
        public string ImageUrl
        {
            get { return this.imageUrlField; }
            set { this.imageUrlField = value; }
        }

        [XmlAttribute]
        public bool IsCustom
        {
            get { return this.isCustomField; }
            set { this.isCustomField = value; }
        }

        [XmlAttribute]
        public bool IsHidden
        {
            get { return this.isHiddenField; }
            set { this.isHiddenField = value; }
        }

        [XmlAttribute]
        public bool IsRootWebOnly
        {
            get { return this.isRootWebOnlyField; }
            set { this.isRootWebOnlyField = value; }
        }

        [XmlAttribute]
        public bool IsSubWebOnly
        {
            get { return this.isSubWebOnlyField; }
            set { this.isSubWebOnlyField = value; }
        }

        [XmlAttribute]
        public bool IsUnique
        {
            get { return this.isUniqueField; }
            set { this.isUniqueField = value; }
        }

        [XmlAttribute]
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        [XmlAttribute]
        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        public Template()
        {
        }
    }
}