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
    public class Contact
    {
        private bool isPrimaryField;

        private string nameField;

        private string emailField;

        private string phoneField;

        public string Email
        {
            get { return this.emailField; }
            set { this.emailField = value; }
        }

        public bool IsPrimary
        {
            get { return this.isPrimaryField; }
            set { this.isPrimaryField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string Phone
        {
            get { return this.phoneField; }
            set { this.phoneField = value; }
        }

        public Contact()
        {
        }
    }
}