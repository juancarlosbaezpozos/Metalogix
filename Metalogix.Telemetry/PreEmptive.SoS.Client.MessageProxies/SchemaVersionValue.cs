using System;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public enum SchemaVersionValue
    {
        [XmlEnum("01.00.02")] Item010002,
        [XmlEnum("01.00.03")] Item010003,
        [XmlEnum("02.00.00")] Item020000
    }
}