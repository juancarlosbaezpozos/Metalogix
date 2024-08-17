using Metalogix.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class SystemInfo
    {
        private Version m_osVersion;

        private Version m_dotNetVersion;

        private Version m_extensionVersion;

        private string m_platform;

        private string m_servicepack;

        private string m_hostname;

        private bool? _hasTaxonomySupport = null;

        private bool m_bSystemInfoAvailable = true;

        private string m_strRep;

        private XmlNode m_xmlNode;

        private string m_xmlString;

        public Version DotNetVersion
        {
            get
            {
                if (this.m_dotNetVersion == null)
                {
                    this.m_dotNetVersion = Environment.Version;
                }

                return this.m_dotNetVersion;
            }
        }

        public Version ExtensionVersion
        {
            get
            {
                if (this.m_extensionVersion == null)
                {
                    this.m_extensionVersion = Assembly.GetExecutingAssembly().GetName().Version;
                }

                return this.m_extensionVersion;
            }
        }

        public bool HasTaxonomySupport
        {
            get
            {
                if (!this._hasTaxonomySupport.HasValue)
                {
                    this._hasTaxonomySupport = new bool?(Utils.HasTaxonomySupport());
                }

                return this._hasTaxonomySupport.Value;
            }
        }

        public string Hostname
        {
            get
            {
                if (this.m_hostname == null)
                {
                    this.m_hostname = Environment.MachineName.ToString();
                }

                return this.m_hostname;
            }
        }

        public Version OSVersion
        {
            get
            {
                if (this.m_osVersion == null)
                {
                    this.m_osVersion = Environment.OSVersion.Version;
                }

                return this.m_osVersion;
            }
        }

        public string Platform
        {
            get
            {
                string str;
                string str1;
                if (this.m_platform == null)
                {
                    OperatingSystem oSVersion = Environment.OSVersion;
                    str = (!Utils.SystemIs64Bit ? "32 bit" : "64 bit");
                    Architecture processorArchitectureNumber = Utils.ProcessorArchitectureNumber;
                    Architecture architecture = processorArchitectureNumber;
                    if (architecture == Architecture.PROCESSOR_ARCHITECTURE_X86)
                    {
                        str1 = "x86";
                    }
                    else if (architecture == Architecture.PROCESSOR_ARCHITECTURE_IA64)
                    {
                        str1 = "IA64";
                    }
                    else
                    {
                        str1 = (architecture != Architecture.PROCESSOR_ARCHITECTURE_AMD64
                            ? processorArchitectureNumber.ToString()
                            : "AMD64");
                    }

                    string[] strArrays = new string[] { oSVersion.Platform.ToString(), " (", str, ") [", str1, "]" };
                    this.m_platform = string.Concat(strArrays);
                }

                return this.m_platform;
            }
        }

        public string ServicePack
        {
            get
            {
                if (this.m_servicepack == null)
                {
                    this.m_servicepack = Environment.OSVersion.ServicePack.ToString();
                }

                return this.m_servicepack;
            }
        }

        public bool SystemInfoAvailable
        {
            get { return this.m_bSystemInfoAvailable; }
        }

        public SystemInfo()
        {
        }

        public SystemInfo(bool bSystemInfoAvailable)
        {
            if (!bSystemInfoAvailable)
            {
                this.InitializeAsEmpty();
            }
        }

        public SystemInfo(string xmlstring)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlstring);
            try
            {
                if (false.ToString() !=
                    XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/@system_info_available"))
                {
                    this.m_extensionVersion =
                        new Version(XmlUtility.GetAttributeValueFromDocument(xmlDocument,
                            "//server_info/@extension_assembly_version"));
                    this.m_osVersion =
                        new Version(XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/os/@version"));
                    this.m_dotNetVersion =
                        new Version(
                            XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/dotnet/@version"));
                    this.m_platform =
                        XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/os/@platform");
                    this.m_servicepack =
                        XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/os/@servicepack");
                    this.m_hostname =
                        XmlUtility.GetAttributeValueFromDocument(xmlDocument, "//server_info/os/@hostname");
                    bool flag = false;
                    bool.TryParse(
                        XmlUtility.GetAttributeValueFromDocument(xmlDocument,
                            "//server_info/SharePoint/@HasTaxonomySupport"), out flag);
                    this._hasTaxonomySupport = new bool?(flag);
                }
                else
                {
                    this.InitializeAsEmpty();
                }
            }
            catch
            {
            }
        }

        public SystemInfo Clone()
        {
            return new SystemInfo(this.ToXmlString());
        }

        private void InitializeAsEmpty()
        {
            this.m_bSystemInfoAvailable = false;
            this.m_osVersion = new Version();
            this.m_dotNetVersion = new Version();
            this.m_extensionVersion = new Version();
            this.m_platform = "";
            this.m_servicepack = "";
            this.m_hostname = "";
            this._hasTaxonomySupport = new bool?(false);
        }

        public override string ToString()
        {
            if (this.m_strRep == null)
            {
                if (!this.m_bSystemInfoAvailable)
                {
                    this.m_strRep = "";
                }
                else
                {
                    string[] hostname = new string[]
                    {
                        "Server Name: ", this.Hostname, "\r\nServer Operating System: ", this.Platform, ", ",
                        this.ServicePack, "\r\nMetalogix SharePoint Extensions Version: ",
                        this.ExtensionVersion.ToString()
                    };
                    this.m_strRep = string.Concat(hostname);
                }
            }

            return this.m_strRep;
        }

        public XmlNode ToXmlNode()
        {
            if (this.m_xmlNode == null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(this.ToXmlString());
                this.m_xmlNode = xmlDocument.FirstChild;
            }

            return this.m_xmlNode;
        }

        public string ToXmlString()
        {
            if (this.m_xmlString == null)
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("server_info");
                if (!this.m_bSystemInfoAvailable)
                {
                    xmlTextWriter.WriteAttributeString("system_info_available", false.ToString());
                }
                else
                {
                    xmlTextWriter.WriteAttributeString("extension_assembly_version", this.ExtensionVersion.ToString());
                    xmlTextWriter.WriteStartElement("os");
                    xmlTextWriter.WriteAttributeString("version", Environment.OSVersion.Version.ToString());
                    xmlTextWriter.WriteAttributeString("platform", this.Platform);
                    xmlTextWriter.WriteAttributeString("servicepack", this.ServicePack);
                    xmlTextWriter.WriteAttributeString("hostname", this.Hostname);
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("dotnet");
                    xmlTextWriter.WriteAttributeString("version", this.DotNetVersion.ToString());
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("SharePoint");
                    xmlTextWriter.WriteAttributeString("HasTaxonomySupport", this.HasTaxonomySupport.ToString());
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                this.m_xmlString = stringWriter.ToString();
            }

            return this.m_xmlString;
        }
    }
}