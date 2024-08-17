using Metalogix.SharePoint.Adapters;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.DB
{
    public class ContentTypeResourceManager
    {
        private static Dictionary<string, string> s_contentTypeFilesSharePoint;

        private static Dictionary<string, string> s_siteColumnFilesSharePoint;

        private static Dictionary<string, string> s_contentTypeFilesAssembly;

        private static Dictionary<string, string> s_siteColumnFilesAssembly;

        private Dictionary<string, string> m_siteColumnFilesCustom;

        private Dictionary<string, string> m_contentTypeFilesCustom;

        private readonly Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion =
            new Metalogix.SharePoint.Adapters.SharePointVersion();

        private readonly TemplateResourceLocation m_resourceLocation = TemplateResourceLocation.EmbeddedWithinAssembly;

        private Dictionary<string, string> ContentTypeDictionary
        {
            get
            {
                if (this.ResourceLocation == TemplateResourceLocation.CustomFileLocation)
                {
                    return this.m_contentTypeFilesCustom;
                }

                if (this.m_resourceLocation != TemplateResourceLocation.EmbeddedWithinAssembly)
                {
                    return ContentTypeResourceManager.s_contentTypeFilesSharePoint;
                }

                return ContentTypeResourceManager.s_contentTypeFilesAssembly;
            }
        }

        public TemplateResourceLocation ResourceLocation
        {
            get { return this.m_resourceLocation; }
        }

        public Metalogix.SharePoint.Adapters.SharePointVersion SharePointVersion
        {
            get { return this.m_SharePointVersion; }
        }

        private Dictionary<string, string> SiteColumnDictionary
        {
            get
            {
                if (this.ResourceLocation == TemplateResourceLocation.CustomFileLocation)
                {
                    return this.m_siteColumnFilesCustom;
                }

                if (this.m_resourceLocation != TemplateResourceLocation.EmbeddedWithinAssembly)
                {
                    return ContentTypeResourceManager.s_siteColumnFilesSharePoint;
                }

                return ContentTypeResourceManager.s_siteColumnFilesAssembly;
            }
        }

        static ContentTypeResourceManager()
        {
        }

        public ContentTypeResourceManager(TemplateResourceLocation location,
            Metalogix.SharePoint.Adapters.SharePointVersion sharePointVersion, string sCustomTemplatePath)
        {
            this.m_SharePointVersion = sharePointVersion;
            this.m_resourceLocation = location;
            if (this.m_resourceLocation == TemplateResourceLocation.EmbeddedWithinAssembly)
            {
                if (ContentTypeResourceManager.s_siteColumnFilesAssembly == null)
                {
                    ContentTypeResourceManager.s_siteColumnFilesAssembly = new Dictionary<string, string>();
                    ContentTypeResourceManager.LoadSiteColumnsFromAssembly();
                }

                if (ContentTypeResourceManager.s_contentTypeFilesAssembly == null)
                {
                    ContentTypeResourceManager.s_contentTypeFilesAssembly = new Dictionary<string, string>();
                    ContentTypeResourceManager.LoadContentTypesFromAssembly();
                    return;
                }
            }
            else if (this.m_resourceLocation != TemplateResourceLocation.SharePointDirectory)
            {
                this.m_siteColumnFilesCustom = new Dictionary<string, string>();
                this.m_contentTypeFilesCustom = new Dictionary<string, string>();
                ContentTypeResourceManager.LoadSiteColumnsFromCustom(this.m_siteColumnFilesCustom, sCustomTemplatePath);
                ContentTypeResourceManager.LoadContentTypesFromCustom(this.m_contentTypeFilesCustom,
                    sCustomTemplatePath);
            }
            else
            {
                if (ContentTypeResourceManager.s_siteColumnFilesSharePoint == null)
                {
                    ContentTypeResourceManager.s_siteColumnFilesSharePoint = new Dictionary<string, string>();
                    ContentTypeResourceManager.LoadSiteColumnsFromSharePoint();
                }

                if (ContentTypeResourceManager.s_contentTypeFilesSharePoint == null)
                {
                    ContentTypeResourceManager.s_contentTypeFilesSharePoint = new Dictionary<string, string>();
                    ContentTypeResourceManager.LoadContentTypesFromSharePoint();
                    return;
                }
            }
        }

        private static string CleanLocalizationRefs(string sSourceString)
        {
            for (int i = sSourceString.IndexOf("$Resources:"); i >= 0; i = sSourceString.IndexOf("$Resources:"))
            {
                sSourceString = sSourceString.Remove(i, 11);
            }

            return sSourceString;
        }

        public XmlNode GetContentType(string sContentTypeId)
        {
            sContentTypeId = sContentTypeId.ToUpper();
            if (!this.ContentTypeDictionary.ContainsKey(sContentTypeId))
            {
                return null;
            }

            XmlDocument xmlDocument = new XmlDocument();
            string str = "";
            List<XmlNode> xmlNodes = new List<XmlNode>();
            string str1 = sContentTypeId;
            for (int i = 0; i < str1.Length; i++)
            {
                char chr = str1[i];
                str = string.Concat(str, chr);
                if (this.ContentTypeDictionary.ContainsKey(str))
                {
                    xmlDocument.LoadXml(this.ContentTypeDictionary[str]);
                    XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("//FieldRefs/FieldRef");
                    if (xmlNodeLists != null)
                    {
                        foreach (XmlNode xmlNodes1 in xmlNodeLists)
                        {
                            xmlNodes.Add(xmlNodes1);
                        }
                    }

                    XmlNodeList xmlNodeLists1 = xmlDocument.SelectNodes("//FieldRefs/RemoveFieldRef");
                    if (xmlNodeLists1 != null)
                    {
                        foreach (XmlNode xmlNodes2 in xmlNodeLists1)
                        {
                            XmlNode xmlNodes3 = null;
                            if (xmlNodes2.Attributes["ID"] == null)
                            {
                                continue;
                            }

                            string upper = xmlNodes2.Attributes["ID"].Value.ToUpper();
                            foreach (XmlNode xmlNode in xmlNodes)
                            {
                                if (xmlNode.Attributes["ID"] == null ||
                                    !(xmlNode.Attributes["ID"].Value.ToUpper() == upper))
                                {
                                    continue;
                                }

                                xmlNodes3 = xmlNode;
                            }

                            if (xmlNodes3 == null)
                            {
                                continue;
                            }

                            xmlNodes.Remove(xmlNodes3);
                        }
                    }
                }
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(256));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("ContentType");
            xmlTextWriter.WriteAttributeString("ID", sContentTypeId);
            XmlNode documentElement = xmlDocument.DocumentElement;
            if (documentElement.Attributes["Group"] != null)
            {
                xmlTextWriter.WriteAttributeString("Group", documentElement.Attributes["Group"].Value);
            }

            if (documentElement.Attributes["Description"] != null)
            {
                xmlTextWriter.WriteAttributeString("Description", documentElement.Attributes["Description"].Value);
            }

            xmlTextWriter.WriteStartElement("FieldRefs");
            foreach (XmlNode xmlNode1 in xmlNodes)
            {
                xmlTextWriter.WriteRaw(xmlNode1.OuterXml);
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlDocument.LoadXml(stringWriter.ToString());
            return xmlDocument.DocumentElement;
        }

        private static string GetSharePointLocation()
        {
            string str;
            string str1 = null;
            str = (!Utils.SystemIs64Bit
                ? "SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint"
                : "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint");
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(str);
            if (registryKey == null)
            {
                str = "SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\AssemblyFolders\\SharePoint16.0";
                registryKey = Registry.LocalMachine.OpenSubKey(str);
            }

            if (registryKey != null)
            {
                str1 = registryKey.GetValue("").ToString();
                str1 = str1.Trim(new char[] { '\\' });
                int num = str1.LastIndexOf("\\");
                str1 = string.Concat(str1.Substring(0, num), "\\TEMPLATE\\FEATURES\\");
            }

            return str1;
        }

        public XmlNode GetSiteColumnNode(Guid columnGuid)
        {
            return this.GetSiteColumnNode(string.Concat("{", columnGuid.ToString(), "}"));
        }

        public XmlNode GetSiteColumnNode(string sColumnGuid)
        {
            sColumnGuid = sColumnGuid.ToUpper();
            if (!this.SiteColumnDictionary.ContainsKey(sColumnGuid))
            {
                return null;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(this.SiteColumnDictionary[sColumnGuid]);
            return xmlDocument.DocumentElement;
        }

        private static void LoadContentTypesFromAssembly()
        {
            ContentTypeResourceManager.LoadFromAssembly(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(ContentTypeResourceManager
                    .SetContentTypesFromXML), ContentTypeResourceManager.s_contentTypeFilesAssembly);
        }

        private static void LoadContentTypesFromCustom(Dictionary<string, string> dictionary,
            string sCustomTemplatePath)
        {
            ContentTypeResourceManager.LoadFromFileLocation(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(ContentTypeResourceManager
                    .SetContentTypesFromXML), dictionary, sCustomTemplatePath);
        }

        private static void LoadContentTypesFromSharePoint()
        {
            ContentTypeResourceManager.LoadFromSharePoint(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(ContentTypeResourceManager
                    .SetContentTypesFromXML), ContentTypeResourceManager.s_contentTypeFilesSharePoint);
        }

        private static void LoadFromAssembly(ContentTypeResourceManager.ConfigureFromXMLDelegate configureDelegate,
            Dictionary<string, string> dictionary)
        {
            string str = "Metalogix.SharePoint.Adapters.DB.Template._12.Template.FEATURES.ContentTypesAndSiteColumns";
            string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            for (int i = 0; i < (int)manifestResourceNames.Length; i++)
            {
                string str1 = manifestResourceNames[i];
                if (str1.StartsWith(str))
                {
                    Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str1);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(manifestResourceStream);
                    configureDelegate(dictionary, xmlDocument.DocumentElement);
                }
            }
        }

        // Metalogix.SharePoint.Adapters.DB.ContentTypeResourceManager
        private static void LoadFromFile(ContentTypeResourceManager.ConfigureFromXMLDelegate configureDelegate,
            Dictionary<string, string> dictionary, DirectoryInfo dirInfo)
        {
            DirectoryInfo[] directories = dirInfo.GetDirectories();
            int i = 0;
            while (i < directories.Length)
            {
                DirectoryInfo directoryInfo = directories[i];
                string text = directoryInfo.FullName + "\\Feature.xml";
                if (!File.Exists(text))
                {
                    goto IL_FC;
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(text);
                XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                xmlNamespaceManager.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/");
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//sp:ElementManifests/sp:ElementManifest[@Location]",
                    xmlNamespaceManager);
                if (xmlNodeList.Count != 0)
                {
                    int arg_7C_0 = xmlNodeList.Count;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        string filename = directoryInfo.FullName + "\\" + xmlNode.Attributes["Location"].Value;
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.Load(filename);
                        configureDelegate(dictionary, xmlDocument2.DocumentElement);
                    }

                    goto IL_FC;
                }

                IL_104:
                i++;
                continue;
                IL_FC:
                ContentTypeResourceManager.LoadFromFile(configureDelegate, dictionary, directoryInfo);
                goto IL_104;
            }
        }

        private static void LoadFromFileLocation(ContentTypeResourceManager.ConfigureFromXMLDelegate configureDelegate,
            Dictionary<string, string> dictionary, string sCustomTemplatePath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(sCustomTemplatePath);
            if (!directoryInfo.Exists)
            {
                ContentTypeResourceManager.LoadFromAssembly(configureDelegate, dictionary);
                return;
            }

            ContentTypeResourceManager.LoadFromFile(configureDelegate, dictionary, directoryInfo);
        }

        private static void LoadFromSharePoint(ContentTypeResourceManager.ConfigureFromXMLDelegate configureDelegate,
            Dictionary<string, string> dictionary)
        {
            ContentTypeResourceManager.LoadFromFile(configureDelegate, dictionary,
                new DirectoryInfo(ContentTypeResourceManager.GetSharePointLocation()));
        }

        private static void LoadSiteColumnsFromAssembly()
        {
            ContentTypeResourceManager.LoadFromAssembly(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(
                    ContentTypeResourceManager.SetSiteColumnsFromXML),
                ContentTypeResourceManager.s_siteColumnFilesAssembly);
        }

        private static void LoadSiteColumnsFromCustom(Dictionary<string, string> dictionary, string sCustomTemplatePath)
        {
            ContentTypeResourceManager.LoadFromFileLocation(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(
                    ContentTypeResourceManager.SetSiteColumnsFromXML), dictionary, sCustomTemplatePath);
        }

        private static void LoadSiteColumnsFromSharePoint()
        {
            ContentTypeResourceManager.LoadFromSharePoint(
                new ContentTypeResourceManager.ConfigureFromXMLDelegate(
                    ContentTypeResourceManager.SetSiteColumnsFromXML),
                ContentTypeResourceManager.s_siteColumnFilesSharePoint);
        }

        private static void SetContentTypesFromXML(Dictionary<string, string> ContentTypeDictionary, XmlNode xmlNode)
        {
            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlNode.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/");
            foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//sp:ContentType[@ID]", xmlNamespaceManagers))
            {
                try
                {
                    string value = xmlNodes.Attributes["ID"].Value;
                    if (!ContentTypeDictionary.ContainsKey(value))
                    {
                        StringWriter stringWriter = new StringWriter(new StringBuilder(256));
                        XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                        xmlTextWriter.WriteStartElement("ContentType");
                        if (xmlNodes.Attributes["Group"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Group",
                                ContentTypeResourceManager.CleanLocalizationRefs(xmlNodes.Attributes["Group"].Value));
                        }

                        if (xmlNodes.Attributes["Description"] != null)
                        {
                            xmlTextWriter.WriteAttributeString("Description",
                                ContentTypeResourceManager.CleanLocalizationRefs(xmlNodes.Attributes["Description"]
                                    .Value));
                        }

                        string outerXml = xmlNodes.SelectSingleNode(".//sp:FieldRefs", xmlNamespaceManagers).OuterXml;
                        int num = outerXml.IndexOf("xmlns");
                        int num1 = outerXml.IndexOf("\"", num);
                        num1 = outerXml.IndexOf("\"", num1 + 1);
                        outerXml = outerXml.Remove(num, num1 + 1 - num);
                        xmlTextWriter.WriteRaw(outerXml);
                        xmlTextWriter.WriteEndElement();
                        ContentTypeDictionary.Add(value.ToUpper(), stringWriter.ToString());
                    }
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                }
            }
        }

        private static void SetSiteColumnsFromXML(Dictionary<string, string> SiteColumnDictionary, XmlNode xmlNode)
        {
            XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlNode.OwnerDocument.NameTable);
            xmlNamespaceManagers.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/");
            foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//sp:Field[@ID]", xmlNamespaceManagers))
            {
                string upper = xmlNodes.Attributes["ID"].Value.ToUpper();
                if (SiteColumnDictionary.ContainsKey(upper))
                {
                    continue;
                }

                StringWriter stringWriter = new StringWriter(new StringBuilder(256));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                string value = xmlNodes.Attributes["Type"].Value;
                xmlTextWriter.WriteStartElement("Field");
                xmlTextWriter.WriteAttributeString("Name", xmlNodes.Attributes["Name"].Value);
                xmlTextWriter.WriteAttributeString("Type", xmlNodes.Attributes["Type"].Value);
                xmlTextWriter.WriteAttributeString("ID", upper);
                xmlTextWriter.WriteAttributeString("IsFromFeature", "True");
                if (xmlNodes.Attributes["ColName"] != null)
                {
                    xmlTextWriter.WriteAttributeString("ColName", xmlNodes.Attributes["ColName"].Value);
                }

                if (xmlNodes.Attributes["Hidden"] != null)
                {
                    xmlTextWriter.WriteAttributeString("Hidden", xmlNodes.Attributes["Hidden"].Value);
                }

                if (xmlNodes.Attributes["ReadOnly"] != null)
                {
                    xmlTextWriter.WriteAttributeString("ReadOnly", xmlNodes.Attributes["ReadOnly"].Value);
                }

                if (xmlNodes.Attributes["Required"] != null)
                {
                    xmlTextWriter.WriteAttributeString("Required", xmlNodes.Attributes["Required"].Value);
                }

                string str = XmlConvert.DecodeName(xmlNodes.Attributes["Name"].Value);
                xmlTextWriter.WriteAttributeString("DisplayName", str);
                xmlTextWriter.WriteEndElement();
                SiteColumnDictionary.Add(upper, stringWriter.ToString());
            }
        }

        private delegate void ConfigureFromXMLDelegate(Dictionary<string, string> dictionary, XmlNode xmlNode);
    }
}