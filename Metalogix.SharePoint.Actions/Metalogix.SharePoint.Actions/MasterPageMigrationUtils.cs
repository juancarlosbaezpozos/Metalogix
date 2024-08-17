using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions
{
	public static class MasterPageMigrationUtils
	{
		public const string SUFFIX_FOR_2007_PAGES = "_2007";

		public const string SUFFIX_FOR_2010_PAGES = "_2010";

		public const string UI_VERSION_FOR_2007 = "3";

		public const string UI_VERSION_FOR_2010 = "4";

		public const string UI_VERSION_FOR_2013 = "15";

		public const string UI_VERSION_FOR_O365 = "16";

		public const string GALLERY_URL_MARKER = "/_catalogs/masterpage/";

		public readonly static string[] SUFFIX_LIST;

		static MasterPageMigrationUtils()
		{
			MasterPageMigrationUtils.SUFFIX_LIST = new string[] { "_2007", "_2010" };
		}

		public static Set<string> GetMasterPageUIVersion(SPListItem item)
		{
			string[] strArrays;
			if (item.ParentList.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				string str = item["UIVersion"];
				string[] strArrays1 = new string[] { ";#" };
				strArrays = str.Split(strArrays1, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				strArrays = new string[] { "3" };
			}
			return new Set<string>(strArrays);
		}

		private static int GetSharePointMajorVersionFromUIVersion(string uiVersion)
		{
			if (uiVersion == "3")
			{
				return 12;
			}
			if (uiVersion == "4")
			{
				return 14;
			}
			if (uiVersion == "15")
			{
				return 15;
			}
			if (uiVersion != "16")
			{
				throw new Exception("The specified UI version was not recognized.");
			}
			return 16;
		}

		public static Set<string> GetSupportedUIVersions(SharePointVersion version)
		{
			string[] strArrays;
			if (version.IsSharePoint2007)
			{
				strArrays = new string[] { "3" };
			}
			else if (version.IsSharePoint2010)
			{
				strArrays = new string[] { "3", "4" };
			}
			else if (!version.IsSharePoint2013)
			{
				if (!version.IsSharePoint2016OrLater)
				{
					throw new Exception("No supported UI versions specified for this SharePoint version.");
				}
				strArrays = new string[] { "4", "15", "16" };
			}
			else
			{
				strArrays = new string[] { "4", "15" };
			}
			return new Set<string>(strArrays);
		}

		public static string GetTargetPageName(string sourcePageName, Set<string> sourcePageUIVersions, Set<string> targetPageUIVersions, SharePointVersion sourceVersion, SharePointVersion targetVersion)
		{
			bool flag;
			if (sourceVersion.IsSharePoint2003 || targetVersion.IsSharePoint2007 || sourceVersion.VersionNumber.Major > targetVersion.VersionNumber.Major)
			{
				return sourcePageName;
			}
			if (sourceVersion.MajorVersion == targetVersion.MajorVersion && sourcePageUIVersions.IntersectsWith(targetPageUIVersions))
			{
				return sourcePageName;
			}
			if (!targetVersion.IsSharePoint2010OrLater)
			{
				flag = false;
			}
			else
			{
				flag = true;
				foreach (string targetPageUIVersion in targetPageUIVersions)
				{
					if (MasterPageMigrationUtils.GetSharePointMajorVersionFromUIVersion(targetPageUIVersion) != targetVersion.VersionNumber.Major)
					{
						continue;
					}
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				string[] sUFFIXLIST = MasterPageMigrationUtils.SUFFIX_LIST;
				int num = 0;
				while (num < (int)sUFFIXLIST.Length)
				{
					string str = sUFFIXLIST[num];
					if (!sourcePageName.EndsWith(str))
					{
						num++;
					}
					else
					{
						sourcePageName = sourcePageName.Substring(0, sourcePageName.Length - str.Length);
						return sourcePageName;
					}
				}
			}
			else if (sourceVersion.IsSharePoint2007)
			{
				sourcePageName = string.Concat(sourcePageName, "_2007");
			}
			else if (!sourceVersion.IsSharePoint2010)
			{
				if (!sourceVersion.IsSharePoint2013OrLater)
				{
					throw new Exception("No new name change case exists for this version of SharePoint.");
				}
				if (!sourcePageUIVersions.Contains("15") || !sourcePageUIVersions.Contains("16"))
				{
					return sourcePageName;
				}
				if (!targetVersion.IsSharePoint2013OrLater)
				{
					throw new Exception("No name change case exists for copying 2013 master pages to newer versions of SharePoint.");
				}
				sourcePageName = string.Concat(sourcePageName, "_2010");
			}
			else
			{
				if (!sourcePageUIVersions.Contains("4"))
				{
					return sourcePageName;
				}
				if (!targetVersion.IsSharePoint2010)
				{
					sourcePageName = string.Concat(sourcePageName, "_2010");
				}
				else
				{
					sourcePageName = string.Concat(sourcePageName, "_2007");
				}
			}
			return sourcePageName;
		}

		public static string GetWebUIVersion(SPWeb web)
		{
			if (web.Adapter.SharePointVersion.IsSharePoint2007)
			{
				return "3";
			}
			if (web.Adapter.SharePointVersion.IsSharePoint2010)
			{
				return web.UIVersion;
			}
			if (!web.Adapter.SharePointVersion.IsSharePoint2013OrLater)
			{
				throw new Exception("No UI version detection case for the detected SharePoint version.");
			}
			if (web.ExperienceVersion != SharePoint2013ExperienceVersion.SP2010)
			{
				return "15";
			}
			return "4";
		}

		public static Set<string> GetWebUIVersionAsSet(SPWeb web)
		{
			string[] webUIVersion = new string[] { MasterPageMigrationUtils.GetWebUIVersion(web) };
			return new Set<string>(webUIVersion);
		}
	}
}