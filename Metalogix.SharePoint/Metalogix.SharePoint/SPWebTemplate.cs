using Metalogix.Actions;
using Metalogix.Data;
using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Web Template")]
	[PluralName("Web Templates")]
	public class SPWebTemplate
	{
		private static SPWebTemplate[] s_webTemplates;

		private static SPWebTemplateCollection s_allTemplates;

		private int m_iID;

		private int m_iConfig;

		private string m_sName;

		private string m_sTitle;

		private bool m_bIsHidden;

		private string m_sDescription;

		private bool m_bIsRootWebOnly;

		private bool m_bIsSubWebOnly;

		public static SPWebTemplateCollection AllTemplates
		{
			get
			{
				if (SPWebTemplate.s_allTemplates == null)
				{
					SPWebTemplate.s_allTemplates = new SPWebTemplateCollection(SPWebTemplate.s_webTemplates);
				}
				return SPWebTemplate.s_allTemplates;
			}
		}

		public int Config
		{
			get
			{
				return this.m_iConfig;
			}
		}

		public string Description
		{
			get
			{
				return this.m_sDescription;
			}
		}

		public int ID
		{
			get
			{
				return this.m_iID;
			}
		}

		public bool IsHidden
		{
			get
			{
				return this.m_bIsHidden;
			}
		}

		public bool IsRootWebOnly
		{
			get
			{
				return this.m_bIsRootWebOnly;
			}
		}

		public bool IsSubWebOnly
		{
			get
			{
				return this.m_bIsSubWebOnly;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
		}

		static SPWebTemplate()
		{
			SPWebTemplate[] sPWebTemplate = new SPWebTemplate[] { new SPWebTemplate(0, 0, "GLOBAL#0", "Global template"), new SPWebTemplate(1, -1, "STS#-1", "Un-configured"), new SPWebTemplate(1, 0, "STS#0", "Team Site"), new SPWebTemplate(1, 1, "STS#1", "Blank Site"), new SPWebTemplate(1, 2, "STS#2", "Document Workspace"), new SPWebTemplate(2, 0, "MPS#0", "Basic Meeting Workspace"), new SPWebTemplate(2, 1, "MPS#1", "Blank Meeting Workspace"), new SPWebTemplate(2, 2, "MPS#2", "Decision Meeting Workspace"), new SPWebTemplate(2, 3, "MPS#3", "Social Meeting Workspace"), new SPWebTemplate(2, 4, "MPS#4", "Multipage Meeting Workspace"), new SPWebTemplate(3, 0, "CENTRALADMIN#0", "Central Admin Site"), new SPWebTemplate(4, 0, "WIKI#0", "Wiki Site"), new SPWebTemplate(9, 0, "BLOG#0", "Blog"), new SPWebTemplate(7, 0, "BDR#0", "Document Center"), new SPWebTemplate(5, 0, "BUSINESS#0", "Business Tools Team Site"), new SPWebTemplate(5, 1, "BUSINESS#1", "Business Blank Site"), new SPWebTemplate(5, 2, "BUSINESS#2", "Business Document Workspace"), new SPWebTemplate(14483, 0, "OFFILE#0", "Records Center"), new SPWebTemplate(14483, 1, "OFFILE#1", "Records Center"), new SPWebTemplate(40, 0, "OSRV#0", "Shared Services Administration Site"), new SPWebTemplate(20, 0, "SPS#0", "SharePoint Portal Server Site"), new SPWebTemplate(21, 0, "SPSPERS#0", "SharePoint Portal Server Personal Space"), new SPWebTemplate(22, 0, "SPSMSITE#0", "Personalization Site"), new SPWebTemplate(30, 0, "SPSTOC#0", "Contents area Template"), new SPWebTemplate(31, 0, "SPSTOPIC#0", "Topic area template"), new SPWebTemplate(32, 0, "SPSNEWS#0", "News Site"), new SPWebTemplate(39, 0, "CMSPUBLISHING#0", "Publishing Site"), new SPWebTemplate(53, 0, "BLANKINTERNET#0", "Publishing Site"), new SPWebTemplate(53, 1, "BLANKINTERNET#1", "Press Releases Site"), new SPWebTemplate(53, 2, "BLANKINTERNET#2", "Publishing Site with Workflow"), new SPWebTemplate(33, 0, "SPSNHOME#0", "News Site"), new SPWebTemplate(34, 0, "SPSSITES#0", "Site Directory"), new SPWebTemplate(36, 0, "SPSCOMMU#0", "Community area template"), new SPWebTemplate(38, 0, "SPSREPORTCENTER#0", "Report Center"), new SPWebTemplate(47, 0, "SPSPORTAL#0", "Collaboration Portal"), new SPWebTemplate(50, 0, "SRCHCEN#0", "Search Center with Tabs"), new SPWebTemplate(51, 0, "PROFILES#0", "Profiles"), new SPWebTemplate(52, 0, "BLANKINTERNETCONTAINER#0", "Publishing Portal"), new SPWebTemplate(54, 0, "SPSMSITEHOST#0", "My Site Host"), new SPWebTemplate(90, 0, "SRCHCENTERLITE#0", "Search Center"), new SPWebTemplate(90, 1, "SRCHCENTERLITE#1", "Search Center") };
			SPWebTemplate.s_webTemplates = sPWebTemplate;
		}

		public SPWebTemplate(int iID, int iConfig, string sName, string sTitle)
		{
			this.m_iID = iID;
			this.m_iConfig = iConfig;
			this.m_sName = sName;
			this.m_sTitle = sTitle;
		}

		public SPWebTemplate(XmlNode node)
		{
			this.m_iID = int.Parse(this.GetAttribute("ID", node));
			this.m_sName = this.GetAttribute("Name", node);
			this.m_sTitle = this.GetAttribute("Title", node);
			this.m_iConfig = int.Parse(this.GetAttribute("Config", node));
			this.m_bIsHidden = bool.Parse(this.GetAttribute("IsHidden", node));
			this.m_bIsRootWebOnly = bool.Parse(this.GetAttribute("IsRootWebOnly", node));
			this.m_bIsSubWebOnly = bool.Parse(this.GetAttribute("IsSubWebOnly", node));
			this.m_sDescription = this.GetAttribute("Description", node);
		}

		public string GetAttribute(string sAttributeName, XmlNode templateXml)
		{
			string value;
			XmlAttribute itemOf = templateXml.Attributes[sAttributeName];
			if (itemOf == null)
			{
				value = null;
			}
			else
			{
				value = itemOf.Value;
			}
			return value;
		}

		public override string ToString()
		{
			string str = string.Concat(this.m_sTitle, " (", this.m_sName, ") ");
			return str;
		}
	}
}