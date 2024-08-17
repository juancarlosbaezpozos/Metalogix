using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.DB
{
    public class SPListTemplateResourceManager
    {
        private TemplateResourceLocation m_resourceLocation = TemplateResourceLocation.EmbeddedWithinAssembly;

        private bool m_bSPS2003DBMode;

        private string[] _manifestResourceNames;

        private List<string> m_templates = new List<string>();

        public string BaseTemplateLocation { get; private set; }

        public string[] ManifestResourceNames
        {
            get
            {
                if (this._manifestResourceNames == null)
                {
                    this._manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                }

                return this._manifestResourceNames;
            }
        }

        public string OnetXMLLocation
        {
            get
            {
                if (this.SharePointVersion.IsSharePoint2003)
                {
                    return "STS\\XML\\ONET.XML";
                }

                return "GLOBAL\\XML\\ONET.XML";
            }
        }

        public TemplateResourceLocation ResourceLocation
        {
            get { return this.m_resourceLocation; }
        }

        public Metalogix.SharePoint.Adapters.SharePointVersion SharePointVersion { get; private set; }

        public bool SPS2003DBMode
        {
            get { return this.m_bSPS2003DBMode; }
        }

        public string[] Templates
        {
            get { return this.m_templates.ToArray(); }
        }

        public SPListTemplateResourceManager(TemplateResourceLocation location,
            Metalogix.SharePoint.Adapters.SharePointVersion sharePointVersion, bool bSPS2003DB,
            string sCustomTemplatePath)
        {
            this.m_resourceLocation = location;
            this.SharePointVersion = sharePointVersion;
            this.m_bSPS2003DBMode = (!sharePointVersion.IsSharePoint2003 ? false : bSPS2003DB);
            DirectoryInfo directoryInfo = null;
            if (this.m_resourceLocation == TemplateResourceLocation.CustomFileLocation &&
                !string.IsNullOrEmpty(sCustomTemplatePath))
            {
                directoryInfo = new DirectoryInfo(sCustomTemplatePath);
            }

            if (directoryInfo == null || !directoryInfo.Exists)
            {
                if (this.m_resourceLocation == TemplateResourceLocation.SharePointDirectory)
                {
                    this.LoadTemplatesFromSharePoint();
                    return;
                }

                this.LoadTemplatesFromAssembly();
                return;
            }

            this.BaseTemplateLocation = string.Concat(directoryInfo.FullName, "\\");
            this.LoadTemplatesFromSharePointDirectory(directoryInfo, "");
        }

        private Stream GetCaseInsensitiveManifestResourceStream(Assembly assembly, string assemblyResource)
        {
            string str = this.ManifestResourceNames.FirstOrDefault<string>((string fileName) =>
                fileName.Equals(assemblyResource, StringComparison.InvariantCultureIgnoreCase));
            return assembly.GetManifestResourceStream(str);
        }

        private string GetCorrectTemplateNameFromCollection(string[] templateNames, string locationPrefix)
        {
            if ((int)templateNames.Length <= 1 || string.IsNullOrEmpty(locationPrefix))
            {
                return templateNames[0];
            }

            return templateNames.FirstOrDefault<string>((string template) =>
                template.StartsWith(locationPrefix, StringComparison.InvariantCultureIgnoreCase));
        }

        public SPListTemplateCollection GetListTemplates()
        {
            return new SPListTemplateCollection(this);
        }

        public XmlDocument GetTemplate(string sTemplateName)
        {
            Stream stream = null;
            XmlDocument result;
            try
            {
                if (this.ResourceLocation == TemplateResourceLocation.EmbeddedWithinAssembly)
                {
                    string text = this.BaseTemplateLocation + sTemplateName.Replace("\\", ".");
                    Assembly executingAssembly = Assembly.GetExecutingAssembly();
                    stream = executingAssembly.GetManifestResourceStream(text);
                    if (stream == null)
                    {
                        stream = this.GetCaseInsensitiveManifestResourceStream(executingAssembly, text);
                        if (stream == null)
                        {
                            result = null;
                            return result;
                        }
                    }

                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string text2 = streamReader.ReadToEnd();
                        if (this.SharePointVersion.IsSharePoint2013OrLater &&
                            sTemplateName.EndsWith("SPSMSITE\\XML\\onet.xml"))
                        {
                            text2 = text2.Replace("CustomMasterUrl=MasterUrl=", "CustomMasterUrl=");
                        }

                        text2 = text2.Replace("<%=", "");
                        text2 = text2.Replace("%>", "");
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(text2);
                        result = xmlDocument;
                        return result;
                    }
                }

                string text3 = this.BaseTemplateLocation + sTemplateName;
                if (!File.Exists(text3))
                {
                    throw new Exception("The resource file: '" + text3 + "' does not exist");
                }

                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.Load(text3);
                result = xmlDocument2;
            }
            catch (Exception innerException)
            {
                throw new Exception("Cannot load template: '" + sTemplateName + "'", innerException);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return result;
        }

        public string GetTemplateName(string regexPattern, string locationPrefix = null)
        {
            return this.GetCorrectTemplateNameFromCollection(this.GetTemplateNames(regexPattern), locationPrefix);
        }

        public string[] GetTemplateNames(string sRegexPattern)
        {
            List<string> strs = new List<string>();
            foreach (string mTemplate in this.m_templates)
            {
                if (!Regex.IsMatch(mTemplate, sRegexPattern, RegexOptions.IgnoreCase))
                {
                    continue;
                }

                if (!mTemplate.StartsWith("GLOBAL"))
                {
                    strs.Add(mTemplate);
                }
                else
                {
                    strs.Insert(0, mTemplate);
                }
            }

            return strs.ToArray();
        }

        private void LoadTemplatesFromAssembly()
        {
            string str;
            this.BaseTemplateLocation = "Metalogix.SharePoint.Adapters.DB.Template._";
            if (this.SharePointVersion.IsSharePoint2003)
            {
                str = (this.m_bSPS2003DBMode ? "60SPS." : "60.");
            }
            else if (!this.SharePointVersion.IsSharePoint2007)
            {
                str = (!this.SharePointVersion.IsSharePoint2010 ? "15.Template." : "14.Template.");
            }
            else
            {
                str = "12.Template.";
            }

            SPListTemplateResourceManager sPListTemplateResourceManager = this;
            sPListTemplateResourceManager.BaseTemplateLocation =
                string.Concat(sPListTemplateResourceManager.BaseTemplateLocation, str);
            string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            for (int i = 0; i < (int)manifestResourceNames.Length; i++)
            {
                string str1 = manifestResourceNames[i];
                if (str1.StartsWith(this.BaseTemplateLocation))
                {
                    string str2 = str1.Substring(this.BaseTemplateLocation.Length);
                    int num = str2.LastIndexOf(".", str2.LastIndexOf(".") - 1);
                    string str3 = str2.Substring(num + 1);
                    string str4 = str2.Substring(0, num + 1);
                    string str5 = string.Concat(str4.Replace(".", "\\"), str3);
                    this.m_templates.Add(str5);
                }
            }
        }

        private void LoadTemplatesFromSharePoint()
        {
            string sharePointIsapiFolderFromRegistry = Utils.GetSharePointIsapiFolderFromRegistry();
            sharePointIsapiFolderFromRegistry = sharePointIsapiFolderFromRegistry.Trim(new char[] { '\\' });
            int num = sharePointIsapiFolderFromRegistry.LastIndexOf("\\");
            sharePointIsapiFolderFromRegistry =
                string.Concat(sharePointIsapiFolderFromRegistry.Substring(0, num), "\\TEMPLATE\\");
            DirectoryInfo directoryInfo = new DirectoryInfo(sharePointIsapiFolderFromRegistry);
            this.BaseTemplateLocation = string.Concat(directoryInfo.FullName, "\\");
            this.LoadTemplatesFromSharePointDirectory(directoryInfo, "");
        }

        private void LoadTemplatesFromSharePointDirectory(DirectoryInfo dir, string sRelativePath)
        {
            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles("*.xml");
            for (int i = 0; i < (int)files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                string str = string.Concat(sRelativePath, fileInfo.Name);
                this.m_templates.Add(str);
            }

            DirectoryInfo[] directories = dir.GetDirectories();
            for (int j = 0; j < (int)directories.Length; j++)
            {
                DirectoryInfo directoryInfo = directories[j];
                this.LoadTemplatesFromSharePointDirectory(directoryInfo,
                    string.Concat(sRelativePath, directoryInfo.Name, "\\"));
            }
        }
    }
}