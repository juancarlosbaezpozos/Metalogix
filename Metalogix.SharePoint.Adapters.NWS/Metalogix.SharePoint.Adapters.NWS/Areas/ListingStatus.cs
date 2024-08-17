using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Areas
{
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/WebQueryService/")]
    public enum ListingStatus
    {
        Pending,
        Approved,
        Rejected,
        Archived
    }
}