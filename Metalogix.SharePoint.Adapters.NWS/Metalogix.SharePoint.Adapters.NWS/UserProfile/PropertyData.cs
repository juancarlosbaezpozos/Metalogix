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
    public class PropertyData
    {
        private bool isPrivacyChangedField;

        private bool isValueChangedField;

        private string nameField;

        private Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy privacyField;

        private ValueData[] valuesField;

        public bool IsPrivacyChanged
        {
            get { return this.isPrivacyChangedField; }
            set { this.isPrivacyChangedField = value; }
        }

        public bool IsValueChanged
        {
            get { return this.isValueChangedField; }
            set { this.isValueChangedField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public Metalogix.SharePoint.Adapters.NWS.UserProfile.Privacy Privacy
        {
            get { return this.privacyField; }
            set { this.privacyField = value; }
        }

        public ValueData[] Values
        {
            get { return this.valuesField; }
            set { this.valuesField = value; }
        }

        public PropertyData()
        {
        }
    }
}