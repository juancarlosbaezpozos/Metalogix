using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.Permissions;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.ExternalConnections
{
    public abstract class ExternalConnection : IXmlable
    {
        private int m_externalConnectionID = -1;

        protected ConnectionStatus m_Status;

        protected ExternalConnectionType m_Type;

        private Metalogix.Permissions.Credentials m_credentials;

        public Metalogix.Permissions.Credentials Credentials
        {
            get { return this.m_credentials; }
            set { this.m_credentials = value; }
        }

        public int ExternalConnectionID
        {
            get { return this.m_externalConnectionID; }
            internal set { this.m_externalConnectionID = value; }
        }

        public virtual ConnectionStatus Status
        {
            get { return this.m_Status; }
            set { this.m_Status = value; }
        }

        public virtual ExternalConnectionType Type
        {
            get { return this.m_Type; }
        }

        public ExternalConnection()
        {
        }

        public ExternalConnection(XmlNode ndExternalConnection)
        {
            this.FromXml(ndExternalConnection);
        }

        public abstract void CheckConnection();

        public void Delete()
        {
            if (this.ExternalConnectionID == -1)
            {
                throw new Exception("Connection is not in database");
            }

            ExternalConnectionManager.INSTANCE.RemoveConnection(this);
        }

        public virtual void FromXml(XmlNode ndExternalConnection)
        {
            this.m_credentials =
                new Metalogix.Permissions.Credentials(ndExternalConnection.SelectSingleNode(".//Credentials"));
            this.m_Type = (ExternalConnectionType)Enum.Parse(typeof(ExternalConnectionType),
                ndExternalConnection.Attributes["Type"].Value);
        }

        public void Insert()
        {
            if (this.ExternalConnectionID != -1)
            {
                throw new Exception("Connection is already in database");
            }

            ExternalConnectionManager.INSTANCE.AddConnection(this);
        }

        public void Refresh()
        {
            if (this.ExternalConnectionID == -1)
            {
                throw new Exception("Connection is not in database");
            }

            ExternalConnectionManager.INSTANCE.RefreshConnection(this);
            ExternalConnectionManager.INSTANCE.GetConnection(this.ExternalConnectionID);
        }

        public string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            this.ToXML(new XmlTextWriter(stringWriter));
            return stringWriter.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ExternalConnection");
            xmlWriter.WriteAttributeString("Type", this.Type.ToString());
            this.WriteConnectionXML(xmlWriter);
            xmlWriter.WriteRaw((this.Credentials == null
                ? Metalogix.Permissions.Credentials.DefaultCredentials.ToXML()
                : this.Credentials.ToXML()));
            xmlWriter.WriteEndElement();
        }

        public void Update()
        {
            if (this.ExternalConnectionID == -1)
            {
                throw new Exception("Connection is not in database");
            }

            ExternalConnectionManager.INSTANCE.UpdateConnection(this);
        }

        protected virtual void WriteConnectionXML(XmlWriter xmlWriter)
        {
        }
    }
}