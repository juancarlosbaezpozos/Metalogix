using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Permissions;
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
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[CmdletEnabled(true, "Copy-MLAllSharePointSiteContent", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.AllSiteContent.ico")]
	[MenuText("2:Paste Site Content {0-Paste} > All Site Content...")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste All Site Content")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SubActionTypes(new Type[] { typeof(PasteNavigationAction), typeof(PasteNavigationAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteSiteContentAction : PasteSiteBaseAction<PasteSiteContentOptions>
	{
		public PasteSiteContentAction()
		{
		}

		public void CopySiteContent(SPWeb sourceWeb, SPWeb targetWeb)
		{
			string keyFor;
			SPWebCollection subWebs;
			SPWebCollection sPAdHocWebCollection;
			bool flag;
			bool flag1;
			if (base.CheckForAbort())
			{
				return;
			}
			base.InitializeWorkflow();
			if (base.SharePointOptions.CorrectingLinks)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
						if (sourceWeb.Adapter.ServerUrl != null)
						{
							string str = sourceWeb.Adapter.ServerUrl.TrimEnd(new char[] { '/' });
							string serverRelativeUrl = sourceWeb.ServerRelativeUrl;
							char[] chrArray = new char[] { '/' };
							string str1 = string.Concat(str, "/", serverRelativeUrl.Trim(chrArray));
							string str2 = targetWeb.Adapter.ServerUrl.TrimEnd(new char[] { '/' });
							string serverRelativeUrl1 = targetWeb.ServerRelativeUrl;
							char[] chrArray1 = new char[] { '/' };
							string str3 = string.Concat(str2, "/", serverRelativeUrl1.Trim(chrArray1));
							base.LinkCorrector.AddMapping(str1, str3);
						}
						base.LinkCorrector.AddMapping(sourceWeb.ServerRelativeUrl, targetWeb.ServerRelativeUrl);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem = new LogItem("Initialize Link Corrector for Site Copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
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
			SPWeb parent = targetWeb.Parent as SPWeb;
			if (parent != null)
			{
				keyFor = base.PermissionsKeyFormatter.GetKeyFor(parent);
			}
			else
			{
				string name = typeof(CopyRoleAssignmentsAction).Name;
				Guid guid = new Guid();
				keyFor = string.Concat(name, guid.ToString());
			}
			string str4 = keyFor;
			LogItem xML = null;
			string xML1 = null;
			try
			{
				try
				{
					PasteSiteAction pasteSiteAction = new PasteSiteAction();
					pasteSiteAction.Options.SetFromOptions(this.Options);
					base.SubActions.Add(pasteSiteAction);
					SPWeb sPWeb = sourceWeb.Parent as SPWeb;
					xML = new LogItem("Updating Site", targetWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(xML);
					if (parent != null)
					{
						subWebs = parent.SubWebs;
					}
					else
					{
						subWebs = new SPAdHocWebCollection(targetWeb);
					}
					SPWebCollection sPWebCollection = subWebs;
					if (sPWeb != null)
					{
						sPAdHocWebCollection = sPWeb.SubWebs;
					}
					else
					{
						sPAdHocWebCollection = new SPAdHocWebCollection(sourceWeb);
					}
					SPWebCollection sPWebCollection1 = sPAdHocWebCollection;
					sourceWeb = pasteSiteAction.WebTransformerDefinition.Transform(sourceWeb, pasteSiteAction, sPWebCollection1, sPWebCollection, this.Options.Transformers);
					if (sourceWeb != null)
					{
						xML1 = sourceWeb.XML;
						string str5 = base.LinkCorrector.UpdateLinksInWeb(sourceWeb, xML1);
						UpdateWebOptions updateWebOption = new UpdateWebOptions();
						int updateSiteOptionsBitField = base.SharePointOptions.UpdateSiteOptionsBitField;
						if (base.SharePointOptions.MigrationMode == MigrationMode.Custom && !base.SharePointOptions.OverwriteSites && base.SharePointOptions.UpdateSites)
						{
							updateWebOption.CopyCoreMetaData = (updateSiteOptionsBitField & 1) > 0;
						}
						updateWebOption.MergeFeatures = base.SharePointOptions.MergeSiteFeatures;
						updateWebOption.CopyFeatures = base.SharePointOptions.CopySiteFeatures;
						updateWebOption.ApplyTheme = base.SharePointOptions.ApplyThemeToWeb;
						updateWebOption.ApplyMasterPage = false;
						updateWebOption.CopyNavigation = base.SharePointOptions.CopyNavigation;
						UpdateWebOptions updateWebOption1 = updateWebOption;
						if (!base.SharePointOptions.CopyAccessRequestSettings)
						{
							flag = false;
						}
						else
						{
							flag = (targetWeb.HasUniquePermissions || sourceWeb.HasUniquePermissions ? true : base.SharePointOptions.CopyRootPermissions);
						}
						updateWebOption1.CopyAccessRequestSettings = flag;
						UpdateWebOptions updateWebOption2 = updateWebOption;
						if (!base.SharePointOptions.CopyAssociatedGroups)
						{
							flag1 = false;
						}
						else
						{
							flag1 = (targetWeb.HasUniquePermissions || sourceWeb.HasUniquePermissions ? true : base.SharePointOptions.CopyRootPermissions);
						}
						updateWebOption2.CopyAssociatedGroupSettings = flag1;
						if (updateWebOption.CopyAssociatedGroupSettings)
						{
							str5 = base.MapReferencedGroups(str5, sourceWeb, targetWeb);
						}
						targetWeb.Update(str5, updateWebOption);
						base.AddGuidMappings(sourceWeb.ID, targetWeb.ID);
						if (!base.CheckForAbort())
						{
							if (base.SharePointOptions.CopyPermissionLevels && (sourceWeb.HasUniqueRoles || base.SharePointOptions.CopyRootPermissions))
							{
								PasteRolesAction pasteRolesAction = new PasteRolesAction();
								pasteRolesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
								base.SubActions.Add(pasteRolesAction);
								object[] roles = new object[] { sourceWeb.Roles as SPRoleCollection, targetWeb.Roles as SPRoleCollection };
								pasteRolesAction.RunAsSubAction(roles, new ActionContext(sourceWeb, targetWeb), null);
							}
							if (!base.CheckForAbort())
							{
								if (base.SharePointOptions.CopySitePermissions || base.SharePointOptions.CopyListPermissions || base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
								{
									if (!targetWeb.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload)
									{
										Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
										object[] objArray = new object[] { sourceWeb, targetWeb, base.SharePointOptions.OverwriteSites, true };
										threadManager.QueueBufferedTask(str4, objArray, new ThreadedOperationDelegate(this.CopyWebPermissionsTaskDelegate));
									}
									else
									{
										object[] objArray1 = new object[] { sourceWeb, targetWeb, base.SharePointOptions.OverwriteSites, true };
										base.CopyWebPermissionsTaskDelegate(objArray1);
									}
								}
								if (base.SharePointOptions.PreserveMasterPage)
								{
									base.CopyMasterPageSetting(sourceWeb, targetWeb, str5);
								}
								if (!base.SharePointOptions.CheckResults)
								{
									xML.Status = ActionOperationStatus.Completed;
								}
								else
								{
									base.CompareNodes(sourceWeb, targetWeb, xML);
								}
								if (base.SharePointOptions.Verbose)
								{
									xML.SourceContent = sourceWeb.XML;
									xML.TargetContent = targetWeb.XML;
								}
							}
							else
							{
								return;
							}
						}
						else
						{
							return;
						}
					}
					else
					{
						xML.Status = ActionOperationStatus.Skipped;
						xML.Information = "Site skipped by transformation";
						return;
					}
				}
				catch (Exception exception2)
				{
					xML.Exception = exception2;
					xML.SourceContent = xML1;
				}
				goto Label0;
			}
			finally
			{
				base.FireOperationFinished(xML);
			}
			return;
		Label0:
			if (!base.CheckForAbort())
			{
				base.QueueAccessRequestListCopies(sourceWeb, targetWeb, base.SharePointOptions);
				this.CopyWebContents(sourceWeb, targetWeb, true, true);
				base.StartCommonWorkflowBufferedTasks();
				base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
				base.ThreadManager.SetBufferedTasks(str4, false, false);
				sourceWeb.Dispose();
				targetWeb.Dispose();
				return;
			}
			else
			{
				return;
			}
		}

		public void CopyWebContents(SPWeb sourceWeb, SPWeb targetWeb, bool bIsCopyRoot, bool bWebNewlyCreated)
		{
			bool flag;
			bool flag1;
			string keyFor;
			if (base.SharePointOptions.RunNavigationStructureCopy && targetWeb != null)
			{
				base.SourceTargetNavCopyMap.Add(sourceWeb, targetWeb);
			}
			ActionContext actionContext = new ActionContext(sourceWeb, targetWeb);
			SPWebCollection subWebs = sourceWeb.SubWebs;
			SPListCollection lists = sourceWeb.Lists;
			bool updateSites = base.SharePointOptions.UpdateSites;
			if (bWebNewlyCreated && base.SharePointOptions.CopySiteColumns)
			{
				flag = true;
			}
			else if (bWebNewlyCreated)
			{
				flag = false;
			}
			else
			{
				flag = (!updateSites ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 2) > 0);
			}
			bool flag2 = flag;
			if (bWebNewlyCreated && base.SharePointOptions.CopyContentTypes)
			{
				flag1 = true;
			}
			else if (bWebNewlyCreated)
			{
				flag1 = false;
			}
			else
			{
				flag1 = (!updateSites || !base.SharePointOptions.CopyContentTypes ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 16) > 0);
			}
			bool flag3 = flag1;
			if (base.SharePointOptions.CopyPortalListings)
			{
				PastePortalListingsAction pastePortalListingsAction = new PastePortalListingsAction();
				pastePortalListingsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pastePortalListingsAction);
				object[] objArray = new object[] { sourceWeb, targetWeb };
				pastePortalListingsAction.RunAsSubAction(objArray, new ActionContext(sourceWeb, targetWeb), null);
			}
			string str = PasteSiteContentAction.DisableSiteAndListWorkflows(targetWeb);
			if (!base.SharePointOptions.CopyLists || lists.Count <= 0)
			{
				if (flag2)
				{
					SPFieldCollection sPFieldCollections = (bIsCopyRoot ? sourceWeb.GetAvailableColumns(false) : sourceWeb.GetSiteColumns(false));
					SPFieldCollection sPFieldCollections1 = (bIsCopyRoot ? targetWeb.GetAvailableColumns(false) : targetWeb.GetSiteColumns(false));
					if (base.CheckForAbort())
					{
						return;
					}
					CopySiteColumnsAction copySiteColumnsAction = new CopySiteColumnsAction();
					copySiteColumnsAction.Options.SetFromOptions(this.Options);
					this.ConnectSubaction(copySiteColumnsAction);
					object[] siteFieldsFilterExpression = new object[] { sPFieldCollections, (this.Options as PasteSiteOptions).SiteFieldsFilterExpression, sPFieldCollections1, (this.Options as PasteSiteOptions).TermstoreNameMappingTable, false };
					copySiteColumnsAction.RunAsSubAction(siteFieldsFilterExpression, new ActionContext(null, targetWeb), null);
				}
				if (base.CheckForAbort())
				{
					return;
				}
				if (flag3)
				{
					CopyContentTypesAction copyContentTypesAction = new CopyContentTypesAction();
					copyContentTypesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyContentTypesAction);
					object[] contentTypes = new object[] { sourceWeb.ContentTypes, targetWeb.ContentTypes };
					copyContentTypesAction.RunAsSubAction(contentTypes, new ActionContext(sourceWeb, targetWeb), null);
				}
				if (base.CheckForAbort())
				{
					return;
				}
			}
			else
			{
				PasteSiteLists pasteSiteList = new PasteSiteLists();
				pasteSiteList.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteSiteList);
				object[] objArray1 = new object[] { lists, targetWeb, bIsCopyRoot, flag2, flag3 };
				pasteSiteList.RunAsSubAction(objArray1, new ActionContext(null, targetWeb), null);
			}
			if ((!sourceWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater || !targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater ? false : base.SharePointOptions.ApplyThemeToWeb))
			{
				this.SetTheme2013(sourceWeb, targetWeb);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			base.MigrateCustomContent(sourceWeb, targetWeb);
			if (base.CheckForAbort())
			{
				return;
			}
			if (bWebNewlyCreated || updateSites && (base.SharePointOptions.UpdateSiteOptionsBitField & 1) > 0)
			{
				base.SetWelcomePage(sourceWeb, targetWeb);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			sourceWeb.Dispose();
			if (base.SharePointOptions.RecursivelyCopySubsites)
			{
				PasteAllSubSitesAction pasteAllSubSitesAction = new PasteAllSubSitesAction();
				pasteAllSubSitesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(pasteAllSubSitesAction);
				object[] objArray2 = new object[] { sourceWeb, targetWeb, bIsCopyRoot };
				pasteAllSubSitesAction.RunAsSubAction(objArray2, actionContext, null);
			}
			if (bIsCopyRoot)
			{
				SPWeb parent = targetWeb.Parent as SPWeb;
				if (parent != null)
				{
					keyFor = base.PermissionsKeyFormatter.GetKeyFor(parent);
				}
				else
				{
					string name = typeof(CopyRoleAssignmentsAction).Name;
					Guid guid = new Guid();
					keyFor = string.Concat(name, guid.ToString());
				}
				base.ThreadManager.SetBufferedTasks(keyFor, false, false);
			}
			if (base.CheckForAbort())
			{
				return;
			}
			if (bWebNewlyCreated && base.SharePointOptions.CopySiteWebParts || !bWebNewlyCreated && base.SharePointOptions.CopySiteWebParts && updateSites && (base.SharePointOptions.UpdateSiteOptionsBitField & 64) > 0)
			{
				Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
				object[] objArray3 = new object[] { sourceWeb, targetWeb };
				threadManager.QueueTask(objArray3, new ThreadedOperationDelegate(this.CopyWelcomePageWebPartsTaskDelegate));
			}
			PasteSiteContentAction.EnableSiteAndListWorkflows(targetWeb, str);
			base.ThreadManager.SetBufferedTasks(base.GetWebPartCopyBufferKey(targetWeb), false, false);
			if (bWebNewlyCreated || updateSites && (base.SharePointOptions.UpdateSiteOptionsBitField & 4096) > 0)
			{
				base.MigrateNintexWorkflows(sourceWeb, targetWeb);
				if ((base.SharePointOptions.CopyContentTypeOOBWorkflowAssociations || base.SharePointOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations) && base.SharePointOptions.CopyContentTypes)
				{
					Metalogix.Threading.ThreadManager threadManager1 = base.ThreadManager;
					string name1 = typeof(CopyWorkflowAssociationsAction).Name;
					object[] objArray4 = new object[] { sourceWeb, targetWeb };
					threadManager1.QueueBufferedTask(name1, objArray4, new ThreadedOperationDelegate(this.CopyContentTypeWorkflowAssociationsTaskDelegate));
				}
				if (base.SharePointOptions.CopyWebOOBWorkflowAssociations || base.SharePointOptions.CopyWebSharePointDesignerNintexWorkflowAssociations)
				{
					Metalogix.Threading.ThreadManager threadManager2 = base.ThreadManager;
					string str1 = typeof(CopyWorkflowAssociationsAction).Name;
					object[] objArray5 = new object[] { sourceWeb, targetWeb };
					threadManager2.QueueBufferedTask(str1, objArray5, new ThreadedOperationDelegate(this.CopyWebWorkflowAssociationsTaskDelegate));
				}
			}
		}

		private static string DisableSiteAndListWorkflows(SPWeb targetWeb)
		{
			if (!targetWeb.Adapter.IsClientOM)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
			{
				xmlWriter.WriteStartElement(XmlElementNames.DisableSiteAndListWorkflows.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			string str = targetWeb.Adapter.Writer.UpdateWeb(stringBuilder.ToString(), new UpdateWebOptions());
			return str;
		}

		private static string EnableSiteAndListWorkflows(SPWeb targetWeb, string disableWorkflowResultsXml)
		{
			if (!targetWeb.Adapter.IsClientOM || disableWorkflowResultsXml == null)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(disableWorkflowResultsXml);
			XmlNode xmlNodes = xmlDocument.FirstChild.SelectSingleNode(string.Format("//Information[contains(@Message,'{0}')]", XmlElementNames.DisableSiteAndListWorkflows));
			if (xmlNodes == null)
			{
				return null;
			}
			string value = xmlNodes.Attributes["Detail"].Value;
			string[] strArrays = value.Split(new char[] { ' ' });
			StringBuilder stringBuilder = new StringBuilder(1024);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings))
			{
				xmlWriter.WriteStartElement(XmlElementNames.EnableSiteAndListWorkflows.ToString());
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					string str = strArrays1[i];
					xmlWriter.WriteStartElement(XmlElementNames.Workflow.ToString());
					xmlWriter.WriteAttributeString(XmlAttributeNames.Guid.ToString(), str);
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			string str1 = targetWeb.Adapter.Writer.UpdateWeb(stringBuilder.ToString(), new UpdateWebOptions());
			return str1;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = source[0] as SPWeb)
			{
				if (item == null)
				{
					throw new ArgumentException("Source is not valid");
				}
				foreach (SPWeb sPWeb in target)
				{
					try
					{
						Node[] nodeArray = new Node[] { sPWeb };
						this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
						this.CopySiteContent(item, sPWeb);
						if (base.SharePointOptions.RunNavigationStructureCopy)
						{
							PasteNavigationAction pasteNavigationAction = new PasteNavigationAction();
							pasteNavigationAction.SharePointOptions.CopyCurrentNavigation = base.SharePointOptions.CopyCurrentNavigation;
							pasteNavigationAction.SharePointOptions.CopyGlobalNavigation = base.SharePointOptions.CopyGlobalNavigation;
							pasteNavigationAction.SharePointOptions.Recursive = false;
							pasteNavigationAction.SharePointOptions.TaskCollection = base.SharePointOptions.TaskCollection;
							base.SubActions.Add(pasteNavigationAction);
							object[] sourceTargetNavCopyMap = new object[] { base.SourceTargetNavCopyMap, false };
							pasteNavigationAction.RunAsSubAction(sourceTargetNavCopyMap, new ActionContext(null, sPWeb), null);
						}
					}
					finally
					{
						sPWeb.Dispose();
					}
				}
			}
			base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}

		protected void SetTheme2013(SPWeb sourceWeb, SPWeb targetWeb)
		{
			LogItem logItem = null;
			try
			{
				try
				{
					if (!base.SharePointOptions.CopyAllThemes)
					{
						string item = sourceWeb.ComposedLooksGallery.CurrentItem["MasterPageUrl"];
						char[] chrArray = new char[] { ',' };
						string str = item.Split(chrArray)[0];
						if (!string.IsNullOrEmpty(str))
						{
							string serverRelative = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, str).ServerRelative;
							SPListItem itemByServerRelativeUrl = sourceWeb.MasterPageGallery.Items.GetItemByServerRelativeUrl(serverRelative);
							if (itemByServerRelativeUrl == null)
							{
								LogItem logItem1 = new LogItem(Resources.CopyMasterpageFailed, str, "", "", ActionOperationStatus.Warning);
								base.FireOperationStarted(logItem1);
								base.FireOperationFinished(logItem1);
							}
							else
							{
								SPListItem matchingItem = targetWeb.MasterPageGallery.Items.GetMatchingItem(itemByServerRelativeUrl, itemByServerRelativeUrl.FileLeafRef, itemByServerRelativeUrl.ParentRelativePath);
								if (matchingItem != null)
								{
									try
									{
										targetWeb.MasterPageGallery.Items.DeleteItem(matchingItem);
									}
									catch (Exception exception1)
									{
										Exception exception = exception1;
										LogItem logItem2 = new LogItem(Resources.DeleteMasterpageFailed, matchingItem.Name, "", targetWeb.MasterPageGallery.DisplayUrl, ActionOperationStatus.Skipped)
										{
											Information = exception.Message
										};
										base.FireOperationStarted(logItem2);
										base.FireOperationFinished(logItem2);
									}
								}
								SPMasterPageGallery masterPageGallery = sourceWeb.MasterPageGallery;
								SPMasterPageGallery sPMasterPageGallery = sourceWeb.MasterPageGallery;
								SPListItem[] sPListItemArray = new SPListItem[] { itemByServerRelativeUrl };
								SPListItemCollection sPListItemCollection = new SPListItemCollection(masterPageGallery, sPMasterPageGallery, sPListItemArray);
								CopyMasterPagesAction copyMasterPagesAction = new CopyMasterPagesAction();
								copyMasterPagesAction.ActionOptions.SetFromOptions(this.ActionOptions);
								copyMasterPagesAction.ActionOptions.UpdateMasterPagesForUseBySpecificUIVersion = false;
								base.SubActions.Add(copyMasterPagesAction);
								object[] items = new object[] { sPListItemCollection, targetWeb.MasterPageGallery.Items };
								copyMasterPagesAction.RunAsSubAction(items, new ActionContext(sourceWeb, targetWeb), null);
							}
						}
					}
					else
					{
						CopyMasterPageGalleryAction copyMasterPageGalleryAction = new CopyMasterPageGalleryAction();
						copyMasterPageGalleryAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
						base.SubActions.Add(copyMasterPageGalleryAction);
						object[] objArray = new object[] { sourceWeb.MasterPageGallery, targetWeb.MasterPageGallery };
						copyMasterPageGalleryAction.RunAsSubAction(objArray, new ActionContext(sourceWeb, targetWeb), null);
					}
					CopyComposedLooksGalleryAction copyComposedLooksGalleryAction = new CopyComposedLooksGalleryAction();
					copyComposedLooksGalleryAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
					base.SubActions.Add(copyComposedLooksGalleryAction);
					object[] objArray1 = new object[] { sourceWeb, targetWeb, base.SharePointOptions.CopyAllThemes };
					copyComposedLooksGalleryAction.RunAsSubAction(objArray1, new ActionContext(sourceWeb, targetWeb), null);
					SharePointVersion sharePointVersion = new SharePointVersion(Resources.MinimumSharePointVersionFor2013Theme);
					if (targetWeb.Adapter.SharePointVersion < sharePointVersion)
					{
						LogItem logItem3 = new LogItem(Resources.ApplyCurrentTheme, "", "", "", ActionOperationStatus.Skipped)
						{
							Information = string.Format(Resources.FS_2013ThemeVersionMessage, Resources.MinimumSharePointVersionFor2013Theme)
						};
						base.FireOperationStarted(logItem3);
						base.FireOperationFinished(logItem3);
					}
					else
					{
						targetWeb.Apply2013Theme();
					}
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					logItem = new LogItem(Resources.ApplyTheme, sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					logItem.Exception = exception2;
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
	}
}