using Metalogix;
using Metalogix.Client.Properties;
using Metalogix.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Metalogix.Actions.Remoting.MappingFiles
{
    public static class ApplicationMappingFiles
    {
        private readonly static string ApplicationMappingFilesPath;

        private readonly static object ApplicationMappingFileLock;

        private static volatile List<Metalogix.Actions.Remoting.MappingFiles.Mapping> _applicationMappingFiles;

        public static List<Metalogix.Actions.Remoting.MappingFiles.Mapping> ApplicationMappings
        {
            get
            {
                if (ApplicationMappingFiles._applicationMappingFiles == null)
                {
                    try
                    {
                        lock (ApplicationMappingFiles.ApplicationMappingFileLock)
                        {
                            if (!File.Exists(ApplicationMappingFiles.ApplicationMappingFilesPath))
                            {
                                ApplicationMappingFiles.SerializeData();
                            }

                            ApplicationMappingFiles._applicationMappingFiles =
                                ApplicationMappingFiles.DeserializeData();
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.LogExceptionToTextFileWithEventLogBackup(exception,
                            "An error occurred while serializing or de-serializing the data for file 'ApplicationMappingFiles.xml'",
                            true);
                    }
                }

                return ApplicationMappingFiles._applicationMappingFiles;
            }
        }

        static ApplicationMappingFiles()
        {
            ApplicationMappingFiles.ApplicationMappingFilesPath =
                Path.Combine(ApplicationData.CommonDataPath, "ApplicationMappingFiles.xml");
            ApplicationMappingFiles.ApplicationMappingFileLock = new object();
        }

        private static List<Metalogix.Actions.Remoting.MappingFiles.Mapping> DeserializeData()
        {
            List<Metalogix.Actions.Remoting.MappingFiles.Mapping> mappings;
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(List<Metalogix.Actions.Remoting.MappingFiles.Mapping>),
                    new XmlRootAttribute("Mappings"));
            using (TextReader streamReader = new StreamReader(ApplicationMappingFiles.ApplicationMappingFilesPath))
            {
                mappings =
                    (List<Metalogix.Actions.Remoting.MappingFiles.Mapping>)xmlSerializer.Deserialize(streamReader);
            }

            return mappings;
        }

        private static void SerializeData()
        {
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_572
                };
            mapping.Files.Add("UserMappings.mls");
            mapping.Files.Add("DomainMappings.mls");
            mapping.Files.Add("UrlMappings.mls");
            mapping.Files.Add("GuidMappings.mls");
            mapping.Files.Add("ExternalConnections.sdf");
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping1 =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_565
                };
            mapping1.Files.Add("PrincipalMappings.mls");
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping2 =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_573
                };
            mapping2.Files.Add("PrincipalMappings.mls");
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping3 =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_578
                };
            mapping3.Files.Add("Metalogix.MediaWiki.Site.Mappings.mlm");
            mapping3.Files.Add("TransformationSettings.xml");
            mapping3.Files.Add("Metalogix.Confluence.Server.Mappings.mlm");
            mapping3.Files.Add("Metalogix.Feed.AtomFeed.Mappings.mlm");
            mapping3.Files.Add("Metalogix.Feed.RSSFeed.Mappings.mlm");
            mapping3.Files.Add("Metalogix.WordPress.Server.Mappings.mlm");
            mapping3.Files.Add("Metalogix.MetaWeblog.Server.Mappings.mlm");
            mapping3.Files.Add("Metalogix.Blogger.Server.Mappings.mlm");
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping4 =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_575
                };
            mapping4.Files.Add("PrincipalMappings.mlm");
            Metalogix.Actions.Remoting.MappingFiles.Mapping mapping5 =
                new Metalogix.Actions.Remoting.MappingFiles.Mapping()
                {
                    Name = Resources.Lic_579
                };
            mapping5.Files.Add("PrincipalMappings.mls");
            mapping5.Files.Add("RoleMappings.mls");
            List<Metalogix.Actions.Remoting.MappingFiles.Mapping> mappings =
                new List<Metalogix.Actions.Remoting.MappingFiles.Mapping>()
                {
                    mapping,
                    mapping1,
                    mapping2,
                    mapping3,
                    mapping4,
                    mapping5
                };
            List<Metalogix.Actions.Remoting.MappingFiles.Mapping> mappings1 = mappings;
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(List<Metalogix.Actions.Remoting.MappingFiles.Mapping>),
                    new XmlRootAttribute("Mappings"));
            using (TextWriter streamWriter = new StreamWriter(ApplicationMappingFiles.ApplicationMappingFilesPath))
            {
                xmlSerializer.Serialize(streamWriter, mappings1);
            }
        }
    }
}