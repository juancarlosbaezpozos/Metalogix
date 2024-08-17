using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration
{
	public class CreateListOptions : SharePointActionOptions
	{
		private string m_sTitle = "";

		private string m_sName = "";

		private string m_sDescription = "";

		private ListTemplateType _template = ListTemplateType.DocumentLibrary;

		private string m_sFeatureId = "";

		private bool m_bIsOnQuickLaunch;

		private bool m_bHasVersions;

		private bool m_bMinorVersions;

		private bool m_bRequiresContentApproval;

		public string Description
		{
			get
			{
				return this.m_sDescription;
			}
			set
			{
				this.m_sDescription = value;
			}
		}

		public string FeatureId
		{
			get
			{
				return this.m_sFeatureId;
			}
			set
			{
				this.m_sFeatureId = value;
			}
		}

		public bool HasMinorVersions
		{
			get
			{
				return this.m_bMinorVersions;
			}
			set
			{
				this.m_bMinorVersions = value;
			}
		}

		public bool HasVersions
		{
			get
			{
				return this.m_bHasVersions;
			}
			set
			{
				this.m_bHasVersions = value;
			}
		}

		public bool IsOnQuickLaunch
		{
			get
			{
				return this.m_bIsOnQuickLaunch;
			}
			set
			{
				this.m_bIsOnQuickLaunch = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
			set
			{
				this.m_sName = value;
			}
		}

		public bool RequiresContentApproval
		{
			get
			{
				return this.m_bRequiresContentApproval;
			}
			set
			{
				this.m_bRequiresContentApproval = value;
			}
		}

		public ListTemplateType Template
		{
			get
			{
				return this._template;
			}
			set
			{
				this._template = value;
			}
		}

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
			set
			{
				this.m_sTitle = value;
			}
		}

		public CreateListOptions()
		{
		}
	}
}