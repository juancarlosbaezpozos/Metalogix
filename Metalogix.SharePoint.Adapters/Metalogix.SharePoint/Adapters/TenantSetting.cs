using Metalogix;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class TenantSetting
    {
        private const string TENANT_SETTING_FILENAME = "TenantSettings.xml";

        public string AdminURL { get; private set; }

        public Dictionary<int, string> LanguageCollection { get; private set; }

        public Dictionary<string, bool> ManagedPathCollection { get; private set; }

        public string MySiteHostPath { get; private set; }

        public string MySiteManagedPath { get; private set; }

        public string MySitePersonalFormat { get; private set; }

        public TenantSetting(string adminURL)
        {
            this.LanguageCollection = new Dictionary<int, string>();
            this.ManagedPathCollection = new Dictionary<string, bool>();
            this.GetTenantSettings(adminURL);
        }

        private void FromXml(XmlNode node, string adminURL)
        {
            this.AdminURL = adminURL;
            this.MySiteHostPath = node.GetAttributeValueAsString("MysiteHostPath");
            if (string.IsNullOrEmpty(this.MySiteHostPath))
            {
                this.MySiteHostPath = this.GetMySiteHostURLFromAdminURL(adminURL);
            }

            this.MySiteManagedPath = node.GetAttributeValueAsString("MysiteManagedPath");
            if (string.IsNullOrEmpty(this.MySiteManagedPath))
            {
                throw new Exception("'MysiteHostPath' attribute value is not available in TenantSettings xml");
            }

            this.MySitePersonalFormat = node.GetAttributeValueAsString("MySitePersonalFormat");
            if (string.IsNullOrEmpty(this.MySitePersonalFormat))
            {
                throw new Exception("'MySitePersonalFormat' attribute value is not available in TenantSettings xml");
            }

            foreach (XmlNode xmlNodes in node.SelectNodes("LanguageCollection/Language"))
            {
                if (xmlNodes.Attributes["Name"] == null || xmlNodes.Attributes["LCID"] == null ||
                    this.LanguageCollection.ContainsKey(Convert.ToInt32(xmlNodes.Attributes["LCID"].Value)))
                {
                    continue;
                }

                this.LanguageCollection.Add(Convert.ToInt32(xmlNodes.Attributes["LCID"].Value),
                    xmlNodes.Attributes["Name"].Value);
            }

            foreach (XmlNode xmlNodes1 in node.SelectNodes("ManagedPathCollection/Path"))
            {
                if (string.IsNullOrEmpty(xmlNodes1.InnerText) || xmlNodes1.Attributes["IsWildcard"] == null ||
                    this.ManagedPathCollection.ContainsKey(xmlNodes1.InnerText))
                {
                    continue;
                }

                this.ManagedPathCollection.Add(xmlNodes1.InnerText,
                    bool.Parse(xmlNodes1.Attributes["IsWildcard"].Value));
            }
        }

        private string GetMySiteHostURLFromAdminURL(string adminURL)
        {
            return Regex.Replace(adminURL, "-admin", "-my", RegexOptions.IgnoreCase);
        }

        private void GetTenantSettings(string adminURL)
        {
            string str = Path.Combine(ApplicationData.CommonDataPath, "TenantSettings.xml");
            string empty = string.Empty;
            if (!File.Exists(str))
            {
                empty = SystemUtils.GetResourceStringFromAssembly("TenantSettings.xml");
                File.WriteAllText(str, empty);
            }
            else
            {
                empty = File.ReadAllText(str);
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(empty);
            string str1 = "/TenantSettings/TenantSetting";
            XmlNode xmlNodes = xmlDocument.SelectSingleNodeWithIgnoreCase(str1, "AdminUrl", adminURL) ??
                               xmlDocument.SelectSingleNodeWithIgnoreCase(str1, "AdminUrl", "default");
            if (xmlNodes == null)
            {
                empty = SystemUtils.GetResourceStringFromAssembly("TenantSettings.xml");
                xmlDocument.LoadXml(empty);
                xmlNodes = xmlDocument.SelectSingleNodeWithIgnoreCase(str1, "AdminUrl", "default");
            }

            this.FromXml(xmlNodes, adminURL);
        }
    }
}