using Metalogix.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public static class WebPartUtils
    {
        public static bool HasEmbeddedWikiField(string sPageContents)
        {
            return (new Regex("<\\s*SharePoint:EmbeddedFormField[^>]*?id\\s*=\\s*\"WikiField\"",
                RegexOptions.IgnoreCase)).Match(sPageContents).Success;
        }

        public static List<string> ParseWebPartPageForZones(string pageContents)
        {
            List<string> strs = new List<string>();
            if (!string.IsNullOrEmpty(pageContents))
            {
                string str = "ZoneName";
                Regex regex =
                    new Regex(
                        string.Concat("<\\s*WebPartPages:WebPartZone([\\s\\S]*?)[(id)]\\s*=\\s*['\"](?<", str,
                            ">[^'\"]*)['\\\"]"), RegexOptions.IgnoreCase);
                MatchCollection matchCollections = regex.Matches(pageContents);
                strs.AddRange((
                    from Match zoneMatch in matchCollections
                    select zoneMatch.Groups[str]).Where<Group>((Group zone) =>
                {
                    if (!zone.Success)
                    {
                        return false;
                    }

                    return zone.Value.Trim().Length > 0;
                }).Select<Group, string>((Group zone) => zone.Value.Trim()));
            }

            return strs;
        }

        public static void ParseWebPartPageToXml(XmlWriter writer, string sPageContents)
        {
            List<string> strs = WebPartUtils.ParseWebPartPageForZones(sPageContents);
            writer.WriteElementString("ZonesFromAspx", string.Join(",", strs.ToArray()));
            writer.WriteElementString("HasEmbeddedWikiField",
                (WebPartUtils.HasEmbeddedWikiField(sPageContents) ? true.ToString() : false.ToString()));
        }

        public static string RetrieveWebPartXmlUsingWebRequest(WebProxy adapterProxy, ICredentials networkCredentials,
            X509CertificateWrapperCollection includedCertificates, string siteUrl, string pageUrl, string webPartGuid)
        {
            string[] strArrays = new string[]
                { siteUrl, "/_vti_bin/exportwp.aspx?pageurl=", pageUrl, "&guidstring=", webPartGuid };
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Concat(strArrays));
            httpWebRequest.Credentials = networkCredentials;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            if (adapterProxy != null)
            {
                httpWebRequest.Proxy = adapterProxy;
            }

            if (includedCertificates != null)
            {
                includedCertificates.CopyCertificatesToCollection(httpWebRequest.ClientCertificates);
            }

            StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream());
            string end = streamReader.ReadToEnd();
            streamReader.Dispose();
            return end;
        }
    }
}