using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointSiteCollection")]
	public class CopySiteCollectionCmdlet : CopySiteCmdlet
	{
		private string m_sName;

		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteCollectionAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The content database name to add the new site collection to.")]
		public string ContentDatabaseName
		{
			get
			{
				return this.PasteSiteCollectionOptions.ContentDatabaseName;
			}
			set
			{
				this.PasteSiteCollectionOptions.ContentDatabaseName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if site collection level audit settings should be copied.")]
		public SwitchParameter CopyAuditSettings
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyAuditSettings;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyAuditSettings = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the list template gallery should be copied.")]
		public SwitchParameter CopyListTemplateGallery
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyListTemplateGallery;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyListTemplateGallery = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the master page gallery should be copied.")]
		public SwitchParameter CopyMasterPageGallery
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyMasterPageGallery;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyMasterPageGallery = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if master pages in the master page gallery should be copied.")]
		public SwitchParameter CopyMasterPages
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyMasterPages;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyMasterPages = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if other resources, such as images, in the master page gallery should be copied.")]
		public SwitchParameter CopyOtherResources
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyOtherResources;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyOtherResources = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if page layouts in the master page gallery should be copied.")]
		public SwitchParameter CopyPageLayouts
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopyPageLayouts;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopyPageLayouts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the source site collection administrators gets copied to the target. If not selected, the Primary and Secondary site admins will be copied.")]
		public SwitchParameter CopySiteAdmins
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopySiteAdmins;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopySiteAdmins = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if a site quota template should be set on the target. If no QuotaID or individual values are specified, the quota will be set to match the source.")]
		public SwitchParameter CopySiteQuota
		{
			get
			{
				return this.PasteSiteCollectionOptions.CopySiteQuota;
			}
			set
			{
				this.PasteSiteCollectionOptions.CopySiteQuota = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if link correction should be run on the contents of master pages.")]
		public SwitchParameter CorrectMasterPageLinks
		{
			get
			{
				return this.PasteSiteCollectionOptions.CorrectMasterPageLinks;
			}
			set
			{
				this.PasteSiteCollectionOptions.CorrectMasterPageLinks = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The description of the site collection.")]
		public string Description
		{
			get
			{
				return this.PasteSiteCollectionOptions.Description;
			}
			set
			{
				this.PasteSiteCollectionOptions.Description = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The experience version to use when creating the site. This setting is only used for SharePoint 2013. Use 15 for the 2013 UI and 14 for the 2010 UI.")]
		public int ExperienceVersion
		{
			get
			{
				return this.PasteSiteCollectionOptions.ExperienceVersion;
			}
			set
			{
				this.PasteSiteCollectionOptions.ExperienceVersion = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates Host header site collection URL.")]
		public string HostHeaderURL
		{
			get
			{
				return this.PasteSiteCollectionOptions.HostHeaderURL;
			}
			set
			{
				this.PasteSiteCollectionOptions.HostHeaderURL = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if migration is with host header site collection.")]
		public SwitchParameter IsHostHeader
		{
			get
			{
				return this.PasteSiteCollectionOptions.IsHostHeader;
			}
			set
			{
				this.PasteSiteCollectionOptions.IsHostHeader = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The 32 bit integer language code of the SharePoint language to use.")]
		public string LanguageCode
		{
			get
			{
				return this.PasteSiteCollectionOptions.LanguageCode;
			}
			set
			{
				this.PasteSiteCollectionOptions.LanguageCode = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="The site collection URL name. Can not include the managed path.")]
		public string Name
		{
			get
			{
				return this.PasteSiteCollectionOptions.Name;
			}
			set
			{
				this.PasteSiteCollectionOptions.Name = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="The login name of the primary owner of the site collection to be added.")]
		public string OwnerLogin
		{
			get
			{
				return this.PasteSiteCollectionOptions.OwnerLogin;
			}
			set
			{
				this.PasteSiteCollectionOptions.OwnerLogin = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteSiteCollectionOptions PasteSiteCollectionOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteSiteCollectionOptions;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="The managed path in the web application to add the site collection under.")]
		public string Path
		{
			get
			{
				return this.PasteSiteCollectionOptions.Path;
			}
			set
			{
				this.PasteSiteCollectionOptions.Path = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The ID of the quota template to use.")]
		public string QuotaID
		{
			get
			{
				return this.PasteSiteCollectionOptions.QuotaID;
			}
			set
			{
				this.PasteSiteCollectionOptions.QuotaID = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The maximum storage limit to use for this site collection. Note that if a valid QuotaID is also specified, that quota template will be used instead.")]
		public long QuotaMaximum
		{
			get
			{
				return this.PasteSiteCollectionOptions.QuotaMaximum;
			}
			set
			{
				this.PasteSiteCollectionOptions.QuotaMaximum = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The storage level to send out a warning email to use for this site collection. Note that if a valid QuotaID is also specified, that quota template will be used instead.")]
		public long QuotaWarning
		{
			get
			{
				return this.PasteSiteCollectionOptions.QuotaWarning;
			}
			set
			{
				this.PasteSiteCollectionOptions.QuotaWarning = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The O365 tenant resource quota.")]
		public double ResourceQuota
		{
			get
			{
				return this.PasteSiteCollectionOptions.ResourceQuota;
			}
			set
			{
				this.PasteSiteCollectionOptions.ResourceQuota = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The login name of the secondary owner of the site collection to be added.")]
		public string SecondaryOwnerLogin
		{
			get
			{
				return this.PasteSiteCollectionOptions.SecondaryOwnerLogin;
			}
			set
			{
				this.PasteSiteCollectionOptions.SecondaryOwnerLogin = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Self Service Create Mode may be used if Site Provisioning is turned on in Central Admin for the given Web Application.")]
		public SwitchParameter SelfServiceCreateMode
		{
			get
			{
				return this.PasteSiteCollectionOptions.SelfServiceCreateMode;
			}
			set
			{
				this.PasteSiteCollectionOptions.SelfServiceCreateMode = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The source site collection administrators to be migrated to the target site collection.")]
		public string SiteCollectionAdministrators
		{
			get
			{
				return this.PasteSiteCollectionOptions.SiteCollectionAdministrators;
			}
			set
			{
				this.PasteSiteCollectionOptions.SiteCollectionAdministrators = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The O365 tenant storage quota.")]
		public long StorageQuota
		{
			get
			{
				return this.PasteSiteCollectionOptions.StorageQuota;
			}
			set
			{
				this.PasteSiteCollectionOptions.StorageQuota = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="DO NOT USE: The URL of the site collection. Includes managed path, and will be set automatically via other parameters. Exists for automated script generation purposes and bypasses validation.")]
		public string URL
		{
			get
			{
				return this.PasteSiteCollectionOptions.URL;
			}
			set
			{
				this.PasteSiteCollectionOptions.URL = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="The name of the web application to add the site collection to.")]
		public string WebApplicationName
		{
			get
			{
				return this.PasteSiteCollectionOptions.WebApplicationName;
			}
			set
			{
				this.PasteSiteCollectionOptions.WebApplicationName = value;
			}
		}

		public CopySiteCollectionCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters() && base.Target is SPServer)
			{
				using (SPServer target = (SPServer)base.Target)
				{
					bool flag = false;
					bool flag1 = false;
					bool flag2 = false;
					foreach (SPWebApplication webApplication in target.WebApplications)
					{
						if (webApplication.Name.ToLower() != this.PasteSiteCollectionOptions.WebApplicationName.ToLower())
						{
							continue;
						}
						flag = true;
						if (string.IsNullOrEmpty(this.PasteSiteCollectionOptions.URL))
						{
							foreach (SPPath path in webApplication.Paths)
							{
								if (path.PathValue.ToLower() != this.PasteSiteCollectionOptions.Path.ToLower())
								{
									continue;
								}
								flag1 = true;
								break;
							}
							if (!flag1)
							{
								base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The managed path supplied was not available in the web application: ", webApplication, ". Please check the name of the path and try again. Please remove any trailing or beginning \"/\" characters from the path name.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
							}
							this.PasteSiteCollectionOptions.URL = string.Concat("/", this.PasteSiteCollectionOptions.Path, "/", this.m_sName);
						}
						if (string.IsNullOrEmpty(this.PasteSiteCollectionOptions.ContentDatabaseName))
						{
							break;
						}
						foreach (SPContentDatabase contentDatabasis in webApplication.ContentDatabases)
						{
							if (contentDatabasis.Name.ToLower() != this.PasteSiteCollectionOptions.ContentDatabaseName.ToLower())
							{
								continue;
							}
							flag2 = true;
							break;
						}
						if (flag2)
						{
							break;
						}
						base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The named content database could not be found within the web application: ", webApplication, ". Please check the name of the content database and try again.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
						break;
					}
					if (!flag)
					{
						base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The specified web application name is unable to be validated on the target server. Please check the name and try again."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
					}
					SPWebTemplate item = null;
					if (this.PasteSiteOptions.WebTemplateName != null)
					{
						foreach (SPLanguage language in target.Languages)
						{
							if (this.PasteSiteCollectionOptions.LanguageCode == null || Convert.ToInt32(this.PasteSiteCollectionOptions.LanguageCode) != language.LCID)
							{
								if (language.LCID != (base.Source as SPWeb).Language)
								{
									continue;
								}
								item = language.Templates[this.PasteSiteOptions.WebTemplateName];
								break;
							}
							else
							{
								foreach (SPWebTemplate template in language.Templates)
								{
									if (template.Name != this.PasteSiteOptions.WebTemplateName)
									{
										continue;
									}
									item = template;
									break;
								}
							}
						}
						if (item == null)
						{
							base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The web template: ", this.PasteSiteOptions.WebTemplateName, " was not found on the target server. Please select a different template to map to.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
						}
					}
					else
					{
						foreach (SPLanguage sPLanguage in target.Languages)
						{
							if (this.PasteSiteCollectionOptions.LanguageCode == null || Convert.ToInt32(this.PasteSiteCollectionOptions.LanguageCode) != sPLanguage.LCID)
							{
								if (sPLanguage.LCID != (base.Source as SPWeb).Language)
								{
									continue;
								}
								foreach (SPWebTemplate sPWebTemplate in sPLanguage.Templates)
								{
									if (sPWebTemplate.Name != (base.Source as SPWeb).Template.Name)
									{
										continue;
									}
									item = sPWebTemplate;
									break;
								}
							}
							else
							{
								foreach (SPWebTemplate template1 in sPLanguage.Templates)
								{
									if (template1.Name != (base.Source as SPWeb).Template.Name)
									{
										continue;
									}
									item = template1;
									break;
								}
							}
						}
						if (item == null)
						{
							base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The web template: ", (base.Source as SPWeb).Template.Name, " was not found on the target server. Please select a different template to map to.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
						}
					}
				}
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}