using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Add", "MLSharePointSiteCollection")]
	public class AddSiteCollectionCmdlet : ActionCmdlet
	{
		private string m_sWebAppName;

		private string m_sWebTemplateName;

		private string m_Langauge;

		private string m_sTitle;

		private string m_sDescription;

		private string m_sPath;

		private string m_sURL;

		private string _hostHeaderSiteCollectionURL;

		private string m_sContentDatabaseName;

		private string m_sOwnerLogin;

		private string m_sSecondaryLogin;

		private bool m_bSelfServiceCreateMode;

		private int m_iExperienceVersion = -1;

		private bool m_bSetSiteQuota;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private string m_sQuotaID;

		private long storageQuota = (long)110;

		private double resourceQuota = 300;

		protected override Type ActionType
		{
			get
			{
				return typeof(CreateSiteCollection);
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Content Database Name to use. Not specifying this value will use the default content database (auto detect).")]
		public string ContentDatabaseName
		{
			get
			{
				return this.m_sContentDatabaseName;
			}
			set
			{
				this.m_sContentDatabaseName = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Description for the Site Collection.")]
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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The experience version to use when creating the site. This setting is only used for SharePoint 2013. Use 15 for the 2013 UI and 14 for the 2010 UI.")]
		public int ExperienceVersion
		{
			get
			{
				return this.m_iExperienceVersion;
			}
			set
			{
				this.m_iExperienceVersion = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Host Header SiteCollection URL to be used when want to create site collection with host header. E.g., \"metalogixdemo.com\"")]
		public string HostHeaderSiteCollectionURL
		{
			get
			{
				return this._hostHeaderSiteCollectionURL;
			}
			set
			{
				this._hostHeaderSiteCollectionURL = value;
			}
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Language to use for the Site Collection. Can be Language Code (LCID) or Language Name. Language must exist in the specified Target.")]
		public string Language
		{
			get
			{
				return this.m_Langauge;
			}
			set
			{
				this.m_Langauge = value;
			}
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Domain and username to be used as the Primary Administrator for the Site Collection. E.g., \"sampledomain\\sample.user\"")]
		public string OwnerLogin
		{
			get
			{
				return this.m_sOwnerLogin;
			}
			set
			{
				this.m_sOwnerLogin = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Managed Path for the Site Collection. Include forward-slash characters where required. E.g., \"/sites/\" or \"/sites/\".")]
		public string Path
		{
			get
			{
				return this.m_sPath;
			}
			set
			{
				this.m_sPath = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Use Quota Template. Can be the integer ID or the string name of the Quota. SetSiteQuota must be set to true and SelfServiceCreateMode should be false.")]
		public string QuotaID
		{
			get
			{
				return this.m_sQuotaID;
			}
			set
			{
				this.m_sQuotaID = value;
				this.m_lQuotaMax = (long)0;
				this.m_lQuotaWarning = (long)0;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Limit site storage to maximum size (Mb). SetSiteQuota must be set to true and SelfServiceCreateMode should be false. Mutually exclusive with QuotaID.")]
		public long QuotaMaximum
		{
			get
			{
				return this.m_lQuotaMax;
			}
			set
			{
				if (value >= (long)0)
				{
					this.m_lQuotaMax = value;
					this.m_sQuotaID = null;
				}
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Send a warning email when the site storage reaches this size (Mb). SetSiteQuota must be set to true and SelfServiceCreateMode should be false. Mutually exclusive with QuotaID.")]
		public long QuotaWarning
		{
			get
			{
				return this.m_lQuotaWarning;
			}
			set
			{
				if (value >= (long)0)
				{
					this.m_lQuotaWarning = value;
					this.m_sQuotaID = null;
				}
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Set Resource Quota for Office 365 Tenant Site Collection.")]
		public double ResourceQuota
		{
			get
			{
				return this.resourceQuota;
			}
			set
			{
				this.resourceQuota = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Domain and username to be used as the Secondary Administrator for the Site Collection. E.g., \"sampledomain\\another.user\"")]
		public string SecondaryOwnerLogin
		{
			get
			{
				return this.m_sSecondaryLogin;
			}
			set
			{
				this.m_sSecondaryLogin = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Allow users without Farm Administrator privileges to add the Site Collection.")]
		public SwitchParameter SelfServiceCreateMode
		{
			get
			{
				return this.m_bSelfServiceCreateMode;
			}
			set
			{
				this.m_bSelfServiceCreateMode = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Set Quota for the Site Collection.")]
		public SwitchParameter SetSiteQuota
		{
			get
			{
				return this.m_bSetSiteQuota;
			}
			set
			{
				this.m_bSetSiteQuota = value;
				if (!value)
				{
					this.m_sQuotaID = null;
					this.m_lQuotaWarning = (long)0;
					this.m_lQuotaMax = (long)0;
				}
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Set Storage Quota for Office 365 Tenant Site Collection.")]
		public long StorageQuota
		{
			get
			{
				return this.storageQuota;
			}
			set
			{
				this.storageQuota = value;
			}
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The Title for the Site Collection.")]
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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Server Relative portion of URL solely used to identify the Site Collection itself. E.g., \"TestSiteCollection\"")]
		public string URL
		{
			get
			{
				return this.m_sURL;
			}
			set
			{
				this.m_sURL = value;
			}
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Name of the Web Application to Create the Site Collection in. Web Application must exist in the specified Target.")]
		public string WebApplicationName
		{
			get
			{
				return this.m_sWebAppName;
			}
			set
			{
				char[] chrArray = new char[] { '/' };
				this.m_sWebAppName = value.TrimEnd(chrArray);
			}
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Web (Site) Template to use for the Site Collection. Web (Site) Template must exist. The value must be in the form name#configuration. E.g., STS#1 for Blank Site")]
		public string WebTemplateName
		{
			get
			{
				return this.m_sWebTemplateName;
			}
			set
			{
				this.m_sWebTemplateName = value;
			}
		}

		public AddSiteCollectionCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			SPWebTemplateCollection sPWebTemplateCollections;
			base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Verifying Parameters ..."));
			if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The Target is null, please initialize a Metalogix SPServer target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			if (!(base.Target is SPBaseServer))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The Target must be a Metalogix SPServer target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			if (base.Target is SPTenant && this.StorageQuota < (long)110)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The Tenant StorageQuota value must be between 110 and 26314."), "ArgumentError", ErrorCategory.InvalidArgument, (object)this.StorageQuota));
			}
			SPBaseServer target = base.Target as SPBaseServer;
			if (this.SelfServiceCreateMode && this.SetSiteQuota)
			{
				this.SetSiteQuota = false;
			}
			base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Web Application ..."));
			SPWebApplication sPWebApplication = null;
			foreach (SPWebApplication webApplication in target.WebApplications)
			{
				if (!webApplication.Name.Equals(this.WebApplicationName, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				sPWebApplication = webApplication;
				break;
			}
			if (sPWebApplication == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Web Applications on Target:");
				foreach (SPWebApplication webApplication1 in target.WebApplications)
				{
					stringBuilder.AppendLine(string.Format("  '{0}'", webApplication1.Name));
				}
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Unable to find Web Application Name '{0}' in Target.{1}{2}{1}", this.WebApplicationName, Environment.NewLine, stringBuilder.ToString())), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			if (string.IsNullOrEmpty(this.HostHeaderSiteCollectionURL) && string.IsNullOrEmpty(this.URL) || !string.IsNullOrEmpty(this.HostHeaderSiteCollectionURL) && !string.IsNullOrEmpty(this.URL))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Please enter either URL or HostHeaderSiteCollectionURL for the Site Collection.", new object[0])), "ArgumentError", ErrorCategory.InvalidData, base.Target));
			}
			if (string.IsNullOrEmpty(this.HostHeaderSiteCollectionURL))
			{
				if (string.IsNullOrEmpty(this.Path))
				{
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Please enter Managed Path for the Site Collection.", new object[0])), "ArgumentError", ErrorCategory.InvalidData, base.Target));
				}
				else
				{
					base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Managed Path ..."));
					if (!this.m_sPath.EndsWith("/"))
					{
						AddSiteCollectionCmdlet addSiteCollectionCmdlet = this;
						addSiteCollectionCmdlet.m_sPath = string.Concat(addSiteCollectionCmdlet.m_sPath, "/");
					}
					if (!this.m_sPath.StartsWith("/"))
					{
						this.m_sPath = string.Concat("/", this.m_sPath);
					}
					this.m_sURL = this.m_sURL.TrimStart("/".ToCharArray());
					this.m_sURL = this.m_sURL.TrimEnd("/".ToCharArray());
					SPPath sPPath = sPWebApplication.Paths.Find((SPPath p) => p.ToString().Equals(this.Path, StringComparison.OrdinalIgnoreCase));
					if (sPPath == null)
					{
						StringBuilder stringBuilder1 = new StringBuilder();
						stringBuilder1.AppendLine("Valid Managed Paths:");
						sPWebApplication.Paths.ForEach((SPPath p) => {
							if (p.IsWildcard)
							{
								stringBuilder1.AppendLine(string.Format("  {0}", p.ToString()));
							}
						});
						object[] path = new object[] { this.Path, this.WebApplicationName, Environment.NewLine, stringBuilder1.ToString() };
						base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Unable to find Managed Path '{0}' in the Web Application '{1}' in the Target.{2}{3}{2}", path)), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
					}
					else if (!sPPath.IsWildcard)
					{
						base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Managed Path '{0}' is Explicit inclusion type. Should be a Wildcard inclusion type in the Web Application '{1}' in the Target.", this.Path, this.WebApplicationName)), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
					}
				}
			}
			if (!string.IsNullOrEmpty(this.ContentDatabaseName))
			{
				base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Content Database  ..."));
				if (sPWebApplication.ContentDatabases.Find((SPContentDatabase cdb) => cdb.Name.Equals(this.ContentDatabaseName)) == null)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.AppendLine("Valid Content Databases:");
					sPWebApplication.ContentDatabases.ForEach((SPContentDatabase db) => stringBuilder2.AppendLine(string.Format("  {0}", db.Name)));
					object[] contentDatabaseName = new object[] { this.ContentDatabaseName, this.WebApplicationName, Environment.NewLine, stringBuilder2.ToString() };
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Unable to find Content Database '{0}' in the Web Application '{1}' in the Target.{2}{3}{2}", contentDatabaseName)), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
				}
			}
			base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Language ..."));
			SPLanguage item = target.Languages[this.Language];
			if (item == null)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.AppendLine("Supported Languages:");
				foreach (SPLanguage language in target.Languages)
				{
					stringBuilder3.AppendLine(string.Format("  LCID : {0} '{1}'", language.LCID, language.Name));
				}
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Unable to find Language '{0}' in Target.{1}{2}{1}", this.Language, Environment.NewLine, stringBuilder3.ToString())), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Web Template ..."));
			sPWebTemplateCollections = (!item.HasMultipleExperienceVersions || !item.ExperienceVersions.Contains(this.m_iExperienceVersion) ? item.Templates : item.GetTemplatesForExperienceVersion(this.m_iExperienceVersion));
			SPWebTemplate sPWebTemplate = sPWebTemplateCollections[this.WebTemplateName];
			if (sPWebTemplate == null)
			{
				StringBuilder stringBuilder4 = new StringBuilder();
				stringBuilder4.AppendLine("Supported Web Templates for Language:");
				foreach (SPWebTemplate template in item.Templates)
				{
					stringBuilder4.AppendLine(string.Format("  '{0}' - '{1}'", template.Name, template.Title));
				}
				object[] webTemplateName = new object[] { this.WebTemplateName, this.Language, Environment.NewLine, stringBuilder4.ToString() };
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("Unable to find WebTemplateName '{0}' for the Language '{1}' in the Target.{2}{3}{2}", webTemplateName)), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			if (!string.IsNullOrEmpty(this.QuotaID))
			{
				base.WriteProgress(new ProgressRecord(0, base.CmdLetName, "Checking Quota ID ..."));
				bool flag = false;
				foreach (SPSiteQuota siteQuotaTemplate in target.SiteQuotaTemplates)
				{
					if (!siteQuotaTemplate.QuotaID.Equals(this.QuotaID, StringComparison.OrdinalIgnoreCase) && !siteQuotaTemplate.Name.Equals(this.QuotaID, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					this.QuotaID = siteQuotaTemplate.QuotaID;
					flag = true;
					break;
				}
				if (!flag)
				{
					StringBuilder stringBuilder5 = new StringBuilder();
					stringBuilder5.AppendLine("Supported Quota Templates:");
					foreach (SPSiteQuota sPSiteQuotum in target.SiteQuotaTemplates)
					{
						stringBuilder5.AppendLine(string.Format("  QuotaID : {0}, Name : '{1}'", sPSiteQuotum.QuotaID, sPSiteQuotum.Name));
					}
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("QuotaID '{0}' is not valid. Please define a valid QuotaID.{1}{2}{1}", this.QuotaID, Environment.NewLine, stringBuilder5.ToString())), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
				}
			}
			string str = string.Concat(sPWebApplication.Url, this.Path, this.URL);
			if (!string.IsNullOrEmpty(this.HostHeaderSiteCollectionURL))
			{
				char chr = ':';
				string url = sPWebApplication.Url;
				char[] chrArray = new char[] { chr };
				string str1 = url.Split(chrArray)[0];
				string hostHeaderSiteCollectionURL = this.HostHeaderSiteCollectionURL;
				char[] chrArray1 = new char[] { '/' };
				str = string.Concat(str1, "://", string.Concat(hostHeaderSiteCollectionURL.TrimEnd(chrArray1), "/"));
			}
			foreach (SPSite site in target.Sites)
			{
				if (!site.Url.Equals(str, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Format("The Absolute URL '{0}' for the new site collection already exists in the Target.", str)), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			base.WriteProgress(new ProgressRecord(0, base.CmdLetName, string.Format("Creating Site Collection '{0}'", str)));
			(base.Action as CreateSiteCollection).SharePointOptions.WebApplication = sPWebApplication;
			(base.Action as CreateSiteCollection).SharePointOptions.Language = item;
			(base.Action as CreateSiteCollection).SharePointOptions.Template = sPWebTemplate;
			(base.Action as CreateSiteCollection).SharePointOptions.ExperienceVersion = this.m_iExperienceVersion;
			(base.Action as CreateSiteCollection).SharePointOptions.Title = this.Title;
			(base.Action as CreateSiteCollection).SharePointOptions.Description = (string.IsNullOrEmpty(this.Description) ? string.Empty : this.Description);
			(base.Action as CreateSiteCollection).SharePointOptions.Path = this.Path;
			(base.Action as CreateSiteCollection).SharePointOptions.URL = this.URL;
			(base.Action as CreateSiteCollection).SharePointOptions.OwnerLogin = this.OwnerLogin;
			(base.Action as CreateSiteCollection).SharePointOptions.SecondaryOwnerLogin = this.SecondaryOwnerLogin;
			(base.Action as CreateSiteCollection).SharePointOptions.ContentDatabaseName = this.ContentDatabaseName;
			(base.Action as CreateSiteCollection).SharePointOptions.SelfServiceCreateMode = this.SelfServiceCreateMode;
			(base.Action as CreateSiteCollection).SharePointOptions.SetSiteQuota = this.SetSiteQuota;
			(base.Action as CreateSiteCollection).SharePointOptions.QuotaMaximum = this.QuotaMaximum;
			(base.Action as CreateSiteCollection).SharePointOptions.QuotaWarning = this.QuotaWarning;
			(base.Action as CreateSiteCollection).SharePointOptions.QuotaID = this.QuotaID;
			(base.Action as CreateSiteCollection).SharePointOptions.ResourceQuota = this.ResourceQuota;
			(base.Action as CreateSiteCollection).SharePointOptions.StorageQuota = this.StorageQuota;
			if (!string.IsNullOrEmpty(this.HostHeaderSiteCollectionURL))
			{
				(base.Action as CreateSiteCollection).SharePointOptions.IsHostHeader = true;
				(base.Action as CreateSiteCollection).SharePointOptions.HostHeaderURL = str;
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}