using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointSiteCollectionWithConfigFile")]
	public class CopySiteCollectionWithConfigFileCmdlet : SharePointActionWithConfigFileCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteCollectionAction);
			}
		}

		private Metalogix.SharePoint.Options.Migration.PasteSiteCollectionOptions PasteSiteCollectionOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteSiteCollectionOptions;
			}
		}

		private Metalogix.SharePoint.Options.Migration.PasteSiteOptions PasteSiteOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteSiteOptions;
			}
		}

		public CopySiteCollectionWithConfigFileCmdlet()
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
							this.PasteSiteCollectionOptions.URL = string.Concat("/", this.PasteSiteCollectionOptions.Path, "/", base.Source.Name);
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
	}
}