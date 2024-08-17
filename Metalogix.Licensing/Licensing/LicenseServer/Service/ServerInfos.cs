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
    public class ServerInfos
    {
        private ServerInfo[] infosField;

        private AdminAccount[] adminAccountsField;

        public AdminAccount[] AdminAccounts
        {
            get { return this.adminAccountsField; }
            set { this.adminAccountsField = value; }
        }

        public ServerInfo[] Infos
        {
            get { return this.infosField; }
            set { this.infosField = value; }
        }

        public ServerInfos()
        {
        }
    }
}