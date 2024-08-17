using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.SiteData
{
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public enum ObjectType
    {
        VirtualServer,
        ContentDatabase,
        SiteCollection,
        Site,
        List,
        Folder,
        ListItem,
        ListItemAttachments
    }
}