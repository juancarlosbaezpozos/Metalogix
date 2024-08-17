using Metalogix.Data;
using System;
using System.Text;
using System.Xml;

namespace Metalogix.Permissions
{
    public class AzureAdGraphCredentials : IXmlable
    {
        private readonly string _appClientId;

        private readonly string _appSecret;

        public string AppClientId
        {
            get { return this._appClientId; }
        }

        public string AppSecret
        {
            get { return this._appSecret; }
        }

        public AzureAdGraphCredentials(string appClientId, string appSecret)
        {
            this._appClientId = appClientId;
            this._appSecret = appSecret;
        }

        public AzureAdGraphCredentials(XmlNode xmlCredentials)
        {
            string value;
            string str;
            if (xmlCredentials.Attributes["AzureAdGraphAppClientId"] == null)
            {
                value = null;
            }
            else
            {
                value = xmlCredentials.Attributes["AzureAdGraphAppClientId"].Value;
            }

            this._appClientId = value;
            if (xmlCredentials.Attributes["AzureAdGraphAppSecret"] == null)
            {
                str = null;
            }
            else
            {
                str = xmlCredentials.Attributes["AzureAdGraphAppSecret"].Value;
            }

            this._appSecret = str;
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
            {
                xmlWriter.WriteStartElement("AzureAdGraphCredentials");
                this.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }

            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            if (this.AppClientId != null && this.AppSecret != null)
            {
                xmlWriter.WriteAttributeString("AzureAdGraphAppClientId", this.AppClientId);
                xmlWriter.WriteAttributeString("AzureAdGraphAppSecret", this.AppSecret);
            }
        }
    }
}