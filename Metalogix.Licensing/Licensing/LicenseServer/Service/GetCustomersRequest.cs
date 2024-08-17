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
    public class GetCustomersRequest
    {
        private StringFilter nameField;

        private StringFilter contactNameField;

        private StringFilter contactEmailField;

        private int offsetField;

        private int pageSizeField;

        public StringFilter ContactEmail
        {
            get { return this.contactEmailField; }
            set { this.contactEmailField = value; }
        }

        public StringFilter ContactName
        {
            get { return this.contactNameField; }
            set { this.contactNameField = value; }
        }

        public StringFilter Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public int Offset
        {
            get { return this.offsetField; }
            set { this.offsetField = value; }
        }

        public int PageSize
        {
            get { return this.pageSizeField; }
            set { this.pageSizeField = value; }
        }

        public GetCustomersRequest()
        {
        }
    }
}