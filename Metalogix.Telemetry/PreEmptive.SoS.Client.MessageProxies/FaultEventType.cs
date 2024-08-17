using System;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public enum FaultEventType
    {
        Thrown,
        Caught,
        Uncaught
    }
}