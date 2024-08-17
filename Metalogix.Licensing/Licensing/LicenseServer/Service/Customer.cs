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
    public class Customer
    {
        private Contact[] contactsField;

        private string nameField;

        private string addressField;

        private string sFIDField;

        public string Address
        {
            get { return this.addressField; }
            set { this.addressField = value; }
        }

        public Contact[] Contacts
        {
            get { return this.contactsField; }
            set { this.contactsField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public string SFID
        {
            get { return this.sFIDField; }
            set { this.sFIDField = value; }
        }

        public Customer()
        {
        }
    }
}