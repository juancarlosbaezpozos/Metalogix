using Metalogix.Permissions;
using Metalogix.Utilities;
using System;
using System.Net;
using System.Xml;

namespace Metalogix.Connectivity.Proxy
{
    public class MLProxy
    {
        protected bool _enabled;

        protected string _server = "";

        protected string _port = "";

        protected Metalogix.Permissions.Credentials _credentials;

        public Metalogix.Permissions.Credentials Credentials
        {
            get { return this._credentials; }
            set { this._credentials = value; }
        }

        public bool Enabled
        {
            get { return this._enabled; }
            set { this._enabled = value; }
        }

        public string Port
        {
            get { return this._port; }
            set { this._port = value; }
        }

        public string Server
        {
            get { return this._server; }
            set { this._server = value; }
        }

        public MLProxy()
        {
        }

        public MLProxy(XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                return;
            }

            this.Server = xmlNode.Attributes["Server"].Value;
            this.Port = xmlNode.Attributes["Port"].Value;
            this.Credentials = new Metalogix.Permissions.Credentials(xmlNode);
            this.Enabled = true;
        }

        public string GetUrl()
        {
            return string.Format("http://{0}:{1}", this.Server, this.Port);
        }

        public WebProxy GetWebProxy()
        {
            WebProxy webProxy = null;
            if (this.Enabled)
            {
                webProxy = new WebProxy(this.Server, int.Parse(this.Port));
                if (!this.Credentials.IsDefault)
                {
                    webProxy.Credentials = this.Credentials.NetworkCredentials;
                }
            }

            return webProxy;
        }

        public void SetUrl(string sUrl)
        {
            sUrl = sUrl.TrimEnd(new char[] { '/' });
            int num = sUrl.IndexOf("://", StringComparison.Ordinal);
            string str = (num >= 0 ? sUrl.Substring(num + 3) : sUrl);
            int num1 = str.IndexOf(":", StringComparison.Ordinal);
            if (num1 < 0)
            {
                this._server = str;
                this._port = "80";
                return;
            }

            this._server = str.Substring(0, num1);
            this._port = str.Substring(num1 + 1);
        }

        public override string ToString()
        {
            object[] objArray = new object[] { this._enabled, this._server, this._port, null };
            objArray[3] = (this._credentials.IsDefault
                ? "Default Credentials"
                : string.Format("User: {0}, Pass: {1}", this._credentials.UserName,
                    this._credentials.Password.ToInsecureString()));
            return string.Format("Enabled: {0}, Server: {1}, Port: {2}, {3}", objArray);
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            if (this.Enabled)
            {
                xmlWriter.WriteStartElement("Proxy");
                xmlWriter.WriteAttributeString("Server", this.Server);
                xmlWriter.WriteAttributeString("Port", this.Port);
                this.Credentials.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }
    }
}