using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public static class SPWebPartPageTemplate
	{
		private static List<string> m_WppTemplateList;

		private static Dictionary<string, string[]> m_PageLayoutZonesMappings;

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> m_TemplateZones;

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> m_DashboardTemplateZones;

		private static Dictionary<string, string[]> m_DefaultTemplateZones;

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> m_TemplateMatchIndicators;

		static SPWebPartPageTemplate()
		{
			SPWebPartPageTemplate.m_WppTemplateList = null;
			SPWebPartPageTemplate.m_PageLayoutZonesMappings = null;
			SPWebPartPageTemplate.m_TemplateZones = null;
			SPWebPartPageTemplate.m_DashboardTemplateZones = null;
			SPWebPartPageTemplate.m_DefaultTemplateZones = null;
			SPWebPartPageTemplate.m_TemplateMatchIndicators = null;
		}

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> BuildDashboardTemplateZonesDictionary()
		{
			if (SPWebPartPageTemplate.m_DashboardTemplateZones == null)
			{
				List<SPWebPartPageTemplate.TemplateZoneMapping> templateZoneMappings = new List<SPWebPartPageTemplate.TemplateZoneMapping>();
				string[] strArrays = new string[] { "FilterZone", "TopRightZone", "TopLeftZone", "BottomZone" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(1, strArrays));
				strArrays = new string[] { "FilterZone", "TopRightZone", "TopLeftZone", "MiddleLeftZone", "MiddleRightZone", "BottomLeftZone", "BottomRightZone" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(2, strArrays));
				strArrays = new string[] { "FilterZone", "TopRightZone", "TopLeftZone", "MiddleLeftZone", "MiddleMiddleZone", "BottomLeftZone", "BottomMiddleZone", "MiddleRightZone", "BottomRightZone" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(0, strArrays));
				SPWebPartPageTemplate.m_DashboardTemplateZones = templateZoneMappings;
			}
			return SPWebPartPageTemplate.m_DashboardTemplateZones;
		}

		private static Dictionary<string, string[]> BuildDefaultTemplateZonesDictionary()
		{
			if (SPWebPartPageTemplate.m_DefaultTemplateZones == null)
			{
				Dictionary<string, string[]> strs = new Dictionary<string, string[]>();
				string[] lower = new string[] { "left", "right" };
				string[] strArrays = lower;
				lower = new string[] { "BlogNavigator".ToLower(), "left", "right" };
				string[] strArrays1 = lower;
				lower = new string[] { "MeetingSummary".ToLower(), "MeetingNavigator".ToLower(), "left", "center", "right" };
				string[] strArrays2 = lower;
				lower = new string[] { "TopZone".ToLower(), "MiddleLeftZone".ToLower(), "MiddleRightZone".ToLower(), "BottomZone".ToLower() };
				string[] strArrays3 = lower;
				strs.Add("STS\\default.aspx".ToLower(), strArrays);
				strs.Add("SiteTemplates\\CENTRALADMIN\\default.aspx".ToLower(), strArrays);
				strs.Add("SiteTemplates\\BDR\\default.aspx".ToLower(), strArrays);
				strs.Add("SiteTemplates\\offile\\default.aspx".ToLower(), strArrays);
				strs.Add("MPS\\default.aspx".ToLower(), strArrays2);
				strs.Add("SiteTemplates\\Blog\\default.aspx".ToLower(), strArrays1);
				strs.Add("SiteTemplates\\SPSMSITE\\default.aspx".ToLower(), strArrays3);
				strs.Add("SiteTemplates\\SPSMSITEHOST\\default.aspx".ToLower(), strArrays3);
				strs.Add("SiteTemplates\\SPSPERS\\default.aspx".ToLower(), strArrays3);
				string str = "STS\\DWS\\default.aspx".ToLower();
				lower = new string[] { "top", "left", "right" };
				strs.Add(str, lower);
				string lower1 = "SiteTemplates\\STS\\defaultdws.aspx".ToLower();
				lower = new string[] { "top", "left", "right" };
				strs.Add(lower1, lower);
				string str1 = "SiteTemplates\\SRCHCENTERLITE\\default.aspx".ToLower();
				lower = new string[] { "TopZone".ToLower(), "BottomZone".ToLower() };
				strs.Add(str1, lower);
				string lower2 = "SiteTemplates\\PUBLISHING\\default.aspx".ToLower();
				lower = new string[] { "TopColumnZone".ToLower(), "RightColumnZone".ToLower(), "LeftColumnZone".ToLower() };
				strs.Add(lower2, lower);
				string str2 = "AdvancedSearchLayout.aspx".ToLower();
				lower = new string[] { "TopZone", "MidUpperLeftZone", "MidUpperRightZone", "BottomZone" };
				strs.Add(str2, lower);
				string lower3 = "BlankWebPartPage.aspx".ToLower();
				lower = new string[] { "Header", "TopLeftRow", "TopRightRow", "RightColumn", "CenterLeftColumn", "CenterColumn", "CenterRightColumn", "Footer" };
				strs.Add(lower3, lower);
				string str3 = "PeopleSearchResults.aspx".ToLower();
				lower = new string[] { "TopZone", "MidUpperLeftZone", "MidUpperRightZone", "MidLowerLeftZone", "MidLowerRightZone", "RightZone", "BottomZone" };
				strs.Add(str3, lower);
				string lower4 = "SearchMain.aspx".ToLower();
				lower = new string[] { "TopZone", "MiddleLeftZone" };
				strs.Add(lower4, lower);
				string str4 = "SearchResults.aspx".ToLower();
				lower = new string[] { "TopZone", "MidUpperLeftZone", "MidUpperRightZone", "MidLowerLeftZone", "MidLowerRightZone", "RightZone", "BottomZone" };
				strs.Add(str4, lower);
				string lower5 = "TabViewPageLayout.aspx".ToLower();
				lower = new string[] { "Header", "TopZone", "RightColumn", "Footer" };
				strs.Add(lower5, lower);
				string str5 = "WelcomeLinks.aspx".ToLower();
				lower = new string[] { "TopColumnZone", "LeftColumnZone", "RightColumnZone" };
				strs.Add(str5, lower);
				string lower6 = "WelcomeTOC.aspx".ToLower();
				lower = new string[] { "TopColumnZone", "LeftColumnZone", "RightColumnZone" };
				strs.Add(lower6, lower);
				string str6 = "DefaultLayout.aspx".ToLower();
				lower = new string[] { "TopZone", "MiddleLeftZone", "MiddleRightZone", "RightZone", "BottomZone" };
				strs.Add(str6, lower);
				string lower7 = "WelcomeSplash.aspx".ToLower();
				lower = new string[] { "TopZone", "BottomLeftZone", "BottomRightZone" };
				strs.Add(lower7, lower);
				SPWebPartPageTemplate.m_DefaultTemplateZones = strs;
			}
			return SPWebPartPageTemplate.m_DefaultTemplateZones;
		}

		private static Dictionary<string, string[]> BuildPageLayoutZonesDictionary()
		{
			if (SPWebPartPageTemplate.m_PageLayoutZonesMappings == null)
			{
				Dictionary<string, string[]> strs = new Dictionary<string, string[]>(8);
				string lower = "AdvancedSearchLayout.aspx".ToLower();
				string[] strArrays = new string[] { "TopZone", "MiddleUpperLeftZone", "MiddleUpperRightZone", "BottomZone" };
				strs.Add(lower, strArrays);
				string str = "BlankWebPartPage.aspx".ToLower();
				strArrays = new string[] { "Header", "TopLeft", "TopRight", "Right", "CenterLeft", "Center", "CenterRight", "Footer" };
				strs.Add(str, strArrays);
				string lower1 = "PeopleSearchResults.aspx".ToLower();
				strArrays = new string[] { "TopZone", "MiddleUpperLeftZone", "MiddleUpperRightZone", "MiddleLowerLeftZone", "MiddleLowerRightZone", "RightZone", "BottomZone" };
				strs.Add(lower1, strArrays);
				string str1 = "SearchMain.aspx".ToLower();
				strArrays = new string[] { "TopZone", "BottomZone" };
				strs.Add(str1, strArrays);
				string lower2 = "SearchResults.aspx".ToLower();
				strArrays = new string[] { "TopZone", "MiddleUpperLeftZone", "MiddleUpperRightZone", "MiddleLowerLeftZone", "MiddleLowerRightZone", "RightZone", "BottomZone" };
				strs.Add(lower2, strArrays);
				string str2 = "TabViewPageLayout.aspx".ToLower();
				strArrays = new string[] { "Header", "LeftColumn", "RightColumn", "Footer" };
				strs.Add(str2, strArrays);
				string lower3 = "WelcomeLinks.aspx".ToLower();
				strArrays = new string[] { "Top", "LeftColumn", "RightColumn" };
				strs.Add(lower3, strArrays);
				string str3 = "WelcomeTOC.aspx".ToLower();
				strArrays = new string[] { "Top", "LeftColumn", "RightColumn" };
				strs.Add(str3, strArrays);
				string lower4 = "DefaultLayout.aspx".ToLower();
				strArrays = new string[] { "TopZone", "MiddleLeftZone", "MiddleRightZone", "RightZone", "BottomZone" };
				strs.Add(lower4, strArrays);
				string str4 = "WelcomeSplash.aspx".ToLower();
				strArrays = new string[] { "Top", "BottomLeftZone", "BottomRightZone" };
				strs.Add(str4, strArrays);
				SPWebPartPageTemplate.m_PageLayoutZonesMappings = strs;
			}
			return SPWebPartPageTemplate.m_PageLayoutZonesMappings;
		}

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> BuildTemplateMatchIndicators()
		{
			if (SPWebPartPageTemplate.m_TemplateMatchIndicators == null)
			{
				List<SPWebPartPageTemplate.TemplateZoneMapping> templateZoneMappings = new List<SPWebPartPageTemplate.TemplateZoneMapping>();
				string[] strArrays = new string[] { "TitleBar", "FullPage" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(1, strArrays));
				strArrays = new string[] { "MiddleColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(2, strArrays));
				strArrays = new string[] { "Body", "LeftColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(3, strArrays));
				strArrays = new string[] { "Body", "RightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(4, strArrays));
				strArrays = new string[] { "Row1" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(5, strArrays));
				strArrays = new string[] { "Row2" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(5, strArrays));
				strArrays = new string[] { "Row3" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(5, strArrays));
				strArrays = new string[] { "Row4" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(5, strArrays));
				strArrays = new string[] { "TopRow", "LeftColumn", "RightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(6, strArrays));
				strArrays = new string[] { "LeftColumn", "RightColumn", "CenterLeftColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(6, strArrays));
				strArrays = new string[] { "LeftColumn", "RightColumn", "CenterRightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(6, strArrays));
				strArrays = new string[] { "LeftColumn", "TopRow" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(7, strArrays));
				strArrays = new string[] { "TopRow", "RightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(8, strArrays));
				SPWebPartPageTemplate.m_TemplateMatchIndicators = templateZoneMappings;
			}
			return SPWebPartPageTemplate.m_TemplateMatchIndicators;
		}

		private static List<SPWebPartPageTemplate.TemplateZoneMapping> BuildTemplateZonesDictionary()
		{
			if (SPWebPartPageTemplate.m_TemplateZones == null)
			{
				List<SPWebPartPageTemplate.TemplateZoneMapping> templateZoneMappings = new List<SPWebPartPageTemplate.TemplateZoneMapping>();
				string[] strArrays = new string[] { "TitleBar", "FullPage" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(1, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "LeftColumn", "MiddleColumn", "RightColumn", "Footer" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(2, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "LeftColumn", "Body" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(3, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "Body", "RightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(4, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "LeftColumn", "Row1", "Row2", "Row3", "Row4", "RightColumn", "Footer" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(5, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "LeftColumn", "TopRow", "RightColumn", "CenterLeftColumn", "CenterRightColumn", "Footer" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(6, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "LeftColumn", "TopRow", "CenterLeftColumn", "CenterColumn", "CenterRightColumn", "Footer" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(7, strArrays));
				strArrays = new string[] { "TitleBar", "Header", "TopRow", "CenterLeftColumn", "CenterColumn", "CenterRightColumn", "Footer", "RightColumn" };
				templateZoneMappings.Add(new SPWebPartPageTemplate.TemplateZoneMapping(8, strArrays));
				SPWebPartPageTemplate.m_TemplateZones = templateZoneMappings;
			}
			return SPWebPartPageTemplate.m_TemplateZones;
		}

		private static List<string> BuildWebPartPageTemplateList()
		{
			if (SPWebPartPageTemplate.m_WppTemplateList == null)
			{
				List<string> strs = new List<string>()
				{
					string.Format("spstd{0}.aspx", "1"),
					string.Format("spstd{0}.aspx", "2"),
					string.Format("spstd{0}.aspx", "3"),
					string.Format("spstd{0}.aspx", "4"),
					string.Format("spstd{0}.aspx", "5"),
					string.Format("spstd{0}.aspx", "6"),
					string.Format("spstd{0}.aspx", "7"),
					string.Format("spstd{0}.aspx", "8"),
					"STS\\DWS\\default.aspx".ToLower(),
					"MPS\\default.aspx".ToLower(),
					"STS\\default.aspx".ToLower(),
					"SiteTemplates\\Blog\\default.aspx".ToLower(),
					"SiteTemplates\\CENTRALADMIN\\default.aspx".ToLower(),
					"SiteTemplates\\MPS\\default.aspx".ToLower(),
					"SiteTemplates\\BDR\\default.aspx".ToLower(),
					"SiteTemplates\\STS\\defaultdws.aspx".ToLower(),
					"SiteTemplates\\SRCHCENTERLITE\\default.aspx".ToLower(),
					"SiteTemplates\\SPSMSITE\\default.aspx".ToLower(),
					"SiteTemplates\\SPSMSITEHOST\\default.aspx".ToLower(),
					"SiteTemplates\\SPSPERS\\default.aspx".ToLower(),
					"SiteTemplates\\offile\\default.aspx".ToLower(),
					"SiteTemplates\\PUBLISHING\\default.aspx".ToLower(),
					"PortalLayouts\\defaultLayout.aspx".ToLower()
				};
				strs.AddRange(SPWebPartPageTemplate.BuildPageLayoutZonesDictionary().Keys);
				SPWebPartPageTemplate.m_WppTemplateList = strs;
			}
			return SPWebPartPageTemplate.m_WppTemplateList;
		}

		public static int DetermineBestDashboardTemplateId(SPWebPartZoneSet zonesPresentOnWebPartPage)
		{
			int num;
			if (zonesPresentOnWebPartPage.Count != 0)
			{
				int? nullable = null;
				nullable = SPWebPartPageTemplate.DetermineExactDashboardTemplateId(zonesPresentOnWebPartPage);
				if (!nullable.HasValue)
				{
					nullable = new int?(SPWebPartPageTemplate.GuessDashboardTemplateId(zonesPresentOnWebPartPage));
				}
				num = (nullable.HasValue ? nullable.Value : 0);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static int DetermineBestTemplateId(SPWebPartZoneSet zonesPresentOnWebPartPage)
		{
			int num;
			if (zonesPresentOnWebPartPage.Count != 0)
			{
				int? nullable = null;
				nullable = SPWebPartPageTemplate.DetermineExactTemplateId(zonesPresentOnWebPartPage);
				if (!nullable.HasValue)
				{
					nullable = new int?(SPWebPartPageTemplate.GuessTemplateId(zonesPresentOnWebPartPage));
				}
				num = (nullable.HasValue ? nullable.Value : 0);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private static int? DetermineExactDashboardTemplateId(SPWebPartZoneSet zonesAvailable)
		{
			int? nullable = null;
			foreach (SPWebPartPageTemplate.TemplateZoneMapping templateZoneMapping in SPWebPartPageTemplate.BuildDashboardTemplateZonesDictionary())
			{
				if (zonesAvailable.IsEqual(templateZoneMapping.Zones))
				{
					nullable = new int?(templateZoneMapping.TemplateId);
					break;
				}
			}
			return nullable;
		}

		private static int? DetermineExactTemplateId(SPWebPartZoneSet zonesAvailable)
		{
			int? nullable = null;
			foreach (SPWebPartPageTemplate.TemplateZoneMapping templateZoneMapping in SPWebPartPageTemplate.BuildTemplateMatchIndicators())
			{
				if (zonesAvailable.IsSupersetOf(templateZoneMapping.Zones))
				{
					nullable = new int?(templateZoneMapping.TemplateId);
					break;
				}
			}
			return nullable;
		}

		public static SPWebPartZoneSet GetAvailableZonesForDefaultWebPartPage(string sTemplateFile)
		{
			SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
			if (!string.IsNullOrEmpty(sTemplateFile))
			{
				Dictionary<string, string[]> strs = SPWebPartPageTemplate.BuildDefaultTemplateZonesDictionary();
				foreach (string key in strs.Keys)
				{
					if (sTemplateFile.ToLower().Contains(key))
					{
						sPWebPartZoneSet.AddZones(strs[key]);
						break;
					}
				}
			}
			return sPWebPartZoneSet;
		}

		public static SPWebPartZoneSet GetAvailableZonesForTemplateId(int iPageTemplateId)
		{
			SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
			if ((iPageTemplateId < 1 ? false : iPageTemplateId <= 8))
			{
				foreach (SPWebPartPageTemplate.TemplateZoneMapping templateZoneMapping in SPWebPartPageTemplate.BuildTemplateZonesDictionary())
				{
					if (templateZoneMapping.TemplateId == iPageTemplateId)
					{
						sPWebPartZoneSet.AddZones(templateZoneMapping.Zones.ToArray());
						break;
					}
				}
			}
			return sPWebPartZoneSet;
		}

		public static SPWebPartZoneSet GetAvailableZonesFromTemplate(string sTemplateFile)
		{
			SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
			if (SPWebPartPageTemplate.IsWebPartPageTemplateFile(sTemplateFile))
			{
				if (!string.IsNullOrEmpty(sTemplateFile))
				{
					int? nullable = SPWebPartPageTemplate.ParseTemplateFileForId(sTemplateFile);
					if (nullable.HasValue)
					{
						sPWebPartZoneSet = SPWebPartPageTemplate.GetAvailableZonesForTemplateId(nullable.Value);
					}
				}
				if (sPWebPartZoneSet.Count <= 0)
				{
					sPWebPartZoneSet = SPWebPartPageTemplate.GetAvailableZonesForDefaultWebPartPage(sTemplateFile);
				}
			}
			return sPWebPartZoneSet;
		}

		private static int GuessDashboardTemplateId(SPWebPartZoneSet zonesInUse)
		{
			List<SPWebPartPageTemplate.TemplateZoneMapping> templateZoneMappings = SPWebPartPageTemplate.BuildDashboardTemplateZonesDictionary();
			int templateId = 0;
			int count = 0;
			foreach (SPWebPartPageTemplate.TemplateZoneMapping templateZoneMapping in templateZoneMappings)
			{
				if ((!zonesInUse.IsSubsetOf(templateZoneMapping.Zones) ? false : templateZoneMapping.Zones.Count > count))
				{
					templateId = templateZoneMapping.TemplateId;
					count = templateZoneMapping.Zones.Count;
				}
			}
			return templateId;
		}

		private static int GuessTemplateId(SPWebPartZoneSet zonesInUse)
		{
			List<SPWebPartPageTemplate.TemplateZoneMapping> templateZoneMappings = SPWebPartPageTemplate.BuildTemplateZonesDictionary();
			int templateId = 6;
			int count = 0;
			foreach (SPWebPartPageTemplate.TemplateZoneMapping templateZoneMapping in templateZoneMappings)
			{
				if ((!zonesInUse.IsSubsetOf(templateZoneMapping.Zones) ? false : templateZoneMapping.Zones.Count > count))
				{
					templateId = templateZoneMapping.TemplateId;
					count = templateZoneMapping.Zones.Count;
				}
			}
			return templateId;
		}

		public static bool IsWebPartPageTemplateFile(string sSetupPath)
		{
			bool flag;
			if (sSetupPath != null)
			{
				string lower = sSetupPath.ToLower();
				if (lower.Contains("|"))
				{
					lower = lower.Substring(0, lower.IndexOf("|"));
				}
				foreach (string str in SPWebPartPageTemplate.BuildWebPartPageTemplateList())
				{
					if (lower.ToLower().Contains(str))
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private static int? ParseTemplateFileForId(string sTemplateFile)
		{
			int? nullable;
			int? nullable1 = null;
			char[] chrArray = new char[] { '\\', '/' };
			int num = sTemplateFile.LastIndexOfAny(chrArray);
			string str = (num < 0 || sTemplateFile.Length <= 1 ? "" : sTemplateFile.Substring(num + 1));
			if ((!str.ToLower().StartsWith("spstd") ? false : str.ToLower().EndsWith(".aspx")))
			{
				string str1 = str.Substring("spstd".Length, 1);
				int num1 = -1;
				if (int.TryParse(str1, out num1))
				{
					nullable = new int?(num1);
				}
				else
				{
					nullable = null;
				}
				nullable1 = nullable;
			}
			return nullable1;
		}

		private class TemplateZoneMapping
		{
			private int m_iTemplateId;

			private string m_sTemplateFileName;

			private SPWebPartZoneSet m_Zones;

			public string TemplateFileName
			{
				get
				{
					return this.m_sTemplateFileName;
				}
			}

			public int TemplateId
			{
				get
				{
					return this.m_iTemplateId;
				}
			}

			public SPWebPartZoneSet Zones
			{
				get
				{
					return this.m_Zones;
				}
			}

			public TemplateZoneMapping(int iTemplateId, string[] stringSet)
			{
				this.m_iTemplateId = iTemplateId;
				this.m_sTemplateFileName = null;
				this.m_Zones = new SPWebPartZoneSet(stringSet);
			}

			public TemplateZoneMapping(string sTemplateFileName, string[] stringSet)
			{
				this.m_iTemplateId = -1;
				this.m_sTemplateFileName = sTemplateFileName;
				this.m_Zones = new SPWebPartZoneSet(stringSet);
			}
		}
	}
}