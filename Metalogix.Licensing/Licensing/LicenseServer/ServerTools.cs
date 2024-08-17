using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.Licensing.LicenseServer
{
    internal static class ServerTools
    {
        private const string _SERVER_NS = "urn:LicenseServer";

        public static string GetSoapMessageDetail(Exception exception)
        {
            SoapException soapException = exception as SoapException;
            if (soapException == null || soapException.Detail == null || soapException.Detail.OwnerDocument == null)
            {
                return null;
            }

            XmlNamespaceManager xmlNamespaceManagers =
                new XmlNamespaceManager(soapException.Detail.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("ls", "urn:LicenseServer");
            XmlNode xmlNodes = soapException.Detail.SelectSingleNode("./ls:Message", xmlNamespaceManagers);
            if (xmlNodes == null)
            {
                return null;
            }

            return xmlNodes.InnerText;
        }

        public static bool IsSoapExceptionOfType(Exception ex, string type)
        {
            SoapException soapException = ex as SoapException;
            if (soapException == null)
            {
                return false;
            }

            return string.CompareOrdinal(soapException.Code.Name, type) == 0;
        }
    }
}