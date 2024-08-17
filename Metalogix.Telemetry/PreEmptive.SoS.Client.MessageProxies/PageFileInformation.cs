using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class PageFileInformation
    {
        private string nameField;

        private int initialSizeField;

        private int maxSizeField;

        public int InitialSize
        {
            get { return this.initialSizeField; }
            set { this.initialSizeField = value; }
        }

        public int MaxSize
        {
            get { return this.maxSizeField; }
            set { this.maxSizeField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public PageFileInformation()
        {
        }
    }
}