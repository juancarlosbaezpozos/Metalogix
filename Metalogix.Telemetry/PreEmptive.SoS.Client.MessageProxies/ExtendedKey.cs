using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class ExtendedKey
    {
        private string dataTypeField;

        private string keyField;

        private string valueField;

        public string DataType
        {
            get { return this.dataTypeField; }
            set { this.dataTypeField = value; }
        }

        public string Key
        {
            get { return this.keyField; }
            set { this.keyField = value; }
        }

        public string Value
        {
            get { return this.valueField; }
            set { this.valueField = value; }
        }

        public ExtendedKey()
        {
        }
    }
}