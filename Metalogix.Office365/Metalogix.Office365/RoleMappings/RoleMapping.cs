using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Metalogix.Office365.RoleMappings
{
    public class RoleMapping
    {
        [XmlAttribute] public string Id { get; set; }

        [XmlAttribute] public string RoleOrder { get; set; }

        [XmlAttribute] public string RoleType { get; set; }

        [XmlIgnore] public RoleTemplateMapping Template { get; set; }

        [XmlElement("Template")] public List<RoleTemplateMapping> Templates { get; set; }

        public RoleMapping()
        {
        }

        public RoleMapping(string id, string roleOrder, string roleType, List<RoleTemplateMapping> templates)
        {
            this.Id = id;
            this.Templates = templates;
            this.RoleOrder = roleOrder;
            this.RoleType = roleType;
        }
    }
}