using Metalogix;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class Settings
	{
		private static StringCollectionMRU s_visitedSPServers;

		public static StringCollectionMRU VisitedSPServers
		{
			get
			{
				if (Settings.s_visitedSPServers == null)
				{
					string visitedSPServersFile = Settings.VisitedSPServersFile;
					if (!File.Exists(visitedSPServersFile))
					{
						Settings.s_visitedSPServers = new StringCollectionMRU();
					}
					else
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(visitedSPServersFile);
						Settings.s_visitedSPServers = new StringCollectionMRU(xmlDocument.FirstChild);
					}
				}
				return Settings.s_visitedSPServers;
			}
		}

		private static string VisitedSPServersFile
		{
			get
			{
				return Path.Combine(ApplicationData.ApplicationPath, "VisitedSPServers.xml");
			}
		}

		protected Settings()
		{
		}

		public static void SaveVisitedSPServers()
		{
			if (!ApplicationData.IsWeb)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(Settings.VisitedSPServers.ToXML());
				xmlDocument.Save(Settings.VisitedSPServersFile);
			}
		}
	}
}