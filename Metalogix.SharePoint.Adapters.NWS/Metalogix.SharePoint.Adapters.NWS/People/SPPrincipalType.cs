using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.People
{
    [Flags]
    [GeneratedCode("System.Xml", "4.0.30319.18034")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public enum SPPrincipalType
    {
        None = 1,
        User = 2,
        DistributionList = 4,
        SecurityGroup = 8,
        SharePointGroup = 16,
        All = 32
    }
}