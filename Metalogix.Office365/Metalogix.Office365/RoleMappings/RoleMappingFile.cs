using Metalogix;
using Metalogix.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.Office365.RoleMappings
{
    public class RoleMappingFile
    {
        private readonly static string RoleMappingFilesPath;

        private readonly static object RoleMappingFileLock;

        private static volatile XmlDocument _xmlDoc;

        public static XmlDocument XmlDoc
        {
            get
            {
                if (RoleMappingFile._xmlDoc == null)
                {
                    try
                    {
                        lock (RoleMappingFile.RoleMappingFileLock)
                        {
                            if (!File.Exists(RoleMappingFile.RoleMappingFilesPath))
                            {
                                RoleMappingFile.CreateRoleMappingFile();
                            }

                            RoleMappingFile._xmlDoc = new XmlDocument();
                            RoleMappingFile._xmlDoc.Load(RoleMappingFile.RoleMappingFilesPath);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                            "An error occurred while loading xml file 'RoleMapping.xml'", true);
                    }
                }

                return RoleMappingFile._xmlDoc;
            }
        }

        static RoleMappingFile()
        {
            RoleMappingFile.RoleMappingFilesPath = Path.Combine(ApplicationData.CommonDataPath, "RoleMapping.xml");
            RoleMappingFile.RoleMappingFileLock = new object();
        }

        public RoleMappingFile()
        {
        }

        private static void CreateRoleMappingFile()
        {
            List<RoleMapping> roleMappings = new List<RoleMapping>();
            List<RoleTemplateMapping> roleTemplateMappings = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate(
                    "BICenterSite#0,BDR#0,SRCHCEN#0,PRODUCTCATALOG#0,CMSPUBLISHING#0,BLANKINTERNET#0,BLANKINTERNET#2,WIKI#0",
                    "Approve", "$Resources:cmscore,RoleNameApprover", "$Resources:cmscore,RoleDescriptionApprover"),
                RoleMappingFile.GetTemplate("Default", "View Only", "$Resources:xlsrv,RoleNameViewer;",
                    "$Resources:xlsrv,RoleDescriptionViewers;")
            };
            roleMappings.Add(new RoleMapping("1073741924", "2147483647", "0", roleTemplateMappings));
            List<RoleTemplateMapping> roleTemplateMappings1 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("COMMUNITY#0", "Moderate", "Moderate",
                    "Can view, add, update, delete and moderate list items and documents"),
                RoleMappingFile.GetTemplate("OFFILE#0,OFFILE#1", "Records Center Web Service Submitters",
                    "Records Center Web Service Submitters", "Submit content to this site using Web Services."),
                RoleMappingFile.GetTemplate("Default", "Manage Hierarchy",
                    "$Resources:cmscore,RoleNameHierarchyManager", "$Resources:cmscore,RoleDescriptionHierarchyManager")
            };
            roleMappings.Add(new RoleMapping("1073741925", "2147483647", "0", roleTemplateMappings1));
            List<RoleTemplateMapping> roleTemplateMappings2 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Restricted Read", "$Resources:cmscore,RoleNameViewer",
                    "$Resources:cmscore,RoleDescriptionViewer")
            };
            roleMappings.Add(new RoleMapping("1073741926", "2147483647", "0", roleTemplateMappings2));
            List<RoleTemplateMapping> roleTemplateMappings3 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Full Control", "$Resources:fpext,0x001C003Bu",
                    "$Resources:fpext,0x001C0041u")
            };
            roleMappings.Add(new RoleMapping("1073741829", "1", "5", roleTemplateMappings3));
            List<RoleTemplateMapping> roleTemplateMappings4 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Design", "$Resources:fpext,0x001C003Cu",
                    "$Resources:fpext,0x001C0042u")
            };
            roleMappings.Add(new RoleMapping("1073741828", "32", "4", roleTemplateMappings4));
            List<RoleTemplateMapping> roleTemplateMappings5 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Contribute", "$Resources:fpext,0x001C003Du",
                    "$Resources:fpext,0x001C0043u")
            };
            roleMappings.Add(new RoleMapping("1073741827", "64", "3", roleTemplateMappings5));
            List<RoleTemplateMapping> roleTemplateMappings6 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Edit", "$Resources:fpext,0x001C003Eu",
                    "$Resources:fpext,0x001C0044u")
            };
            roleMappings.Add(new RoleMapping("1073741830", "48", "6", roleTemplateMappings6));
            List<RoleTemplateMapping> roleTemplateMappings7 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Read", "$Resources:fpext,0x001C003Fu",
                    "$Resources:fpext,0x001C0045u")
            };
            roleMappings.Add(new RoleMapping("1073741826", "128", "2", roleTemplateMappings7));
            List<RoleTemplateMapping> roleTemplateMappings8 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Limited Access", "$Resources:fpext,0x001C0040u",
                    "$Resources:fpext,0x001C0046u")
            };
            roleMappings.Add(new RoleMapping("1073741825", "160", "1", roleTemplateMappings8));
            List<RoleTemplateMapping> roleTemplateMappings9 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "Restricted Interfaces for Translation",
                    "$Resources:smscore,use_remote_interfaces_role_name",
                    "$Resources:smscore,use_remote_interfaces_role_description")
            };
            roleMappings.Add(new RoleMapping("1073741927", "2147483647", "0", roleTemplateMappings9));
            List<RoleTemplateMapping> roleTemplateMappings10 = new List<RoleTemplateMapping>()
            {
                RoleMappingFile.GetTemplate("Default", "View Only", "$Resources:xlsrv,RoleNameViewer;",
                    "$Resources:xlsrv,RoleDescriptionViewers;")
            };
            roleMappings.Add(new RoleMapping("1073741928", "2147483647", "0", roleTemplateMappings10));
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<RoleMapping>), new XmlRootAttribute("Roles"));
            using (TextWriter streamWriter = new StreamWriter(RoleMappingFile.RoleMappingFilesPath))
            {
                xmlSerializer.Serialize(streamWriter, roleMappings);
            }
        }

        public static RoleMapping GetRole(string roleId)
        {
            XmlNodeList xmlNodeLists =
                RoleMappingFile.XmlDoc.SelectNodes(string.Concat("/Roles/RoleMapping[@Id=", roleId, "]"));
            if (xmlNodeLists != null && xmlNodeLists.Count > 0)
            {
                XmlNode itemOf = xmlNodeLists[0];
                if (itemOf != null)
                {
                    RoleMapping roleMapping = new RoleMapping()
                    {
                        RoleOrder = itemOf.Attributes["RoleOrder"].Value,
                        RoleType = itemOf.Attributes["RoleType"].Value
                    };
                    return roleMapping;
                }
            }

            return null;
        }

        public static RoleTemplateMapping GetTargetRole(string roleId, string defaultTitle, string webTemplate)
        {
            XmlDocument xmlDoc = RoleMappingFile.XmlDoc;
            string[] strArrays = new string[]
            {
                "/Roles/RoleMapping[@Id=", roleId, "]/Template[contains(@Id, '", webTemplate,
                "') and contains(@DefaultTitle, '", defaultTitle, "')]"
            };
            XmlNodeList xmlNodeLists = xmlDoc.SelectNodes(string.Concat(strArrays));
            if (xmlNodeLists != null && xmlNodeLists.Count == 0)
            {
                XmlDocument xmlDocument = RoleMappingFile.XmlDoc;
                string[] strArrays1 = new string[]
                {
                    "/Roles/RoleMapping[@Id=", roleId,
                    "]/Template[contains(@Id, \"Default\") and contains(@DefaultTitle, '", defaultTitle, "')]"
                };
                xmlNodeLists = xmlDocument.SelectNodes(string.Concat(strArrays1));
            }

            if (xmlNodeLists != null && xmlNodeLists.Count > 0)
            {
                XmlNode itemOf = xmlNodeLists[0];
                if (itemOf != null)
                {
                    RoleTemplateMapping roleTemplateMapping = new RoleTemplateMapping()
                    {
                        Title = itemOf["Title"].InnerText,
                        Description = itemOf["Description"].InnerText
                    };
                    return roleTemplateMapping;
                }
            }

            return null;
        }

        private static RoleTemplateMapping GetTemplate(string templateId, string defaultTitle, string title,
            string description)
        {
            RoleTemplateMapping roleTemplateMapping = new RoleTemplateMapping()
            {
                TemplateId = templateId,
                DefaultTitle = defaultTitle,
                Title = title,
                Description = description
            };
            return roleTemplateMapping;
        }
    }
}