using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public enum LicenseType
    {
        Trial,
        Perpetual,
        Term
    }
}