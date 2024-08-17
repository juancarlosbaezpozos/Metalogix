using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration
{
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Create Site Collection... {2-Create} > 1:Admin Mode...")]
	[Name("Create Site Collection")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPBaseServer), true)]
	public class CreateSiteCollection : SharePointAction<CreateSiteCollectionOptions>
	{
		public CreateSiteCollection()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections) || targetSelections.Count <= 0)
			{
				return false;
			}
			return !(targetSelections[0] is SPTenantMySiteHost);
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
			List<string> strs = new List<string>()
			{
				"ValidateOwnerLogins",
				"MapAudiences",
				"LogSkippedItems",
				"CheckResults",
				"ForceRefresh",
				"ComparisonLevel",
				"CorrectingLinks",
				"LinkCorrectTextFields",
				"LinkCorrectionScope",
				"UseComprehensiveLinkCorrection",
				"AllowDBUserWriting",
				"MapGroupsByName",
				"OverwriteGroups",
				"OverrideCheckouts",
				"MapMissingUsersToLoginName",
				"PersistMappings"
			};
			PropertyInfo[] optionParameters = base.GetOptionParameters(cmdletOptions);
			for (int i = 0; i < (int)optionParameters.Length; i++)
			{
				PropertyInfo propertyInfo = optionParameters[i];
				if (!strs.Contains(propertyInfo.Name))
				{
					propertyInfos.Add(propertyInfo);
				}
			}
			return propertyInfos.ToArray();
		}

		private void IsValidSiteCollectionURL()
		{
			if (base.SharePointOptions.IsHostHeader)
			{
				if (!Utils.IsValidHostHeaderURL(base.SharePointOptions.HostHeaderURL))
				{
					throw new Exception(string.Concat(Resources.ValidSiteCollectionURL, " ", Utils.illegalCharactersForHostHeaderSiteUrl));
				}
			}
			else if (!Utils.IsValidSharePointURL(base.SharePointOptions.URL, false))
			{
				throw new Exception(string.Concat(Resources.ValidSiteCollectionURL, " ", Utils.IllegalCharactersForSiteUrl));
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				if (target == null)
				{
					throw new Exception("Target is null");
				}
				if (!(target[0] is SPBaseServer))
				{
					throw new Exception("Target[0] must be of SPServer type");
				}
				using (SPBaseServer item = target[0] as SPBaseServer)
				{
					this.IsValidSiteCollectionURL();
					StringBuilder stringBuilder = new StringBuilder();
					XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
					xmlTextWriter.WriteStartElement("Site");
					xmlTextWriter.WriteAttributeString("ServerRelativeUrl", string.Concat(base.SharePointOptions.Path, base.SharePointOptions.URL));
					xmlTextWriter.WriteAttributeString("Name", base.SharePointOptions.URL);
					xmlTextWriter.WriteAttributeString("Title", base.SharePointOptions.Title);
					xmlTextWriter.WriteAttributeString("Description", base.SharePointOptions.Description);
					int lCID = base.SharePointOptions.Language.LCID;
					xmlTextWriter.WriteAttributeString("Language", lCID.ToString());
					int num = base.SharePointOptions.Language.LCID;
					xmlTextWriter.WriteAttributeString("Locale", num.ToString());
					int d = base.SharePointOptions.Template.ID;
					xmlTextWriter.WriteAttributeString("WebTemplateID", d.ToString());
					int config = base.SharePointOptions.Template.Config;
					xmlTextWriter.WriteAttributeString("WebTemplateConfig", config.ToString());
					xmlTextWriter.WriteAttributeString("WebTemplateName", base.SharePointOptions.Template.Name.ToString());
					xmlTextWriter.WriteAttributeString("Owner", base.SharePointOptions.OwnerLogin);
					if (base.SharePointOptions.SecondaryOwnerLogin != null)
					{
						xmlTextWriter.WriteAttributeString("SecondaryOwner", base.SharePointOptions.SecondaryOwnerLogin);
					}
					if (base.SharePointOptions.QuotaID != null)
					{
						xmlTextWriter.WriteAttributeString("QuotaID", base.SharePointOptions.QuotaID);
					}
					long quotaMaximum = base.SharePointOptions.QuotaMaximum * (long)1048576;
					xmlTextWriter.WriteAttributeString("QuotaStorageLimit", quotaMaximum.ToString());
					long quotaWarning = base.SharePointOptions.QuotaWarning * (long)1048576;
					xmlTextWriter.WriteAttributeString("QuotaStorageWarning", quotaWarning.ToString());
					if (item.Adapter.SharePointVersion.IsSharePoint2013OrLater && base.SharePointOptions.ExperienceVersion > 0)
					{
						xmlTextWriter.WriteAttributeString("ExperienceVersion", base.SharePointOptions.ExperienceVersion.ToString());
					}
					xmlTextWriter.WriteAttributeString("StorageQuota", base.SharePointOptions.StorageQuota.ToString());
					xmlTextWriter.WriteAttributeString("ResourceQuota", base.SharePointOptions.ResourceQuota.ToString());
					xmlTextWriter.WriteAttributeString("IsHostHeader", base.SharePointOptions.IsHostHeader.ToString());
					if (!string.IsNullOrEmpty(base.SharePointOptions.HostHeaderURL))
					{
						xmlTextWriter.WriteAttributeString("HostHeaderURL", base.SharePointOptions.HostHeaderURL.ToString());
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.Flush();
					xmlTextWriter.Close();
					AddSiteCollectionOptions addSiteCollectionOption = new AddSiteCollectionOptions()
					{
						ValidateOwners = base.SharePointOptions.ValidateOwnerLogins,
						ContentDatabase = base.SharePointOptions.ContentDatabaseName,
						CopySiteQuota = base.SharePointOptions.SetSiteQuota,
						SelfServiceCreateMode = base.SharePointOptions.SelfServiceCreateMode,
						CopyAssociatedGroupSettings = false
					};
					SPSite sPSite = item.Sites.AddSiteCollection(base.SharePointOptions.WebApplication.Name, stringBuilder.ToString(), addSiteCollectionOption);
					sPSite.Dispose();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new Exception(string.Format("Message: {0}{1}Stack Trace:{2}", exception.Message, Environment.NewLine, exception.StackTrace));
			}
		}
	}
}