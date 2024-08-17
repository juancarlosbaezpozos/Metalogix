using Metalogix;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPGroupOptions : OptionsBase
	{
		private bool m_bMapGroupsByName;

		private bool m_bOverwriteGroups;

		private XmlNode m_xmlGroupExclusions;

		public XmlNode GroupExclusions
		{
			get
			{
				return this.m_xmlGroupExclusions;
			}
			set
			{
				this.m_xmlGroupExclusions = value;
			}
		}

		public bool MapGroupsByName
		{
			get
			{
				return this.m_bMapGroupsByName;
			}
			set
			{
				this.m_bMapGroupsByName = value;
			}
		}

		public bool OverwriteGroups
		{
			get
			{
				return this.m_bOverwriteGroups;
			}
			set
			{
				this.m_bOverwriteGroups = value;
			}
		}

		public SPGroupOptions()
		{
		}
	}
}