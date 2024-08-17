using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.SharePoint.Adapters;
using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Other Root Folder")]
	[PluralName("Other Root Folders")]
	public class SPFolderBasic
	{
		private string m_displayUrl = null;

		private SPFileCollection m_files = null;

		private SPFolderBasicCollection m_subFolders = null;

		public string DisplayUrl
		{
			get
			{
				if (this.m_displayUrl == null)
				{
					string[] displayUrl = new string[] { this.Web.DisplayUrl, this.WebRelativeUrl };
					this.m_displayUrl = UrlUtils.ConcatUrls(displayUrl);
				}
				return this.m_displayUrl;
			}
		}

		public SPFileCollection Files
		{
			get
			{
				if (this.m_files == null)
				{
					this.m_files = new SPFileCollection(this.Web, this);
					this.m_files.FetchData();
				}
				return this.m_files;
			}
		}

		public string Name
		{
			get;
			private set;
		}

		[IsSystem(true)]
		public Guid ParentListId
		{
			get;
			private set;
		}

		public string ServerRelativeUrl
		{
			get
			{
				string[] serverRelativeUrl = new string[] { this.Web.ServerRelativeUrl, this.WebRelativeUrl };
				return UrlUtils.EnsureLeadingSlash(UrlUtils.ConcatUrls(serverRelativeUrl));
			}
		}

		public SPFolderBasicCollection SubFolders
		{
			get
			{
				if (this.m_subFolders == null)
				{
					this.m_subFolders = new SPFolderBasicCollection(this.Web, this);
					this.m_subFolders.FetchData();
				}
				return this.m_subFolders;
			}
		}

		public SPWeb Web
		{
			get;
			private set;
		}

		public string WebRelativeUrl
		{
			get;
			private set;
		}

		[IsSystem(true)]
		public string XML
		{
			get;
			private set;
		}

		public SPFolderBasic(SPWeb web) : this(web, "")
		{
		}

		public SPFolderBasic(SPWeb web, XmlNode node)
		{
			this.Web = web;
			this.FromXml(node);
		}

		public SPFolderBasic(SPWeb web, string webRelativeUrl)
		{
			this.Web = web;
			this.WebRelativeUrl = webRelativeUrl;
		}

		private void FromXml(XmlNode node)
		{
			this.Name = node.Attributes["Name"].Value;
			if (node.Attributes["ParentListId"] != null)
			{
				this.ParentListId = new Guid(node.Attributes["ParentListId"].Value);
			}
			this.WebRelativeUrl = node.Attributes["Url"].Value;
			this.XML = node.OuterXml;
		}
	}
}