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
    public class LicenseDataResponse
    {
        private LicenseInfo licenseField;

        private Metalogix.Licensing.LicenseServer.Service.CustomFields customFieldsField;

        private Metalogix.Licensing.LicenseServer.Service.Customer customerField;

        private ServerInfos serverField;

        public Metalogix.Licensing.LicenseServer.Service.Customer Customer
        {
            get { return this.customerField; }
            set { this.customerField = value; }
        }

        public Metalogix.Licensing.LicenseServer.Service.CustomFields CustomFields
        {
            get { return this.customFieldsField; }
            set { this.customFieldsField = value; }
        }

        public LicenseInfo License
        {
            get { return this.licenseField; }
            set { this.licenseField = value; }
        }

        public ServerInfos Server
        {
            get { return this.serverField; }
            set { this.serverField = value; }
        }

        public LicenseDataResponse()
        {
        }
    }
}