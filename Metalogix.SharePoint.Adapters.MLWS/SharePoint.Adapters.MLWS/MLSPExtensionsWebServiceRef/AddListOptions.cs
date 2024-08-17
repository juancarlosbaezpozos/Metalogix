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
    public class AddListOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddOptions
    {
        private bool copyViewsField;

        private bool copyFieldsField;

        private bool updateFieldTypesField;

        private bool copyEnableAssignToEmailField;

        private bool deletePreExistingViewsField;

        private bool ensureUrlNameMatchesInputField;

        public bool CopyEnableAssignToEmail
        {
            get { return this.copyEnableAssignToEmailField; }
            set { this.copyEnableAssignToEmailField = value; }
        }

        public bool CopyFields
        {
            get { return this.copyFieldsField; }
            set { this.copyFieldsField = value; }
        }

        public bool CopyViews
        {
            get { return this.copyViewsField; }
            set { this.copyViewsField = value; }
        }

        public bool DeletePreExistingViews
        {
            get { return this.deletePreExistingViewsField; }
            set { this.deletePreExistingViewsField = value; }
        }

        public bool EnsureUrlNameMatchesInput
        {
            get { return this.ensureUrlNameMatchesInputField; }
            set { this.ensureUrlNameMatchesInputField = value; }
        }

        public bool UpdateFieldTypes
        {
            get { return this.updateFieldTypesField; }
            set { this.updateFieldTypesField = value; }
        }

        public AddListOptions()
        {
        }
    }
}