using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MandatoryTransformers(new Type[] { typeof(SiteCollectionFeatureEnforcer) })]
	[SubActionTypes(new Type[] { typeof(PasteSiteAction) })]
	public abstract class PasteSiteCollectionBaseAction<T> : PasteSiteBaseAction<T>
	where T : PasteSiteCollectionOptions
	{
		protected TransformerDefinition<SPWeb, PasteSiteCollectionAction, SPWebCollection, SPSiteCollection> _siteTransformerDefinition;

		internal TransformerDefinition<SPWeb, PasteSiteCollectionAction, SPWebCollection, SPSiteCollection> SiteTransformerDefinition
		{
			get
			{
				return this._siteTransformerDefinition;
			}
		}

		protected PasteSiteCollectionBaseAction()
		{
		}

		private void AddChildNodesToSiteXml(SPWeb sourceWeb, XmlNode node)
		{
			try
			{
				XmlNode nodeXML = sourceWeb.GetNodeXML();
				if (nodeXML != null && nodeXML.ChildNodes.Count > 0)
				{
					foreach (XmlNode childNode in nodeXML.ChildNodes)
					{
						XmlNode xmlNodes = node.OwnerDocument.ImportNode(childNode, true);
						node.AppendChild(xmlNodes);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Format("Error occurred while adding child nodes to site xml for site '{0}'. Error '{1}'", sourceWeb.Url, exception.ToString()));
			}
		}

		protected string AddLegacyFeaturesToSiteCollection(string sSourceXml)
		{
			string str;
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(sSourceXml);
				if (xmlDocument.DocumentElement.Attributes["SiteCollFeatures"] != null)
				{
					if (!string.IsNullOrEmpty(xmlDocument.DocumentElement.Attributes["SiteCollFeatures"].Value))
					{
						XmlAttribute itemOf = xmlDocument.DocumentElement.Attributes["SiteCollFeatures"];
						itemOf.Value = string.Concat(itemOf.Value, ",6c09612b-46af-4b2f-8dfc-59185c962a29,02464c6a-9d07-4f30-ba04-e9035cf54392,c845ed8d-9ce5-448c-bd3e-ea71350ce45b");
					}
					else
					{
						xmlDocument.DocumentElement.Attributes["SiteCollFeatures"].Value = "6c09612b-46af-4b2f-8dfc-59185c962a29,02464c6a-9d07-4f30-ba04-e9035cf54392,c845ed8d-9ce5-448c-bd3e-ea71350ce45b";
					}
				}
				return xmlDocument.DocumentElement.OuterXml;
			}
			catch
			{
				str = sSourceXml;
			}
			return str;
		}

		private void AddOrUpdateAttributeValue(string sAttributeName, object oValue, XmlNode nodeToUpdate)
		{
			if (nodeToUpdate.Attributes[sAttributeName] != null)
			{
				nodeToUpdate.Attributes[sAttributeName].Value = (oValue == null ? "" : oValue.ToString());
				return;
			}
			XmlAttribute xmlAttribute = nodeToUpdate.OwnerDocument.CreateAttribute(sAttributeName);
			xmlAttribute.Value = (oValue == null ? "" : oValue.ToString());
			nodeToUpdate.Attributes.Append(xmlAttribute);
		}

		protected void CopyMasterPageGallery(object[] oParams)
		{
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPSite sPSite = oParams[1] as SPSite;
			bool flag = (bool)oParams[2];
			if (flag && base.SharePointOptions.CopyMasterPageGallery || !flag && (base.SharePointOptions.UpdateSiteOptionsBitField & 256) > 0)
			{
				CopyMasterPageGalleryAction copyMasterPageGalleryAction = new CopyMasterPageGalleryAction();
				base.SubActions.Add(copyMasterPageGalleryAction);
				copyMasterPageGalleryAction.Options.SetFromOptions(base.SharePointOptions);
				TransformerCollection transformers = copyMasterPageGalleryAction.Options.Transformers;
				transformers.Remove(transformers.Find("Page Layout GUID Mapping"));
				object[] masterPageGallery = new object[] { sPWeb.RootSite.MasterPageGallery, sPSite.MasterPageGallery };
				copyMasterPageGalleryAction.RunAsSubAction(masterPageGallery, new ActionContext(sPWeb, sPSite), null);
			}
			if (base.SharePointOptions.PreserveMasterPage)
			{
				base.CopyMasterPageSetting(sPWeb, sPSite, base.LinkCorrector.UpdateLinksInWeb(sPWeb, sPWeb.XML));
			}
			if (((sPWeb.Adapter.SharePointVersion.IsSharePoint2013 || sPWeb.Adapter.SharePointVersion.IsSharePoint2016) && (sPSite.Adapter.SharePointVersion.IsSharePoint2013 || sPSite.Adapter.SharePointVersion.IsSharePoint2016) ? base.SharePointOptions.ApplyThemeToWeb : false))
			{
				this.Set2013Theme(sPWeb, sPSite);
			}
		}

		public void CopySiteCollectionInformation(SPWeb sourceWeb, SPSite addedSite, bool bIsMySite, bool bSiteNewlyAdded)
		{
			bool flag;
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				addedSite.Dispose();
				return;
			}
			if (bIsMySite)
			{
				flag = true;
			}
			else if (!base.SharePointOptions.CopyPermissionLevels)
			{
				flag = false;
			}
			else if (bSiteNewlyAdded)
			{
				flag = true;
			}
			else
			{
				flag = (bSiteNewlyAdded || !base.SharePointOptions.UpdateSites ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 8) > 0);
			}
			if (flag)
			{
				PasteRolesAction pasteRolesAction = new PasteRolesAction();
				base.SubActions.Add(pasteRolesAction);
				pasteRolesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				object[] roles = new object[] { sourceWeb.Roles as SPRoleCollection, addedSite.Roles as SPRoleCollection };
				pasteRolesAction.RunAsSubAction(roles, new ActionContext(sourceWeb, addedSite), null);
			}
			string str = null;
			str = (!bIsMySite ? (new Guid()).ToString() : sourceWeb.Name);
			if (base.SharePointOptions.CopySitePermissions || base.SharePointOptions.CopyListPermissions || base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
			{
				if (!addedSite.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload)
				{
					Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
					string str1 = string.Concat(typeof(CopyRoleAssignmentsAction).Name, str);
					object[] objArray = new object[] { sourceWeb, addedSite, bIsMySite, bSiteNewlyAdded };
					threadManager.QueueBufferedTask(str1, objArray, new ThreadedOperationDelegate(this.CopySiteCollectionPermissionsTaskDelegate));
				}
				else
				{
					object[] objArray1 = new object[] { sourceWeb, addedSite, bIsMySite, bSiteNewlyAdded };
					this.CopySiteCollectionPermissionsTaskDelegate(objArray1);
				}
			}
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				addedSite.Dispose();
				return;
			}
			ThreadedOperationDelegate threadedOperationDelegate = new ThreadedOperationDelegate(this.CopyMasterPageGallery);
			object[] objArray2 = new object[] { sourceWeb, addedSite, bSiteNewlyAdded };
			TaskDefinition taskDefinition = new TaskDefinition(threadedOperationDelegate, objArray2);
			base.ThreadManager.QueueBufferedTask(string.Concat(typeof(CopyMasterPageGalleryAction).Name, addedSite.ID), taskDefinition);
			base.InitializeAudienceMappings(sourceWeb, addedSite);
		}

		private void CopySiteCollectionPermissionsTaskDelegate(object[] oParams)
		{
			bool flag;
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPSite sPSite = oParams[1] as SPSite;
			bool flag1 = (bool)oParams[2];
			bool flag2 = (bool)oParams[3];
			if (flag1)
			{
				flag = true;
			}
			else if (!base.SharePointOptions.CopySitePermissions)
			{
				flag = false;
			}
			else if (flag2)
			{
				flag = true;
			}
			else
			{
				flag = (flag2 || !base.SharePointOptions.UpdateSites ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 4) > 0);
			}
			if (flag)
			{
				CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
				base.SubActions.Add(copyRoleAssignmentsAction);
				copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				object[] objArray = new object[] { sPWeb, sPSite, !flag2 };
				copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sPWeb, sPSite), null);
			}
			base.ThreadManager.SetBufferedTasks(base.PermissionsKeyFormatter.GetKeyFor(sPSite), false, false);
			base.ThreadManager.SetBufferedTasks(base.GetAccessRequestListCopyBufferKey(sPSite), false, false);
		}

		protected void CopySubSiteAndListContent(SPWeb sourceWeb, SPSite addedSite, bool bIsMySite, bool bSiteNewlyAdded)
		{
			bool flag;
			bool flag1;
			ActionContext actionContext = new ActionContext(sourceWeb, addedSite);
			SPWebCollection subWebs = sourceWeb.SubWebs;
			SPListCollection lists = sourceWeb.Lists;
			if (!bSiteNewlyAdded || !base.SharePointOptions.CopySiteColumns)
			{
				flag = (bSiteNewlyAdded ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 2) > 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (bIsMySite || bSiteNewlyAdded && base.SharePointOptions.CopyContentTypes)
			{
				flag1 = true;
			}
			else
			{
				flag1 = (bSiteNewlyAdded || !base.SharePointOptions.CopyContentTypes ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 16) > 0);
			}
			bool flag3 = flag1;
			if ((base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyListSharePointDesignerNintexWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations) && addedSite.GetExternalConnectionsOfType<NintexExternalConnection>(true).Count > 0)
			{
				this.SetNintexStorageDatabaseEntry(addedSite);
			}
			if (base.SharePointOptions.CopyListTemplateGallery)
			{
				CopyListTemplateGalleryAction copyListTemplateGalleryAction = new CopyListTemplateGalleryAction();
				copyListTemplateGalleryAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(copyListTemplateGalleryAction);
				object[] objArray = new object[] { sourceWeb, addedSite };
				copyListTemplateGalleryAction.RunAsSubAction(objArray, actionContext, null);
			}
			if (!base.SharePointOptions.CopyLists)
			{
				if (flag2)
				{
					SPFieldCollection availableColumns = sourceWeb.GetAvailableColumns(false);
					if (base.CheckForAbort())
					{
						sourceWeb.Dispose();
						addedSite.Dispose();
						return;
					}
					CopySiteColumnsAction copySiteColumnsAction = new CopySiteColumnsAction();
					copySiteColumnsAction.Options.SetFromOptions(this.Options);
					this.ConnectSubaction(copySiteColumnsAction);
					object[] siteFieldsFilterExpression = new object[] { availableColumns, (this.Options as PasteSiteOptions).SiteFieldsFilterExpression, addedSite.GetAvailableColumns(false), (this.Options as PasteSiteOptions).TermstoreNameMappingTable, false };
					copySiteColumnsAction.RunAsSubAction(siteFieldsFilterExpression, actionContext, null);
				}
				if (base.CheckForAbort())
				{
					sourceWeb.Dispose();
					addedSite.Dispose();
					return;
				}
				if (flag3)
				{
					CopyContentTypesAction copyContentTypesAction = new CopyContentTypesAction();
					copyContentTypesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyContentTypesAction);
					object[] contentTypes = new object[] { sourceWeb.ContentTypes, addedSite.ContentTypes };
					copyContentTypesAction.RunAsSubAction(contentTypes, actionContext, null);
				}
				base.ThreadManager.SetBufferedTasks(string.Concat(typeof(CopyMasterPageGalleryAction).Name, addedSite.ID), false, true);
			}
			else
			{
				PasteSiteLists pasteSiteList = new PasteSiteLists();
				pasteSiteList.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteSiteList);
				object[] objArray1 = new object[] { lists, addedSite, true, flag2, flag3 };
				pasteSiteList.RunAsSubAction(objArray1, actionContext, null);
			}
			base.MigrateCustomContent(sourceWeb, addedSite.RootWeb);
			if (bSiteNewlyAdded || base.SharePointOptions.UpdateSites && (base.SharePointOptions.UpdateSiteOptionsBitField & 1) > 0)
			{
				base.SetWelcomePage(sourceWeb, addedSite);
			}
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				addedSite.Dispose();
				return;
			}
			sourceWeb.Dispose();
			addedSite.Dispose();
			if (base.SharePointOptions.RecursivelyCopySubsites)
			{
				PasteAllSubSitesAction pasteAllSubSitesAction = new PasteAllSubSitesAction();
				pasteAllSubSitesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteAllSubSitesAction);
				object[] objArray2 = new object[] { sourceWeb, addedSite, true };
				pasteAllSubSitesAction.RunAsSubAction(objArray2, actionContext, null);
			}
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				addedSite.Dispose();
				return;
			}
			if (bSiteNewlyAdded && base.SharePointOptions.CopySiteWebParts || !bSiteNewlyAdded && base.SharePointOptions.CopySiteWebParts && (base.SharePointOptions.UpdateSiteOptionsBitField & 64) > 0)
			{
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				object[] objArray3 = new object[] { sourceWeb, addedSite };
				threadManager.QueueTask(objArray3, new ThreadedOperationDelegate(this.CopyWelcomePageWebPartsTaskDelegate));
			}
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				addedSite.Dispose();
				return;
			}
			base.ThreadManager.SetBufferedTasks(base.GetWebPartCopyBufferKey(addedSite), false, false);
			if ((bSiteNewlyAdded || (base.SharePointOptions.UpdateSiteOptionsBitField & 4096) > 0) && base.SharePointOptions.CopyContentTypes && (base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations))
			{
				base.ThreadManager.QueueBufferedTask(typeof(CopyWorkflowAssociationsAction).Name, null, (object[] oParams) => {
					try
					{
						CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
						copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(this.SharePointOptions);
						this.SubActions.Add(copyWorkflowAssociationsAction);
						foreach (SPContentType contentType in sourceWeb.ContentTypes)
						{
							SPContentType contentTypeByName = addedSite.ContentTypes.GetContentTypeByName(contentType.Name);
							if (contentTypeByName == null || !(contentType.ContentTypeID != contentTypeByName.ContentTypeID))
							{
								continue;
							}
							copyWorkflowAssociationsAction.RunAsSubAction(new object[] { contentType, addedSite.ContentTypes.GetContentTypeByName(contentType.Name), null }, actionContext, null);
						}
					}
					finally
					{
						sourceWeb.Dispose();
						addedSite.Dispose();
					}
				});
			}
			base.MigrateNintexWorkflows(sourceWeb, addedSite);
			if (base.SharePointOptions.CopyWebOOBWorkflowAssociations || base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations)
			{
				base.ThreadManager.QueueBufferedTask(typeof(CopyWorkflowAssociationsAction).Name, null, (object[] oParams) => {
					try
					{
						CopyWorkflowAssociationsAction copyWorkflowAssociationsAction = new CopyWorkflowAssociationsAction();
						copyWorkflowAssociationsAction.SharePointOptions.SetFromOptions(this.SharePointOptions);
						this.SubActions.Add(copyWorkflowAssociationsAction);
						copyWorkflowAssociationsAction.RunAsSubAction(new object[] { sourceWeb, addedSite }, actionContext, null);
					}
					finally
					{
						sourceWeb.Dispose();
						addedSite.Dispose();
					}
				});
			}
			sourceWeb.Dispose();
			addedSite.Dispose();
		}

		protected string GetSiteXML(SPWeb sourceWeb, SPBaseServer targetServer)
		{
			XmlNode languageCode;
			SPWebCollection subWebs;
			PasteSiteCollectionAction pasteSiteCollectionAction = this as PasteSiteCollectionAction;
			if (pasteSiteCollectionAction == null)
			{
				pasteSiteCollectionAction = new PasteSiteCollectionAction();
				pasteSiteCollectionAction.Options.SetFromOptions(this.Options);
				base.SubActions.Add(pasteSiteCollectionAction);
			}
			SPWeb parent = sourceWeb.Parent as SPWeb;
			SPSiteCollection sites = targetServer.Sites;
			if (parent != null)
			{
				subWebs = parent.SubWebs;
			}
			else
			{
				subWebs = new SPAdHocWebCollection(sourceWeb);
			}
			SPWebCollection sPWebCollection = subWebs;
			sourceWeb = pasteSiteCollectionAction.SiteTransformerDefinition.Transform(sourceWeb, pasteSiteCollectionAction, sPWebCollection, sites, this.Options.Transformers);
			if (sourceWeb == null)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			if (!(sourceWeb is SPSite))
			{
				xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Site"));
				languageCode = xmlDocument.SelectSingleNode("//Site");
				foreach (XmlAttribute attribute in sourceWeb.GetNodeXML().Attributes)
				{
					XmlAttribute value = xmlDocument.CreateAttribute(attribute.Name);
					value.Value = attribute.Value;
					languageCode.Attributes.Append(value);
				}
			}
			else
			{
				xmlDocument.LoadXml(sourceWeb.XML);
				languageCode = xmlDocument.SelectSingleNode("//Site");
			}
			if (base.SharePointOptions.CopySiteAdmins)
			{
				this.AddOrUpdateAttributeValue("SiteCollectionAdministrators", base.SharePointOptions.SiteCollectionAdministrators, languageCode);
			}
			if (base.SharePointOptions.CopySiteQuota)
			{
				this.AddOrUpdateAttributeValue("QuotaID", base.SharePointOptions.QuotaID, languageCode);
				if (base.SharePointOptions.QuotaMaximum > (long)0 || base.SharePointOptions.QuotaWarning > (long)0)
				{
					T sharePointOptions = base.SharePointOptions;
					this.AddOrUpdateAttributeValue("QuotaStorageLimit", sharePointOptions.QuotaMaximum * (long)1048576, languageCode);
					T t = base.SharePointOptions;
					this.AddOrUpdateAttributeValue("QuotaStorageWarning", t.QuotaWarning * (long)1048576, languageCode);
				}
			}
			this.AddOrUpdateAttributeValue("IsHostHeader", base.SharePointOptions.IsHostHeader, languageCode);
			if (!string.IsNullOrEmpty(base.SharePointOptions.HostHeaderURL))
			{
				this.AddOrUpdateAttributeValue("HostHeaderURL", base.SharePointOptions.HostHeaderURL, languageCode);
			}
			if (targetServer is SPTenant)
			{
				this.AddOrUpdateAttributeValue("StorageQuota", base.SharePointOptions.StorageQuota, languageCode);
				this.AddOrUpdateAttributeValue("ResourceQuota", base.SharePointOptions.ResourceQuota, languageCode);
			}
			if (!string.IsNullOrEmpty(base.SharePointOptions.OwnerLogin))
			{
				this.AddOrUpdateAttributeValue("Owner", base.SharePointOptions.OwnerLogin, languageCode);
			}
			if (string.IsNullOrEmpty(base.SharePointOptions.SecondaryOwnerLogin))
			{
				this.AddOrUpdateAttributeValue("SecondaryOwner", "", languageCode);
			}
			else
			{
				this.AddOrUpdateAttributeValue("SecondaryOwner", base.SharePointOptions.SecondaryOwnerLogin, languageCode);
			}
			if (!string.IsNullOrEmpty(base.SharePointOptions.LanguageCode))
			{
				languageCode.Attributes["Language"].Value = base.SharePointOptions.LanguageCode;
			}
			if (string.IsNullOrEmpty(base.SharePointOptions.URL) || this.Options is PasteMySiteOptions)
			{
				string str = languageCode.Attributes["ServerRelativeUrl"].Value.Substring(0, languageCode.Attributes["ServerRelativeUrl"].Value.LastIndexOf("/") + 1);
				languageCode.Attributes["ServerRelativeUrl"].Value = languageCode.Attributes["ServerRelativeUrl"].Value.Replace(str, ((PasteMySiteOptions)this.Options).Path);
			}
			else
			{
				languageCode.Attributes["ServerRelativeUrl"].Value = base.SharePointOptions.URL;
			}
			if (languageCode.Attributes["Owner"] == null)
			{
				return null;
			}
			SPLanguage item = targetServer.Languages[base.SharePointOptions.LanguageCode];
			if (item != null && item.HasMultipleExperienceVersions && item.ExperienceVersions.Contains(base.SharePointOptions.ExperienceVersion))
			{
				this.AddOrUpdateAttributeValue("ExperienceVersion", base.SharePointOptions.ExperienceVersion, languageCode);
			}
			this.AddChildNodesToSiteXml(sourceWeb, languageCode);
			return languageCode.OuterXml;
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(this._siteTransformerDefinition);
			return supportedDefinitions;
		}

		protected void RefreshSiteCollection(SPWeb sourceMySite, SPBaseServer targetServer)
		{
			LogItem logItem = null;
			try
			{
				logItem = new LogItem("Refreshing Site Collection", sourceMySite.DisplayName, sourceMySite.DisplayUrl, (targetServer != null ? targetServer.DisplayUrl : ""), ActionOperationStatus.Running)
				{
					WriteToJobDatabase = false
				};
				base.FireOperationStarted(logItem);
				RefreshAction refreshAction = new RefreshAction();
				Node[] nodeArray = new Node[] { sourceMySite };
				refreshAction.Run(null, new NodeCollection(nodeArray));
			}
			finally
			{
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
		}

		private void Set2013Theme(SPWeb sourceWeb, SPSite addedSite)
		{
			LogItem logItem = null;
			try
			{
				try
				{
					CopyComposedLooksGalleryAction copyComposedLooksGalleryAction = new CopyComposedLooksGalleryAction();
					copyComposedLooksGalleryAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyComposedLooksGalleryAction);
					object[] copyAllThemes = new object[] { sourceWeb, addedSite, null };
					copyAllThemes[2] = base.SharePointOptions.CopyAllThemes;
					copyComposedLooksGalleryAction.RunAsSubAction(copyAllThemes, new ActionContext(sourceWeb, addedSite), null);
					SharePointVersion sharePointVersion = new SharePointVersion(Resources.MinimumSharePointVersionFor2013Theme);
					if (addedSite.Adapter.SharePointVersion < sharePointVersion)
					{
						LogItem logItem1 = new LogItem(Resources.ApplyCurrentTheme, "", "", "", ActionOperationStatus.Skipped)
						{
							Information = string.Format(Resources.FS_2013ThemeVersionMessage, Resources.MinimumSharePointVersionFor2013Theme)
						};
						base.FireOperationStarted(logItem1);
						base.FireOperationFinished(logItem1);
					}
					else
					{
						addedSite.Apply2013Theme();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem = new LogItem(Resources.ApplyTheme, sourceWeb.Name, sourceWeb.DisplayUrl, addedSite.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Warning;
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

		private void SetNintexStorageDatabaseEntry(SPSite addedSite)
		{
			LogItem logItem = null;
			try
			{
				logItem = new LogItem("Setting Nintex site collection database entry.", "", null, addedSite.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				NintexExternalConnection externalNintexConfigurationDatabase = Metalogix.SharePoint.ExternalConnections.Utils.GetExternalNintexConfigurationDatabase(addedSite);
				if (externalNintexConfigurationDatabase != null)
				{
					externalNintexConfigurationDatabase.SetNintexStorageData(addedSite.RootSiteGUID);
				}
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception)
			{
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
			}
		}
	}
}