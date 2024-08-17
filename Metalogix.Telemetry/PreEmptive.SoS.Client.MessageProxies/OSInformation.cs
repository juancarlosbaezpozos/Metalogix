using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class OSInformation
    {
        private string osProductIdField;

        private DateTime osInstallDateField;

        private string osNameField;

        private long osServicePackMajorVersionField;

        private long osServicePackMinorVersionField;

        private string localeField;

        private string oSLanguageField;

        private bool isVirtualizedField;

        public bool IsVirtualized
        {
            get { return this.isVirtualizedField; }
            set { this.isVirtualizedField = value; }
        }

        public string Locale
        {
            get { return this.localeField; }
            set { this.localeField = value; }
        }

        public DateTime OsInstallDate
        {
            get { return this.osInstallDateField; }
            set { this.osInstallDateField = value; }
        }

        public string OSLanguage
        {
            get { return this.oSLanguageField; }
            set { this.oSLanguageField = value; }
        }

        public string OsName
        {
            get { return this.osNameField; }
            set { this.osNameField = value; }
        }

        public string OsProductId
        {
            get { return this.osProductIdField; }
            set { this.osProductIdField = value; }
        }

        public long OsServicePackMajorVersion
        {
            get { return this.osServicePackMajorVersionField; }
            set { this.osServicePackMajorVersionField = value; }
        }

        public long OsServicePackMinorVersion
        {
            get { return this.osServicePackMinorVersionField; }
            set { this.osServicePackMinorVersionField = value; }
        }

        public OSInformation()
        {
        }
    }
}