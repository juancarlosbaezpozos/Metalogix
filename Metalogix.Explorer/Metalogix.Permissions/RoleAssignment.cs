using Metalogix;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public class RoleAssignment : IEnumerable<Role>, IEnumerable, IXmlable
    {
        private SecurityPrincipal m_principal;

        private SerializableList<Role> m_roles;

        public Role this[string sRoleName]
        {
            get
            {
                Role role;
                using (IEnumerator<Role> enumerator = this.m_roles.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Role current = enumerator.Current;
                        if (current.RoleName != sRoleName)
                        {
                            continue;
                        }

                        role = current;
                        return role;
                    }

                    return null;
                }

                return role;
            }
        }

        public SecurityPrincipal Principal
        {
            get { return this.m_principal; }
        }

        public SerializableList<Role> Roles
        {
            get { return this.m_roles; }
        }

        public RoleAssignment(XmlNode xml)
        {
            this.m_roles = new CommonSerializableSet<Role>();
            if (xml != null)
            {
                this.FromXML(xml);
            }
        }

        public RoleAssignment(SecurityPrincipal principal, Role role)
        {
            this.m_principal = principal;
            this.m_roles = new CommonSerializableSet<Role>()
            {
                role
            };
        }

        public RoleAssignment(SecurityPrincipal principal, Role[] roles)
        {
            this.m_principal = principal;
            this.m_roles = new CommonSerializableSet<Role>();
            this.m_roles.AddRangeToCollection(roles);
        }

        public void FromXML(XmlNode xml)
        {
            XmlNode xmlNodes = xml.SelectSingleNode("//RoleAssignment");
            if (xmlNodes != null)
            {
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./SecurityPrincipal");
                if (xmlNodes1 != null)
                {
                    XmlAttribute itemOf = xmlNodes1.Attributes["Type"];
                    if (itemOf != null)
                    {
                        Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                        object[] firstChild = new object[] { xmlNodes1.FirstChild };
                        this.m_principal = (SecurityPrincipal)Activator.CreateInstance(type, firstChild);
                    }
                }

                xmlNodes1 = xmlNodes.SelectSingleNode("./Roles");
                if (xmlNodes1 != null)
                {
                    this.m_roles.FromXML(xmlNodes1.FirstChild);
                }
            }
        }

        public IEnumerator<Role> GetEnumerator()
        {
            return (IEnumerator<Role>)((IEnumerable)this.m_roles).GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.m_roles).GetEnumerator();
        }

        public string ToXML()
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    this.ToXML(xmlTextWriter);
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("RoleAssignment");
            if (this.m_principal != null)
            {
                xmlWriter.WriteStartElement("SecurityPrincipal");
                xmlWriter.WriteAttributeString("Type", this.m_principal.GetType().FullName);
                this.m_principal.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            if (this.m_roles != null)
            {
                xmlWriter.WriteStartElement("Roles");
                this.m_roles.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}