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
    public class LicenseInfoRequest
    {
        private string keyField;

        private long usedDataField;

        private string serverField;

        private string adminField;

        private int serverCountField;

        private string productVersionField;

        private int productCodeField;

        private string messageField;

        private string systemInfoField;

        public string Admin
        {
            get { return this.adminField; }
            set { this.adminField = value; }
        }

        public string Key
        {
            get { return this.keyField; }
            set { this.keyField = value; }
        }

        public string Message
        {
            get { return this.messageField; }
            set { this.messageField = value; }
        }

        public int ProductCode
        {
            get { return this.productCodeField; }
            set { this.productCodeField = value; }
        }

        public string ProductVersion
        {
            get { return this.productVersionField; }
            set { this.productVersionField = value; }
        }

        public string Server
        {
            get { return this.serverField; }
            set { this.serverField = value; }
        }

        public int ServerCount
        {
            get { return this.serverCountField; }
            set { this.serverCountField = value; }
        }

        public string SystemInfo
        {
            get { return this.systemInfoField; }
            set { this.systemInfoField = value; }
        }

        public long UsedData
        {
            get { return this.usedDataField; }
            set { this.usedDataField = value; }
        }

        public LicenseInfoRequest()
        {
        }
    }
}