using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration
{
	public class CreateSiteOptions : SharePointActionOptions
	{
		private string m_URL = "";

		private string m_Title = "";

		private string m_Description = "";

		private SPWebTemplate m_Template;

		private bool m_bOverwrite;

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				this.m_Description = value;
			}
		}

		public bool Overwrite
		{
			get
			{
				return this.m_bOverwrite;
			}
			set
			{
				this.m_bOverwrite = value;
			}
		}

		public SPWebTemplate Template
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
			}
		}

		public string Title
		{
			get
			{
				return this.m_Title;
			}
			set
			{
				this.m_Title = value;
			}
		}

		public string URL
		{
			get
			{
				return this.m_URL;
			}
			set
			{
				this.m_URL = value;
			}
		}

		public CreateSiteOptions()
		{
		}
	}
}