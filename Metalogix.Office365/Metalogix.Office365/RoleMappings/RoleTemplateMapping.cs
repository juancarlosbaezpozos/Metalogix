using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Metalogix.Office365.RoleMappings
{
    public class RoleTemplateMapping
    {
        [XmlAttribute("DefaultTitle")] public string DefaultTitle { get; set; }

        [XmlElement("Description")] public string Description { get; set; }

        [XmlAttribute("Id")] public string TemplateId { get; set; }

        [XmlElement("Title")] public string Title { get; set; }

        public RoleTemplateMapping()
        {
        }
    }
}