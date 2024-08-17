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
    public class AdvancedOptionsResponse
    {
        private Metalogix.Licensing.LicenseServer.Service.AdvancedOptionFields advancedOptionFieldsField;

        public Metalogix.Licensing.LicenseServer.Service.AdvancedOptionFields AdvancedOptionFields
        {
            get { return this.advancedOptionFieldsField; }
            set { this.advancedOptionFieldsField = value; }
        }

        public AdvancedOptionsResponse()
        {
        }
    }
}