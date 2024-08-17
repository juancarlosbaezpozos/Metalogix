using Metalogix.Actions;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyGroupsOptions : SharePointActionOptions
	{
		private XmlNode m_xmlGroupExclusions;

		private Metalogix.SharePoint.Migration.LinkCorrectionScope m_bLinkCorrectionScope;

		private bool m_bMapAudiences;

		private bool m_bOverrideCheckouts = true;

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

		[CmdletEnabledParameter(false)]
		public new Metalogix.SharePoint.Migration.LinkCorrectionScope LinkCorrectionScope
		{
			get
			{
				return this.m_bLinkCorrectionScope;
			}
			set
			{
				this.m_bLinkCorrectionScope = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public new bool MapAudiences
		{
			get
			{
				return this.m_bMapAudiences;
			}
			set
			{
				this.m_bMapAudiences = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public new bool OverrideCheckouts
		{
			get
			{
				return this.m_bOverrideCheckouts;
			}
			set
			{
				this.m_bOverrideCheckouts = value;
			}
		}

		public CopyGroupsOptions()
		{
		}
	}
}