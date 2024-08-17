using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = "http://license.metalogix.com/webservices/")]
    public class BPOSLicenseInfo
    {
        private LicenseInfo licenseField;

        private Association[] associationsField;

        public Association[] Associations
        {
            get { return this.associationsField; }
            set { this.associationsField = value; }
        }

        public LicenseInfo License
        {
            get { return this.licenseField; }
            set { this.licenseField = value; }
        }

        public BPOSLicenseInfo()
        {
        }
    }
}