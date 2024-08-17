using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [Flags]
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public enum LicenseDataDetails
    {
        LicenseInfo = 1,
        CustomerInfo = 2,
        ServerInfo = 4,
        CustomFields = 8,
        Full = 16
    }
}