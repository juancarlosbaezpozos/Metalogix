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
    public class AdvancedOptionFields
    {
        private AdvancedOptionField[] fieldsField;

        public AdvancedOptionField[] Fields
        {
            get { return this.fieldsField; }
            set { this.fieldsField = value; }
        }

        public AdvancedOptionFields()
        {
        }
    }
}