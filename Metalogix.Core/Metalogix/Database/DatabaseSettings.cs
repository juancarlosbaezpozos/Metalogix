using Metalogix;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Database
{
    public abstract class DatabaseSettings
    {
        private static StringCollectionMRU s_visitedSQLServers;

        public static StringCollectionMRU VisitedSQLServers
        {
            get
            {
                if (DatabaseSettings.s_visitedSQLServers == null)
                {
                    if (!File.Exists(DatabaseSettings.VisitedSQLServersFile))
                    {
                        DatabaseSettings.s_visitedSQLServers = new StringCollectionMRU();
                    }
                    else
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(DatabaseSettings.VisitedSQLServersFile);
                        DatabaseSettings.s_visitedSQLServers = new StringCollectionMRU(xmlDocument.FirstChild);
                    }
                }

                return DatabaseSettings.s_visitedSQLServers;
            }
        }

        private static string VisitedSQLServersFile
        {
            get { return string.Concat(ApplicationData.ApplicationPath, "VisitedSQLServers.xml"); }
        }

        protected DatabaseSettings()
        {
        }

        public static void SaveVisitedSQLServers()
        {
            if (ApplicationData.IsWeb)
            {
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(DatabaseSettings.VisitedSQLServers.ToXML());
            xmlDocument.Save(DatabaseSettings.VisitedSQLServersFile);
        }
    }
}