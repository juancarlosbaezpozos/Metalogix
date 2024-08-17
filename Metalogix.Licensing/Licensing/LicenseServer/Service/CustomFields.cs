using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public class CustomFields
    {
        private CustomField[] fieldsField;

        public CustomField[] Fields
        {
            get { return this.fieldsField; }
            set { this.fieldsField = value; }
        }

        public CustomFields()
        {
        }
    }
}