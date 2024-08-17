using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint.Database
{
	public abstract class DatabaseSettings
	{
		private static Metalogix.Explorer.Settings.MappingCollection s_openedBackups;

		public static Metalogix.Explorer.Settings.MappingCollection OpenedBackups
		{
			get
			{
				if (DatabaseSettings.s_openedBackups == null)
				{
					string openedBackupsFile = DatabaseSettings.OpenedBackupsFile;
					if (!File.Exists(openedBackupsFile))
					{
						DatabaseSettings.s_openedBackups = new Metalogix.Explorer.Settings.MappingCollection();
					}
					else
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(openedBackupsFile);
						DatabaseSettings.s_openedBackups = new Metalogix.Explorer.Settings.MappingCollection(xmlDocument.FirstChild);
					}
				}
				return DatabaseSettings.s_openedBackups;
			}
		}

		private static string OpenedBackupsFile
		{
			get
			{
				return Path.Combine(ApplicationData.ApplicationPath, "OpenedBackups.xml");
			}
		}

		protected DatabaseSettings()
		{
		}

		public static void SaveOpenedBackups()
		{
			if (!ApplicationData.IsWeb)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(DatabaseSettings.OpenedBackups.ToXML());
				xmlDocument.Save(DatabaseSettings.OpenedBackupsFile);
			}
		}
	}
}