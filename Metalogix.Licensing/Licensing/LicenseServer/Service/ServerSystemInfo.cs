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
    public class ServerSystemInfo
    {
        private string keyField;

        private string serverField;

        private string systemInfoField;

        public string Key
        {
            get { return this.keyField; }
            set { this.keyField = value; }
        }

        public string Server
        {
            get { return this.serverField; }
            set { this.serverField = value; }
        }

        public string SystemInfo
        {
            get { return this.systemInfoField; }
            set { this.systemInfoField = value; }
        }

        public ServerSystemInfo()
        {
        }
    }
}