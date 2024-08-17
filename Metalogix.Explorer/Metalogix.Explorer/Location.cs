using Metalogix.ObjectResolution;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.Explorer
{
    public class Location : ObjectLink<Node>
    {
        private string m_sDisplayUrl;

        private string m_sConnectionString;

        private string m_sPath;

        private Node m_node;

        public string ConnectionString
        {
            get { return this.m_sConnectionString; }
        }

        public string DisplayUrl
        {
            get { return this.m_sDisplayUrl; }
        }

        public string Path
        {
            get { return this.m_sPath; }
        }

        public Type ResolvedObjectType
        {
            get { return typeof(Node); }
        }

        public Location(XmlNode xmlNode)
        {
            this.FromXml(xmlNode);
        }

        public Location(string sPath, string sDisplayUrl, string sConnectionString)
        {
            this.m_sPath = sPath;
            this.m_sDisplayUrl = sDisplayUrl;
            this.m_sConnectionString = sConnectionString;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location))
            {
                return false;
            }

            return ((Location)obj).Path == this.Path;
        }

        public void FromXml(string strXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(strXml);
            this.FromXml(xmlDocument.FirstChild);
        }

        public void FromXml(XmlNode xmlNode)
        {
            XmlNode xmlNodes = (string.Equals("Location", xmlNode.Name, StringComparison.OrdinalIgnoreCase)
                ? xmlNode
                : xmlNode.SelectSingleNode("./Location"));
            XmlNode itemOf = xmlNodes.Attributes["Path"];
            if (itemOf != null)
            {
                this.m_sPath = itemOf.InnerText;
            }

            XmlNode itemOf1 = xmlNodes.Attributes["DisplayUrl"];
            if (itemOf1 != null)
            {
                this.m_sDisplayUrl = itemOf1.InnerText;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./Connection");
            if (xmlNodes1 != null)
            {
                this.m_sConnectionString = xmlNodes1.OuterXml;
            }
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public virtual Node GetNode()
        {
            if (this.m_node == null)
            {
                this.m_node = base.Resolve();
            }

            return this.m_node;
        }

        public bool IsEqual(Location location)
        {
            return location.ToXML().Equals(this.ToXML());
        }

        public override string ToString()
        {
            return this.DisplayUrl;
        }

        public override string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        public override void ToXML(XmlWriter writer)
        {
            writer.WriteStartElement("Location");
            writer.WriteAttributeString("Path", this.Path);
            writer.WriteAttributeString("DisplayUrl", this.DisplayUrl);
            writer.WriteRaw(this.ConnectionString);
            writer.WriteEndElement();
        }
    }
}