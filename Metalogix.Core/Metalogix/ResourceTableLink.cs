using Metalogix.ObjectResolution;
using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix
{
    public class ResourceTableLink : ObjectLink<ResourceTable>
    {
        public string Name { get; private set; }

        public ResourceScope Scope { get; private set; }

        public ResourceTableLink(string name, ResourceScope scope)
        {
            this.Name = name;
            this.Scope = scope;
        }

        public override bool Equals(object obj)
        {
            ResourceTableLink resourceTableLink = obj as ResourceTableLink;
            if (obj == null)
            {
                return false;
            }

            if (this.Name != resourceTableLink.Name)
            {
                return false;
            }

            return this.Scope == resourceTableLink.Scope;
        }

        public override int GetHashCode()
        {
            return string.Concat(this.Name, this.Scope.ToString()).GetHashCode();
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Resource");
            xmlWriter.WriteAttributeString("Name", this.Name);
            xmlWriter.WriteAttributeString("Scope", this.Scope.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}