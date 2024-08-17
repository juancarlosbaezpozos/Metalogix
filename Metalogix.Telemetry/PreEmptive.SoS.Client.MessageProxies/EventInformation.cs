using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class EventInformation
    {
        private PrivacySettings privacySettingField;

        private string codeField;

        public string Code
        {
            get { return this.codeField; }
            set { this.codeField = value; }
        }

        public PrivacySettings PrivacySetting
        {
            get { return this.privacySettingField; }
            set { this.privacySettingField = value; }
        }

        public EventInformation()
        {
        }
    }
}