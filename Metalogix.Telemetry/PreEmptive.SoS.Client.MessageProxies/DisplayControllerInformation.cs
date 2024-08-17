using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class DisplayControllerInformation
    {
        private string nameField;

        private int refreshRateField;

        private int verticalResolutionField;

        private int horizontalResolutionField;

        public int HorizontalResolution
        {
            get { return this.horizontalResolutionField; }
            set { this.horizontalResolutionField = value; }
        }

        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        public int RefreshRate
        {
            get { return this.refreshRateField; }
            set { this.refreshRateField = value; }
        }

        public int VerticalResolution
        {
            get { return this.verticalResolutionField; }
            set { this.verticalResolutionField = value; }
        }

        public DisplayControllerInformation()
        {
        }
    }
}