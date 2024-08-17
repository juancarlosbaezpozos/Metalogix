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
    public class AdvancedOptionField
    {
        private string nameField;

        private string valueField;

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string Value
        {
            get { return this.valueField; }
            set { this.valueField = value; }
        }

        public AdvancedOptionField()
        {
        }
    }
}