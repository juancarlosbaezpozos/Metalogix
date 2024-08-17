using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class TimeZoneInformation
    {
        private short currentTimeZoneField;

        private bool daylightSavingsTimeInEffectField;

        public short CurrentTimeZone
        {
            get { return this.currentTimeZoneField; }
            set { this.currentTimeZoneField = value; }
        }

        public bool DaylightSavingsTimeInEffect
        {
            get { return this.daylightSavingsTimeInEffectField; }
            set { this.daylightSavingsTimeInEffectField = value; }
        }

        public TimeZoneInformation()
        {
        }
    }
}