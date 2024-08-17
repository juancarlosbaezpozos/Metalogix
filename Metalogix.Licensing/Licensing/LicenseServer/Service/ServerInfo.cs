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
    public class ServerInfo
    {
        private DateTime lastRequestField;

        private DateTime activationField;

        private long usedDataField;

        private int serverCountField;

        private string serverIDField;

        private string productVersionField;

        private int productField;

        public DateTime Activation
        {
            get { return this.activationField; }
            set { this.activationField = value; }
        }

        public DateTime LastRequest
        {
            get { return this.lastRequestField; }
            set { this.lastRequestField = value; }
        }

        public int Product
        {
            get { return this.productField; }
            set { this.productField = value; }
        }

        public string ProductVersion
        {
            get { return this.productVersionField; }
            set { this.productVersionField = value; }
        }

        public int ServerCount
        {
            get { return this.serverCountField; }
            set { this.serverCountField = value; }
        }

        public string ServerID
        {
            get { return this.serverIDField; }
            set { this.serverIDField = value; }
        }

        public long UsedData
        {
            get { return this.usedDataField; }
            set { this.usedDataField = value; }
        }

        public ServerInfo()
        {
        }
    }
}