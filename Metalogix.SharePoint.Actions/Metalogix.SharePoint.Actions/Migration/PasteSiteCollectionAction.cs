using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[CmdletEnabled(true, "Copy-MLSharePointSiteCollection", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Incrementable(true, "Paste Site Collection Incrementally")]
	[MenuText("1:Paste Site Collection...{0-Paste} > 1:Admin Mode")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste Site Collection")]
	[SourceType(typeof(SPWeb), true)]
	[SubActionTypes(new Type[] { typeof(PasteNavigationAction), typeof(PasteRolesAction), typeof(CopyRoleAssignmentsAction), typeof(CopyListTemplateGalleryAction), typeof(CopyMasterPageGalleryAction), typeof(PasteSiteLists), typeof(CopyContentTypesAction), typeof(CopyWebPartPageAction), typeof(PasteAllSubSitesAction), typeof(CopyWorkflowAssociationsAction), typeof(CopySiteColumnsAction) })]
	[TargetType(typeof(SPBaseServer))]
	public class PasteSiteCollectionAction : PasteSiteCollectionBaseAction<PasteSiteCollectionOptions>
	{
		private SPWeb m_auditSettingsSourceWeb;

		private SPSite m_auditSettingsTargetSite;

		private string m_sAuditSettingsUpdateXml;

		public PasteSiteCollectionAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections) || sourceSelections.Count <= 0 || !(sourceSelections[0] is SPWeb) || targetSelections.Count <= 0)
			{
				return false;
			}
			if (!(targetSelections[0] is SPBaseServer))
			{
				return false;
			}
			return !(targetSelections[0] is SPTenantMySiteHost);
		}

		private void BufferAuditSettingsUpdate(SPWeb sourceWeb, SPSite targetSite, string sSourceXml)
		{
			this.m_auditSettingsSourceWeb = sourceWeb;
			this.m_auditSettingsTargetSite = targetSite;
			this.m_sAuditSettingsUpdateXml = sSourceXml;
		}

		private void CheckSourceAndTarget(IXMLAbleList source, SPBaseServer targetServer)
		{
			if (base.SharePointOptions.OverwriteSites)
			{
				SPWeb item = source[0] as SPWeb;
				if (item != null)
				{
					string str = this.CreateTargetSiteCollectionUrl(targetServer).Replace("%2F", "/");
					if (item.Url.Equals(str, StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception(Metalogix.Actions.Properties.Resources.CannotRunTargetIsSameAsSource);
					}
				}
			}
		}

		public void CopySiteCollection(SPWeb sourceWeb, SPBaseServer targetServer)
		{
			SPWebTemplateCollection sPWebTemplateCollections;
			if (base.CheckForAbort())
			{
				return;
			}
			if (base.SharePointOptions.FilterSites && !base.SharePointOptions.SiteFilterExpression.Evaluate(sourceWeb, new CompareDatesInUtc()))
			{
				return;
			}
			this.PopulateLinkCorrector(sourceWeb, targetServer);
			LogItem logItem = null;
			SPSite sPSite = null;
			string siteXML = null;
			bool flag = false;
			try
			{
				try
				{
					logItem = new LogItem("Copy Site Collection", sourceWeb.Name, sourceWeb.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					siteXML = base.GetSiteXML(sourceWeb, targetServer);
					if (siteXML != null)
					{
						SPLanguage item = targetServer.Languages[base.SharePointOptions.LanguageCode];
						sPWebTemplateCollections = (!item.HasMultipleExperienceVersions || !item.ExperienceVersions.Contains(base.SharePointOptions.ExperienceVersion) ? item.Templates : item.GetTemplatesForExperienceVersion(base.SharePointOptions.ExperienceVersion));
						siteXML = base.MapWebTemplate(siteXML, sourceWeb.Template.Name, sPWebTemplateCollections, true);
						if (base.SharePointOptions.CorrectingLinks && base.LinkCorrector != null)
						{
							siteXML = base.LinkCorrector.UpdateLinksInWeb(sourceWeb, siteXML);
						}
						if (base.SharePointOptions.RenameSpecificNodes)
						{
							foreach (TransformationTask taskCollection in base.SharePointOptions.TaskCollection)
							{
								if (!taskCollection.ApplyTo.Evaluate(sourceWeb, new CompareDatesInUtc()))
								{
									continue;
								}
								siteXML = taskCollection.PerformTransformation(siteXML);
							}
						}
						string str = this.CreateTargetSiteCollectionUrl(targetServer);
						SPSite item1 = (SPSite)targetServer.Sites[str];
						bool flag1 = false;
						if (item1 != null)
						{
							if (base.SharePointOptions.OverwriteSites)
							{
								if (base.SharePointOptions.IsHostHeader)
								{
									if (!((SPBaseServer)item1.Parent).Sites.DeleteHostHeaderSiteCollection(item1))
									{
										throw new Exception(string.Format("Could not overwrite the site collection '{0}' on the target because it could not be deleted.", item1.LinkableUrl));
									}
								}
								else if (!((SPBaseServer)item1.Parent).Sites.DeleteSiteCollection(item1))
								{
									throw new Exception(string.Format("Could not overwrite the site collection '{0}' on the target because it could not be deleted.", item1.LinkableUrl));
								}
								item1 = null;
							}
							else if (base.SharePointOptions.UpdateSiteOptionsBitField <= 0)
							{
								flag1 = true;
								logItem.Operation = "Skipping Site";
								logItem.Information = "The site collection was skipped because sites are not being overwritten and there was nothing to update for the site's metadata. Content underneath the site may still be copied depending on your option settings.";
								logItem.Status = ActionOperationStatus.Skipped;
								base.FireOperationUpdated(logItem);
							}
						}
						if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2007 && targetServer.Adapter.SharePointVersion.IsSharePoint2010OrLater && (base.SharePointOptions.CopyListOOBWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations))
						{
							siteXML = base.AddLegacyFeaturesToSiteCollection(siteXML);
						}
						if (item1 != null)
						{
							sPSite = item1;
							if (!flag1)
							{
								UpdateWebOptions updateWebOption = new UpdateWebOptions();
								int updateSiteOptionsBitField = base.SharePointOptions.UpdateSiteOptionsBitField;
								updateWebOption.CopyCoreMetaData = (updateSiteOptionsBitField & 1) > 0;
								bool flag2 = (!base.SharePointOptions.CopySiteFeatures ? false : (updateSiteOptionsBitField & 2048) > 0);
								updateWebOption.CopyFeatures = flag2;
								updateWebOption.MergeFeatures = (!flag2 ? false : base.SharePointOptions.MergeSiteFeatures);
								updateWebOption.CopyNavigation = (!base.SharePointOptions.CopyNavigation ? false : (updateSiteOptionsBitField & 32) > 0);
								updateWebOption.ApplyMasterPage = false;
								updateWebOption.ApplyTheme = (updateSiteOptionsBitField & 512) > 0;
								updateWebOption.CopyAccessRequestSettings = (updateSiteOptionsBitField & 1024) > 0;
								updateWebOption.CopyAssociatedGroupSettings = ((updateSiteOptionsBitField & 8192) <= 0 ? false : base.SharePointOptions.CopyAssociatedGroups);
								if (updateWebOption.CopyAssociatedGroupSettings)
								{
									siteXML = base.MapReferencedGroups(siteXML, sourceWeb, sPSite);
								}
								sPSite.Update(siteXML, updateWebOption);
							}
						}
						else
						{
							AddSiteCollectionOptions addSiteCollectionOption = new AddSiteCollectionOptions()
							{
								Overwrite = base.SharePointOptions.OverwriteSites,
								MergeFeatures = base.SharePointOptions.MergeSiteFeatures,
								CopyFeatures = base.SharePointOptions.CopySiteFeatures,
								ApplyTheme = base.SharePointOptions.ApplyThemeToWeb,
								ApplyMasterPage = false,
								ContentDatabase = base.SharePointOptions.ContentDatabaseName,
								CopyNavigation = base.SharePointOptions.CopyNavigation,
								CopySiteAdmins = base.SharePointOptions.CopySiteAdmins,
								CopySiteQuota = base.SharePointOptions.CopySiteQuota,
								CopyAccessRequestSettings = (!base.SharePointOptions.CopyAccessRequestSettings ? false : base.SharePointOptions.CopyRootPermissions),
								SelfServiceCreateMode = base.SharePointOptions.SelfServiceCreateMode
							};
							if (base.SharePointOptions.PreserveUIVersion && sourceWeb.UIVersion == "3" && targetServer.Adapter.SharePointVersion.IsSharePoint2010OrLater)
							{
								addSiteCollectionOption.PreserveUIVersion = base.SharePointOptions.PreserveUIVersion;
							}
							siteXML = this.MapSiteAdministrators(siteXML, logItem, targetServer.Adapter.SharePointVersion.IsSharePointOnline);
							sPSite = targetServer.Sites.AddSiteCollection(base.SharePointOptions.WebApplicationName, siteXML, addSiteCollectionOption);
							if (base.SharePointOptions.CopyAssociatedGroups)
							{
								LogItem logItem1 = new LogItem("Setting associated groups", sPSite.Name, sourceWeb.DisplayUrl, sPSite.DisplayUrl, ActionOperationStatus.Running);
								try
								{
									siteXML = base.MapReferencedGroups(siteXML, sourceWeb, sPSite);
									UpdateWebOptions updateWebOption1 = new UpdateWebOptions()
									{
										ApplyMasterPage = false,
										ApplyTheme = false,
										CopyAccessRequestSettings = false,
										CopyCoreMetaData = false,
										CopyFeatures = false,
										CopyNavigation = false,
										CopyAssociatedGroupSettings = true
									};
									sPSite.Update(siteXML, updateWebOption1);
									logItem1.Status = ActionOperationStatus.Completed;
								}
								catch (Exception exception)
								{
									logItem1.Exception = exception;
									base.FireOperationStarted(logItem1);
									base.FireOperationFinished(logItem1);
								}
							}
							flag = true;
						}
						base.AddGuidMappings(sourceWeb.ID, sPSite.ID);
						base.QueueAccessRequestListCopies(sourceWeb, sPSite, base.SharePointOptions);
						this.UpdateUserInformationListMetadata(sourceWeb.UserInformationList, sPSite.RootWeb.UserInformationList);
						if (!flag1)
						{
							if (!base.SharePointOptions.CheckResults)
							{
								logItem.Status = ActionOperationStatus.Completed;
							}
							else
							{
								base.CompareNodes(sourceWeb, sPSite, logItem);
							}
							if (flag)
							{
								LogItem licenseDataUsed = logItem;
								licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + SPObjectSizes.GetObjectSize(sPSite);
							}
							logItem.AddCompletionDetail(Metalogix.SharePoint.Actions.Properties.Resources.Migration_Detail_SitesCopied, (long)1);
						}
						if (base.SharePointOptions.Verbose)
						{
							logItem.SourceContent = sourceWeb.XML;
							logItem.TargetContent = sPSite.XML;
						}
					}
					else
					{
						logItem.Operation = "Skipping Site Collection";
						logItem.Status = ActionOperationStatus.Skipped;
						logItem.Information = "Site Collection skipped by transformation";
						return;
					}
				}
				catch (Exception exception2)
				{
					Exception exception1 = exception2;
					logItem.Exception = exception1;
					logItem.Details = exception1.StackTrace;
					logItem.SourceContent = siteXML;
					return;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
			try
			{
				if (sPSite != null)
				{
					if (base.SharePointOptions.CopyReferencedManagedMetadata)
					{
						base.EnsureManagedMetadataExistence(sourceWeb.AvailableColumns, sourceWeb, sPSite, base.SharePointOptions.ResolveManagedMetadataByName);
					}
					if (base.SharePointOptions.RunNavigationStructureCopy)
					{
						base.SourceTargetNavCopyMap.Add(sourceWeb, sPSite);
					}
					base.CopySiteCollectionInformation(sourceWeb, sPSite, false, flag);
					base.CopySubSiteAndListContent(sourceWeb, sPSite, false, flag);
					if (base.SharePointOptions.CopyAuditSettings)
					{
						this.BufferAuditSettingsUpdate(sourceWeb, sPSite, siteXML);
					}
					Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
					string name = typeof(CopyRoleAssignmentsAction).Name;
					Guid guid = new Guid();
					threadManager.SetBufferedTasks(string.Concat(name, guid.ToString()), false, false);
				}
			}
			finally
			{
				if (flag)
				{
					this.EmptyRecycleBin(sPSite);
				}
			}
		}

		private string CreateTargetSiteCollectionUrl(SPBaseServer targetServer)
		{
			if (targetServer == null)
			{
				return "";
			}
			if (!(targetServer.WebApplications[base.SharePointOptions.WebApplicationName] is SPWebApplication))
			{
				throw new Exception(string.Format(Metalogix.SharePoint.Actions.Properties.Resources.WebAppNotFoundMessage, base.SharePointOptions.WebApplicationName));
			}
			string str = targetServer.WebApplications[base.SharePointOptions.WebApplicationName].Url.TrimEnd(new char[] { '/' });
			string uRL = base.SharePointOptions.URL;
			char[] chrArray = new char[] { '/' };
			str = string.Concat(str, "/", uRL.TrimStart(chrArray));
			if (base.SharePointOptions.IsHostHeader)
			{
				str = string.Concat(base.SharePointOptions.HostHeaderURL, "/");
			}
			return str.Replace("/", "%2F");
		}

		private void EmptyRecycleBin(SPSite targetSite)
		{
			if (targetSite.Adapter.SharePointVersion.IsSharePointOnline)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
				{
					xmlWriter.WriteStartElement("SiteCollection");
					xmlWriter.WriteAttributeString("EmptyRecycleBin", "true");
					xmlWriter.WriteEndElement();
				}
				targetSite.Adapter.Writer.UpdateSiteCollectionSettings(stringBuilder.ToString(), null);
			}
		}

		private void FlushAuditSettingsUpdate()
		{
			ActionOperationStatus actionOperationStatu;
			if (this.m_auditSettingsSourceWeb == null || this.m_auditSettingsTargetSite == null || string.IsNullOrEmpty(this.m_sAuditSettingsUpdateXml))
			{
				this.m_auditSettingsSourceWeb = null;
				this.m_auditSettingsTargetSite = null;
				this.m_sAuditSettingsUpdateXml = null;
				return;
			}
			LogItem logItem = new LogItem("Copying Audit Settings", this.m_auditSettingsTargetSite.Name, this.m_auditSettingsSourceWeb.DisplayUrl, this.m_auditSettingsTargetSite.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					UpdateSiteCollectionOptions updateSiteCollectionOption = new UpdateSiteCollectionOptions()
					{
						ClearExistingSiteAdmins = false,
						UpdateSiteAdmins = false,
						UpdateSiteQuota = false,
						UpdateSiteAuditSettings = true
					};
					OperationReportingResult operationReportingResult = this.m_auditSettingsTargetSite.UpdateSiteCollectionSettings(this.m_sAuditSettingsUpdateXml, updateSiteCollectionOption);
					if (operationReportingResult == null)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						logItem.Details = operationReportingResult.AllReportElementsAsString;
						LogItem logItem1 = logItem;
						if (operationReportingResult.ErrorOccured)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (operationReportingResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem1.Status = actionOperationStatu;
					}
					if (logItem.Status != ActionOperationStatus.Completed)
					{
						logItem.Information = "Issues have been encountered, please review details.";
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
					logItem.SourceContent = this.m_sAuditSettingsUpdateXml;
				}
			}
			finally
			{
				this.m_auditSettingsSourceWeb = null;
				this.m_auditSettingsTargetSite = null;
				this.m_sAuditSettingsUpdateXml = null;
				base.FireOperationFinished(logItem);
			}
		}

		public void InitializeActionConfiguration(SPWeb sourceWeb, SPBaseServer targetServer)
		{
			if (targetServer != null && targetServer.WebApplication != null)
			{
				if (!string.IsNullOrEmpty(base.SharePointOptions.WebApplicationName))
				{
					bool flag = false;
					foreach (SPWebApplication webApplication in targetServer.WebApplications)
					{
						if (base.SharePointOptions.WebApplicationName != webApplication.Name)
						{
							continue;
						}
						flag = true;
						break;
					}
					if (!flag)
					{
						base.SharePointOptions.WebApplicationName = targetServer.WebApplication.Name;
					}
				}
				else
				{
					base.SharePointOptions.WebApplicationName = targetServer.WebApplication.Name;
				}
			}
			if (base.SharePointOptions.WebTemplateName == null)
			{
				int num = sourceWeb.DisplayUrl.LastIndexOf("/");
				string str = "";
				try
				{
					str = sourceWeb.DisplayUrl.Substring(num + 1);
					base.SharePointOptions.Name = str;
					base.SharePointOptions.SourceUrlName = sourceWeb.ServerRelativeUrl;
				}
				catch
				{
				}
				try
				{
					if (sourceWeb.IsChildOf(targetServer))
					{
						base.SharePointOptions.IsSameServer = true;
					}
					string xML = sourceWeb.RootSite.XML;
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xML);
					XmlNode firstChild = xmlDocument.FirstChild;
					if (firstChild.Attributes["Owner"] != null)
					{
						base.SharePointOptions.OwnerLogin = firstChild.Attributes["Owner"].Value;
					}
					if (firstChild.Attributes["SecondaryOwner"] != null)
					{
						base.SharePointOptions.SecondaryOwnerLogin = firstChild.Attributes["SecondaryOwner"].Value;
					}
					if (firstChild.Attributes["SiteCollectionAdministrators"] != null)
					{
						base.SharePointOptions.SiteCollectionAdministrators = firstChild.Attributes["SiteCollectionAdministrators"].Value;
					}
					if (firstChild.Attributes["WebApplication"] != null)
					{
						base.SharePointOptions.SourceWebApplication = base.SharePointOptions.WebApplicationName;
					}
					if (firstChild.Attributes["ManagedPath"] != null)
					{
						base.SharePointOptions.Path = firstChild.Attributes["ManagedPath"].Value;
						base.SharePointOptions.SourcePath = base.SharePointOptions.Path;
					}
				}
				catch
				{
				}
				base.SharePointOptions.WebTemplateName = sourceWeb.Template.Name;
				base.SharePointOptions.ChangeWebTemplate = true;
				SPLanguage item = targetServer.Languages[sourceWeb.Language.ToString()];
				SPWebTemplateCollection templates = null;
				if (item != null)
				{
					templates = item.Templates;
				}
				else if (targetServer.Languages.Count > 0)
				{
					item = targetServer.Languages[0];
					templates = item.Templates;
				}
				if (item != null)
				{
					base.SharePointOptions.LanguageCode = item.LCID.ToString();
				}
				if (templates != null)
				{
					PasteSiteAction.InitializeMappings(base.SharePointOptions, sourceWeb, templates);
				}
			}
		}

		private void IsValidSiteCollectionURL()
		{
			if (!base.SharePointOptions.IsHostHeader)
			{
				char[] chrArray = new char[] { '/' };
				string[] strArrays = base.SharePointOptions.URL.Split(chrArray);
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					if (!Utils.IsValidSharePointURL(strArrays[i], false))
					{
						throw new Exception(string.Concat(Metalogix.Actions.Properties.Resources.ValidSiteCollectionURL, " ", Utils.IllegalCharactersForSiteUrl));
					}
				}
			}
			else if (!Utils.IsValidHostHeaderURL(base.SharePointOptions.HostHeaderURL))
			{
				throw new Exception(string.Concat(Metalogix.Actions.Properties.Resources.ValidSiteCollectionURL, " ", Utils.illegalCharactersForHostHeaderSiteUrl));
			}
		}

		private string MapSiteAdministrators(string sourceXML, LogItem copySiteOperation, bool isTargetSharePointOnline)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(sourceXML);
				XmlNode xmlNodes = xmlDocument.SelectSingleNode("./Site");
				StringBuilder stringBuilder = new StringBuilder();
				string attributeValueAsString = xmlNodes.GetAttributeValueAsString("SiteCollectionAdministrators");
				if (!string.IsNullOrEmpty(attributeValueAsString))
				{
					string[] strArrays = new string[] { ";" };
					string[] strArrays1 = attributeValueAsString.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < (int)strArrays1.Length; i++)
					{
						string str = strArrays1[i];
						if (!string.IsNullOrEmpty(str))
						{
							stringBuilder.Append(SPGlobalMappings.Map(str, isTargetSharePointOnline));
							stringBuilder.Append(";");
						}
					}
					if (!string.IsNullOrEmpty(stringBuilder.ToString()))
					{
						XmlAttribute itemOf = xmlNodes.Attributes["SiteCollectionAdministrators"];
						string str1 = stringBuilder.ToString();
						char[] chrArray = new char[] { ';' };
						itemOf.Value = str1.Trim(chrArray);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str2 = string.Format("{0}An error occured while mapping site collection administrators. Error: '{1}'", Environment.NewLine, exception.ToString());
				copySiteOperation.Details = string.Concat(copySiteOperation.Details, str2);
			}
			return xmlDocument.OuterXml;
		}

		private void PopulateLinkCorrector(SPWeb sourceWeb, SPBaseServer targetServer)
		{
			if (base.SharePointOptions.CorrectingLinks)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						if (!base.SharePointOptions.UseComprehensiveLinkCorrection || !base.LinkCorrector.MappingsFullyGenerated)
						{
							if (base.SharePointOptions.UseComprehensiveLinkCorrection && !base.LinkCorrector.MappingsFullyGenerated)
							{
								logItem = new LogItem("Comprehensive Link Mapping Generation for Site Copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem);
							}
							base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
							SPWebApplication item = (SPWebApplication)targetServer.WebApplications[base.SharePointOptions.WebApplicationName];
							string uRL = base.SharePointOptions.URL;
							if (!base.SharePointOptions.IsHostHeader)
							{
								base.LinkCorrector.PopulateForSiteCollectionCopy(sourceWeb, item, uRL, base.SharePointOptions.TaskCollection, base.SharePointOptions.UseComprehensiveLinkCorrection, null);
							}
							else
							{
								base.LinkCorrector.PopulateForSiteCollectionCopy(sourceWeb, item, uRL, base.SharePointOptions.TaskCollection, base.SharePointOptions.UseComprehensiveLinkCorrection, base.SharePointOptions.HostHeaderURL);
							}
							if (logItem != null)
							{
								logItem.Status = ActionOperationStatus.Completed;
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem == null)
						{
							logItem = new LogItem("Initialize Link Corrector for Site Collection Copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
						}
						logItem.Exception = exception;
						logItem.Details = exception.StackTrace;
					}
				}
				finally
				{
					if (logItem != null)
					{
						base.FireOperationFinished(logItem);
					}
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPBaseServer item = target[0] as SPBaseServer;
			if (item == null)
			{
				throw new ArgumentException("Target is not an SPServer");
			}
			this.IsValidSiteCollectionURL();
			if (!AdapterConfigurationVariables.AllowDuplicateSiteCollection)
			{
				this.CheckSourceAndTarget(source, item);
			}
			base.SharePointOptions.ChangeWebTemplate = !string.IsNullOrEmpty(base.SharePointOptions.WebTemplateName);
			base.InitializeWorkflow();
			this.InitializeSharePointCopy(source, target, false);
			foreach (SPWeb sPWeb in source)
			{
				SPWeb parent = sPWeb.Parent as SPWeb;
				SPWebCollection sPAdHocWebCollection = null;
				if (parent == null)
				{
					sPAdHocWebCollection = new SPAdHocWebCollection(sPWeb);
				}
				else
				{
					sPAdHocWebCollection = parent.SubWebs;
				}
				try
				{
					try
					{
						base.SiteTransformerDefinition.BeginTransformation(this, sPAdHocWebCollection, item.Sites, this.Options.Transformers);
						if (base.SharePointOptions.ForceRefresh)
						{
							base.RefreshSiteCollection(sPWeb, item);
						}
						this.CopySiteCollection(sPWeb, item);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						LogItem logItem = new LogItem("Error copying site collection", "", sPWeb.DisplayUrl, item.DisplayUrl, ActionOperationStatus.Failed)
						{
							Exception = exception
						};
						base.FireOperationStarted(logItem);
						base.FireOperationFinished(logItem);
					}
				}
				finally
				{
					base.SiteTransformerDefinition.EndTransformation(this, sPAdHocWebCollection, item.Sites, this.Options.Transformers);
					sPWeb.Dispose();
				}
			}
			base.CopySiteNavigationStructure(item);
			base.StartCommonWorkflowBufferedTasks();
			base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
			base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}

		protected override void RunPostOp(IXMLAbleList source, IXMLAbleList target)
		{
			this.FlushAuditSettingsUpdate();
		}

		private void UpdateUserInformationListMetadata(SPList sourceGallery, SPList targetGallery)
		{
			if (sourceGallery == null || targetGallery == null)
			{
				return;
			}
			LogItem logItem = new LogItem("Updating User Information List Settings", sourceGallery.Name, sourceGallery.DisplayUrl, targetGallery.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceGallery.XML);
				XmlNode xmlNodes = XmlUtility.StringToXmlNode(targetGallery.XML);
				string[] strArrays = new string[] { "ReadSecurity", "WriteSecurity", "EnableAttachments", "NoCrawl" };
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					if (xmlNode.Attributes != null && xmlNode.Attributes[str] != null)
					{
						((XmlElement)xmlNodes).SetAttribute(str, xmlNode.Attributes[str].Value);
					}
				}
				targetGallery.UpdateList(xmlNodes.OuterXml, false, false);
				logItem.Status = ActionOperationStatus.Completed;
			}
			catch (Exception exception)
			{
				logItem.Exception = exception;
			}
			base.FireOperationFinished(logItem);
		}
	}
}