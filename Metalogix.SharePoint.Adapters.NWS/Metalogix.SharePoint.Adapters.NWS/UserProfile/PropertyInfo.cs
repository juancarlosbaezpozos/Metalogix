using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService")]
    public class PropertyInfo
    {
        private string nameField;

        private string descriptionField;

        private int displayOrderField;

        private int maximumShownField;

        private bool isAdminEditableField;

        private bool isSearchableField;

        private bool isSystemField;

        private string managedPropertyNameField;

        private string displayNameField;

        private string typeField;

        private bool allowPolicyOverrideField;

        private Privacy defaultPrivacyField;

        private bool isAliasField;

        private bool isColleagueEventLogField;

        private bool isRequiredField;

        private bool isUserEditableField;

        private bool isVisibleOnEditorField;

        private bool isVisibleOnViewerField;

        private bool isReplicableField;

        private bool userOverridePrivacyField;

        private int lengthField;

        private bool isImportedField;

        private bool isMultiValueField;

        private ChoiceTypes choiceTypeField;

        private Guid? termSetIdField;

        public bool AllowPolicyOverride
        {
            get { return this.allowPolicyOverrideField; }
            set { this.allowPolicyOverrideField = value; }
        }

        public ChoiceTypes ChoiceType
        {
            get { return this.choiceTypeField; }
            set { this.choiceTypeField = value; }
        }

        public Privacy DefaultPrivacy
        {
            get { return this.defaultPrivacyField; }
            set { this.defaultPrivacyField = value; }
        }

        public string Description
        {
            get { return this.descriptionField; }
            set { this.descriptionField = value; }
        }

        public string DisplayName
        {
            get { return this.displayNameField; }
            set { this.displayNameField = value; }
        }

        public int DisplayOrder
        {
            get { return this.displayOrderField; }
            set { this.displayOrderField = value; }
        }

        public bool IsAdminEditable
        {
            get { return this.isAdminEditableField; }
            set { this.isAdminEditableField = value; }
        }

        public bool IsAlias
        {
            get { return this.isAliasField; }
            set { this.isAliasField = value; }
        }

        public bool IsColleagueEventLog
        {
            get { return this.isColleagueEventLogField; }
            set { this.isColleagueEventLogField = value; }
        }

        public bool IsImported
        {
            get { return this.isImportedField; }
            set { this.isImportedField = value; }
        }

        public bool IsMultiValue
        {
            get { return this.isMultiValueField; }
            set { this.isMultiValueField = value; }
        }

        public bool IsReplicable
        {
            get { return this.isReplicableField; }
            set { this.isReplicableField = value; }
        }

        public bool IsRequired
        {
            get { return this.isRequiredField; }
            set { this.isRequiredField = value; }
        }

        public bool IsSearchable
        {
            get { return this.isSearchableField; }
            set { this.isSearchableField = value; }
        }

        public bool IsSystem
        {
            get { return this.isSystemField; }
            set { this.isSystemField = value; }
        }

        public bool IsUserEditable
        {
            get { return this.isUserEditableField; }
            set { this.isUserEditableField = value; }
        }

        public bool IsVisibleOnEditor
        {
            get { return this.isVisibleOnEditorField; }
            set { this.isVisibleOnEditorField = value; }
        }

        public bool IsVisibleOnViewer
        {
            get { return this.isVisibleOnViewerField; }
            set { this.isVisibleOnViewerField = value; }
        }

        public int Length
        {
            get { return this.lengthField; }
            set { this.lengthField = value; }
        }

        public string ManagedPropertyName
        {
            get { return this.managedPropertyNameField; }
            set { this.managedPropertyNameField = value; }
        }

        public int MaximumShown
        {
            get { return this.maximumShownField; }
            set { this.maximumShownField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        [XmlElement(IsNullable = true)]
        public Guid? TermSetId
        {
            get { return this.termSetIdField; }
            set { this.termSetIdField = value; }
        }

        public string Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public bool UserOverridePrivacy
        {
            get { return this.userOverridePrivacyField; }
            set { this.userOverridePrivacyField = value; }
        }

        public PropertyInfo()
        {
        }
    }
}