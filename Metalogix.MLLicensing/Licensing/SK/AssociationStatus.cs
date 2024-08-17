using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = "http://license.metalogix.com/webservices/")]
    public enum AssociationStatus
    {
        NPOR,
        POR,
        Invalid,
        Purchased,
        Pending
    }
}