using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Jobs;
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
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Analyzable(true)]
	[BasicModeViewAllowed(true)]
	[CmdletEnabled(true, "Copy-MLSharePointSite", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[Incrementable(true, "Paste Site Incrementally")]
	[MenuText("1:Paste Site as Subsite... {0-Paste}")]
	[MenuTextPlural("1:Paste Sites as Subsites... {0-Paste}", PluralCondition.MultipleSources)]
	[Name("Paste Site as Subsite")]
	[RunAsync(true)]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteSiteAction : PasteSiteBaseAction<PasteSiteOptions>
	{
		private bool dontShowLogWhenListFilteringIsOn;

		public PasteSiteAction()
		{
		}

		public override Dictionary<string, string> AnalyzeAction(Job parentJob, DateTime pivotDate)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			long num = (long)0;
			long num1 = (long)0;
			IXMLAbleList sourceList = parentJob.SourceList;
			if (typeof(SPNode).IsAssignableFrom(sourceList.CollectionType))
			{
				bool flag = true;
				foreach (SPNode sPNode in sourceList)
				{
					long num2 = (long)0;
					long num3 = (long)0;
					flag = (!flag ? false : sPNode.AnalyzeChurn(pivotDate, base.SharePointOptions.RecursivelyCopySubsites, out num3, out num2));
					if (!flag)
					{
						continue;
					}
					num1 += num3;
					num += num2;
				}
				if (flag)
				{
					strs.Add("ItemsChanged", num.ToString());
					strs.Add("BytesChanged", Format.FormatSize(new long?(num1)));
				}
			}
			return strs;
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool displayUrl = true;
			if (sourceSelections.Count > 0 && targetSelections.Count > 0 && sourceSelections[0] is SPWeb && targetSelections[0] is SPWeb)
			{
				SPWeb item = sourceSelections[0] as SPWeb;
				SPWeb sPWeb = targetSelections[0] as SPWeb;
				if (item.Parent != null)
				{
					displayUrl = item.Parent.DisplayUrl != sPWeb.DisplayUrl;
				}
			}
			if (SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return displayUrl;
			}
			return false;
		}

		protected override void CleanUp()
		{
			base.CleanUp();
			if (base.SourceTargetNavCopyMap != null)
			{
				base.SourceTargetNavCopyMap.Clear();
			}
		}

		public TaskDefinition CopySite(SPWeb sourceWeb, SPWeb targetWeb)
		{
			return this.CopySite(sourceWeb, targetWeb, false);
		}

		public TaskDefinition CopySite(SPWeb sourceWeb, SPWeb targetWeb, bool bIsCopyRoot)
		{
			TaskDefinition taskDefinition;
			bool flag;
			bool flag1;
			bool flag2;
			TaskDefinition taskDefinition1 = null;
			if (base.CheckForAbort())
			{
				sourceWeb.Dispose();
				return taskDefinition1;
			}
			if (base.SharePointOptions.FilterLists && !this.dontShowLogWhenListFilteringIsOn)
			{
				LogItem logItem = new LogItem("Workflow warning", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
				{
					Information = "Workflow will may not migrate properly, if filter will filter out some Workflow lists"
				};
				base.FireOperationStarted(logItem);
				this.dontShowLogWhenListFilteringIsOn = true;
			}
			if (base.SharePointOptions.FilterSites && !base.SharePointOptions.SiteFilterExpression.Evaluate(sourceWeb, new CompareDatesInUtc()) && !bIsCopyRoot)
			{
				LogItem logItem1 = null;
				logItem1 = new LogItem("Skipping Site", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Skipped)
				{
					Information = "Filtered out by site filters."
				};
				base.FireOperationStarted(logItem1);
				sourceWeb.Dispose();
				return taskDefinition1;
			}
			string transformationValue = null;
			TransformationTask transformationTask = null;
			if (base.SharePointOptions.RenameSpecificNodes)
			{
				transformationValue = base.SharePointOptions.TaskCollection.GetTransformationValue(sourceWeb, "Name", new CompareDatesInUtc(), out transformationTask);
			}
			if (!string.IsNullOrEmpty(transformationValue) && !this.IsValidSiteURL(sourceWeb, targetWeb, transformationValue))
			{
				taskDefinition1 = null;
				return taskDefinition1;
			}
			this.PopulateLinkCorrector(sourceWeb, targetWeb);
			LogItem xML = null;
			bool flag3 = false;
			SPWeb sPWeb = null;
			bool flag4 = false;
			string str = null;
			try
			{
				try
				{
					xML = new LogItem("Copying Site", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					SPWeb item = targetWeb.SubWebs[transformationValue ?? sourceWeb.Name] as SPWeb;
					bool flag5 = false;
					if (item != null)
					{
						if (base.SharePointOptions.OverwriteSites)
						{
							if (!targetWeb.SubWebs.DeleteWeb(transformationValue ?? sourceWeb.Name))
							{
								throw new Exception(string.Format("Could not overwrite web '{0}' on the target because the web could not be deleted.", item.DisplayUrl));
							}
							item = null;
						}
						else if (!base.SharePointOptions.UpdateSites || base.SharePointOptions.UpdateSiteOptionsBitField <= 0)
						{
							flag5 = true;
							xML.Operation = "Skipping Site";
							xML.Information = "This site was skipped because sites are not being overwritten and there was nothing to update for the site's metadata. Content underneath the site may still be copied depending on your option settings.";
							xML.Status = ActionOperationStatus.Skipped;
						}
					}
					base.FireOperationStarted(xML);
					flag3 = true;
					string str1 = null;
					if (!flag5)
					{
						SPWeb parent = sourceWeb.Parent as SPWeb;
						SPWebCollection sPAdHocWebCollection = null;
						if (parent == null)
						{
							sPAdHocWebCollection = new SPAdHocWebCollection(sourceWeb);
						}
						else
						{
							sPAdHocWebCollection = parent.SubWebs;
						}
						if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && base.SharePointOptions.CopyReferencedManagedMetadata)
						{
							base.EnsureManagedMetadataExistence(sourceWeb.AvailableColumns, sourceWeb, targetWeb, base.SharePointOptions.ResolveManagedMetadataByName);
						}
						SPWeb sPWeb1 = this.webTransformerDefinition.Transform(sourceWeb, this, sPAdHocWebCollection, targetWeb.SubWebs, this.Options.Transformers);
						if (sPWeb1 != null)
						{
							str = sPWeb1.XML;
							str1 = str;
							if (base.SharePointOptions.CopyAssociatedGroups && (item == null || (base.SharePointOptions.UpdateSiteOptionsBitField & 8192) > 0))
							{
								str1 = base.MapReferencedGroups(str1, sourceWeb, targetWeb);
							}
							if (base.SharePointOptions.ChangeWebTemplate || base.SharePointOptions.MapChildWebTemplates)
							{
								str1 = base.MapWebTemplate(str, sourceWeb.Template.Name, targetWeb.Templates, bIsCopyRoot);
							}
							if (transformationTask != null)
							{
								str1 = transformationTask.PerformTransformation(str1);
							}
							if (sourceWeb.Parent == null || sourceWeb.Parent is SPServer)
							{
								str1 = Utils.CorrectRootSiteCollectionTitle(str1);
							}
							if (base.SharePointOptions.CorrectingLinks)
							{
								str1 = base.LinkCorrector.UpdateLinksInWeb(sourceWeb, str1);
							}
						}
						else
						{
							xML.Operation = "Skipping Site";
							xML.Status = ActionOperationStatus.Skipped;
							xML.Information = "Site skipped by transformation";
							taskDefinition = taskDefinition1;
							return taskDefinition;
						}
					}
					if (item != null)
					{
						sPWeb = item;
						if (!flag5)
						{
							UpdateWebOptions updateWebOption = new UpdateWebOptions();
							int updateSiteOptionsBitField = base.SharePointOptions.UpdateSiteOptionsBitField;
							updateWebOption.CopyCoreMetaData = (updateSiteOptionsBitField & 1) > 0;
							bool flag6 = (!base.SharePointOptions.CopySiteFeatures ? false : (updateSiteOptionsBitField & 2048) > 0);
							updateWebOption.CopyFeatures = flag6;
							updateWebOption.MergeFeatures = (!flag6 ? false : base.SharePointOptions.MergeSiteFeatures);
							updateWebOption.CopyNavigation = (!base.SharePointOptions.CopyNavigation ? false : (updateSiteOptionsBitField & 32) > 0);
							updateWebOption.ApplyMasterPage = false;
							updateWebOption.ApplyTheme = (updateSiteOptionsBitField & 512) > 0;
							updateWebOption.CopyAccessRequestSettings = (!base.SharePointOptions.CopyAccessRequestSettings || !sourceWeb.HasUniquePermissions && (!bIsCopyRoot || !base.SharePointOptions.CopyRootPermissions) ? false : (updateSiteOptionsBitField & 1024) > 0);
							updateWebOption.CopyAssociatedGroupSettings = (!base.SharePointOptions.CopyAssociatedGroups || !sourceWeb.HasUniquePermissions && (!bIsCopyRoot || !base.SharePointOptions.CopyRootPermissions) ? false : (updateSiteOptionsBitField & 8192) > 0);
							sPWeb.Update(str1, updateWebOption);
						}
					}
					else
					{
						AddWebOptions addWebOption = new AddWebOptions()
						{
							Overwrite = base.SharePointOptions.OverwriteSites,
							MergeFeatures = base.SharePointOptions.MergeSiteFeatures,
							CopyFeatures = base.SharePointOptions.CopySiteFeatures,
							ApplyTheme = base.SharePointOptions.ApplyThemeToWeb,
							ApplyMasterPage = false,
							CopyNavigation = base.SharePointOptions.CopyNavigation
						};
						AddWebOptions addWebOption1 = addWebOption;
						if (!base.SharePointOptions.CopyAccessRequestSettings)
						{
							flag1 = false;
						}
						else if (sourceWeb.HasUniquePermissions)
						{
							flag1 = true;
						}
						else
						{
							flag1 = (!bIsCopyRoot ? false : base.SharePointOptions.CopyRootPermissions);
						}
						addWebOption1.CopyAccessRequestSettings = flag1;
						AddWebOptions addWebOption2 = addWebOption;
						if (!base.SharePointOptions.CopyAssociatedGroups)
						{
							flag2 = false;
						}
						else if (sourceWeb.HasUniquePermissions)
						{
							flag2 = true;
						}
						else
						{
							flag2 = (!bIsCopyRoot ? false : base.SharePointOptions.CopyRootPermissions);
						}
						addWebOption2.CopyAssociatedGroupSettings = flag2;
						if (base.SharePointOptions.PreserveUIVersion && sourceWeb.UIVersion == "3" && targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
						{
							addWebOption.PreserveUIVersion = base.SharePointOptions.PreserveUIVersion;
						}
						if (sourceWeb.MeetingInstances != null)
						{
							XmlNode xmlNode = XmlUtility.StringToXmlNode(str1);
							foreach (XmlNode childNode in xmlNode.SelectSingleNode(".//MeetingInstances").ChildNodes)
							{
								string str2 = (childNode.Attributes["Organizer"] == null ? "" : childNode.Attributes["Organizer"].Value);
								bool flag7 = false;
								if (str2.Equals(string.Empty))
								{
									continue;
								}
								SPUser sPUser = sourceWeb.SiteUsers[str2] as SPUser;
								if (sPUser == null)
								{
									XmlElement xmlElement = (new XmlDocument()).CreateElement("SPUser");
									xmlElement.SetAttribute("LoginName", str2);
									xmlElement.SetAttribute("Name", "");
									xmlElement.SetAttribute("Email", "");
									xmlElement.SetAttribute("Notes", "");
									sPUser = new SPUser(XmlUtility.StringToXmlNode(xmlElement.OuterXml));
								}
								this.CopyUsers(new SPUserCollection()
								{
									sPUser
								}, targetWeb);
								if (base.PrincipalMappings.ContainsKey(sPUser.LoginName))
								{
									childNode.Attributes["Organizer"].Value = base.PrincipalMappings[sPUser.LoginName];
									flag7 = true;
								}
								if (!flag7)
								{
									childNode.Attributes["Organizer"].Value = "";
								}
								str1 = xmlNode.OuterXml.ToString();
							}
						}
						sPWeb = targetWeb.SubWebs.AddWeb(str1, addWebOption, xML);
						flag4 = true;
					}
					base.AddGuidMappings(sourceWeb.ID, sPWeb.ID);
					if (!flag5)
					{
						if (!base.SharePointOptions.CopyPermissionLevels)
						{
							flag = false;
						}
						else if (flag4)
						{
							flag = true;
						}
						else
						{
							flag = (flag4 || !base.SharePointOptions.UpdateSites ? false : (base.SharePointOptions.UpdateSiteOptionsBitField & 8) > 0);
						}
						if (flag && (sourceWeb.HasUniqueRoles || bIsCopyRoot && base.SharePointOptions.CopyRootPermissions))
						{
							PasteRolesAction pasteRolesAction = new PasteRolesAction();
							pasteRolesAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
							base.SubActions.Add(pasteRolesAction);
							object[] roles = new object[] { sourceWeb.Roles as SPRoleCollection, sPWeb.Roles as SPRoleCollection };
							pasteRolesAction.RunAsSubAction(roles, new ActionContext(sourceWeb, sPWeb), null);
						}
						if (base.SharePointOptions.CopySitePermissions || base.SharePointOptions.CopyListPermissions || base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
						{
							if (!sPWeb.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload)
							{
								Metalogix.Threading.ThreadManager threadManager = base.ThreadManager;
								string keyFor = base.PermissionsKeyFormatter.GetKeyFor(targetWeb);
								object[] objArray = new object[] { sourceWeb, sPWeb, flag4, bIsCopyRoot };
								threadManager.QueueBufferedTask(keyFor, objArray, new ThreadedOperationDelegate(this.CopyWebPermissionsTaskDelegate));
							}
							else
							{
								object[] objArray1 = new object[] { sourceWeb, sPWeb, flag4, bIsCopyRoot };
								base.CopyWebPermissionsTaskDelegate(objArray1);
							}
						}
						if (flag4 && base.SharePointOptions.PreserveMasterPage || !flag4 && base.SharePointOptions.UpdateSites && (base.SharePointOptions.UpdateSiteOptionsBitField & 128) > 0)
						{
							base.CopyMasterPageSetting(sourceWeb, sPWeb, str1);
						}
						if (base.SharePointOptions.CheckResults)
						{
							base.CompareNodes(sourceWeb, sPWeb, xML);
						}
						else if (xML.Status != ActionOperationStatus.Warning)
						{
							xML.Status = ActionOperationStatus.Completed;
						}
						if (flag4)
						{
							LogItem licenseDataUsed = xML;
							licenseDataUsed.LicenseDataUsed = licenseDataUsed.LicenseDataUsed + SPObjectSizes.GetObjectSize(sPWeb);
							if (!base.SharePointOptions.CopyLists || !base.SharePointOptions.OverwriteLists)
							{
								LogItem licenseDataUsed1 = xML;
								licenseDataUsed1.LicenseDataUsed = licenseDataUsed1.LicenseDataUsed + (long)sPWeb.Lists.Count * SPObjectSizes.GetObjectTypeSize(typeof(SPList));
							}
						}
						xML.AddCompletionDetail(Metalogix.SharePoint.Actions.Properties.Resources.Migration_Detail_SitesCopied, (long)1);
					}
					if (base.SharePointOptions.Verbose)
					{
						xML.SourceContent = sourceWeb.XML;
						xML.TargetContent = sPWeb.XML;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (!flag3)
					{
						base.FireOperationStarted(xML);
					}
					StringBuilder stringBuilder = new StringBuilder(1024);
					ExceptionUtils.GetExceptionMessage(exception, stringBuilder);
					xML.Exception = exception;
					xML.Details = stringBuilder.ToString();
					xML.SourceContent = str;
					sourceWeb.Dispose();
					taskDefinition = taskDefinition1;
					return taskDefinition;
				}
				goto Label0;
			}
			finally
			{
				if (xML.Status != ActionOperationStatus.Skipped)
				{
					base.FireOperationFinished(xML);
				}
			}
			return taskDefinition;
		Label0:
			if (base.CheckForAbort())
			{
				return taskDefinition1;
			}
			if (!bIsCopyRoot)
			{
				sourceWeb.Collapse();
				targetWeb.Collapse();
			}
			Metalogix.Threading.ThreadManager threadManager1 = base.ThreadManager;
			object[] objArray2 = new object[] { sourceWeb, sPWeb, bIsCopyRoot, flag4 };
			taskDefinition1 = threadManager1.QueueTask(objArray2, new ThreadedOperationDelegate(this.CopyWebContentsAsync));
			Metalogix.Threading.ThreadManager threadManager2 = base.ThreadManager;
			string str3 = string.Concat("Dispose", sPWeb.ID);
			object[] objArray3 = new object[] { sourceWeb, sPWeb };
			threadManager2.QueueBufferedTask(str3, objArray3, new ThreadedOperationDelegate(this.DisposeWebTaskDelegate));
			base.QueueAccessRequestListCopies(sourceWeb, sPWeb, base.SharePointOptions);
			return taskDefinition1;
		}

		private void CopyUsers(SPUserCollection users, SPWeb targetWeb)
		{
			CopyUsersAction copyUsersAction = new CopyUsersAction();
			copyUsersAction.Options.SetFromOptions(this.Options);
			base.SubActions.Add(copyUsersAction);
			object[] objArray = new object[] { users, targetWeb };
			copyUsersAction.RunAsSubAction(objArray, new ActionContext(null, targetWeb), null);
		}

		private void CopyWebContentsAsync(object[] oParams)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			SPWeb sPWeb = (SPWeb)oParams[0];
			SPWeb sPWeb1 = (SPWeb)oParams[1];
			PasteSiteContentAction pasteSiteContentAction = new PasteSiteContentAction();
			pasteSiteContentAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteSiteContentAction);
			pasteSiteContentAction.CopyWebContents(sPWeb, sPWeb1, (bool)oParams[2], (bool)oParams[3]);
			sPWeb.Dispose();
			sPWeb1.Dispose();
		}

		protected void DisposeWebTaskDelegate(object[] oParams)
		{
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			sPWeb.Dispose();
			sPWeb1.Dispose();
		}

		public override long GetAnalysisCost(Dictionary<string, string> analysisProperties)
		{
			string item;
			string str;
			if (analysisProperties == null || analysisProperties.Count == 0)
			{
				return (long)0;
			}
			if (analysisProperties.ContainsKey("ItemsChanged"))
			{
				item = analysisProperties["ItemsChanged"];
			}
			else
			{
				item = null;
			}
			string str1 = item;
			if (analysisProperties.ContainsKey("BytesChanged"))
			{
				str = analysisProperties["BytesChanged"];
			}
			else
			{
				str = null;
			}
			string str2 = str;
			long num = (long)0;
			if (str1 != null)
			{
				long.TryParse(str1, out num);
			}
			long num1 = (str2 != null ? Format.ParseFormattedSize(str2) : (long)0);
			float single = 2.5f;
			float single1 = single / (1f + single);
			float single2 = single1 * (float)num1 * 1E-06f + (1f - single1) * (float)num;
			return (long)((int)single2);
		}

		public static void InitializeMappings(PasteSiteOptions options, SPWeb sourceWeb, SPWebTemplateCollection targetTemplates)
		{
			string xML = sourceWeb.XML;
			SPWebTemplate sPWebTemplate = targetTemplates.MapWebTemplate(sourceWeb.Template);
			options.WebTemplateName = (sPWebTemplate != null ? sPWebTemplate.Name : "STS#0");
			foreach (SPWebTemplate template in sourceWeb.Templates)
			{
				if (options.WebTemplateMappingTable.ContainsKey(template.Name) || template.ID == -1)
				{
					continue;
				}
				sPWebTemplate = targetTemplates.MapWebTemplate(template) ?? targetTemplates.Find(1, 1);
				options.WebTemplateMappingTable.Add(template.Name, sPWebTemplate.Name);
			}
		}

		protected override void InitializeSharePointCopy(IXMLAbleList source, IXMLAbleList target, bool bRefresh)
		{
			base.InitializeSharePointCopy(source, target, bRefresh);
			if (base.SourceTargetNavCopyMap != null)
			{
				base.SourceTargetNavCopyMap.Clear();
			}
		}

		private bool IsValidSiteURL(SPWeb sourceWeb, SPWeb targetWeb, string sNewSiteName)
		{
			if (Utils.IsValidSharePointURL(sNewSiteName, false))
			{
				return true;
			}
			LogItem logItem = new LogItem("Copying site failed", sNewSiteName, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
			{
				Information = string.Concat(Metalogix.Actions.Properties.Resources.ValidSiteURL, " ", Utils.IllegalCharactersForSiteUrl),
				Status = ActionOperationStatus.Failed
			};
			base.FireOperationStarted(logItem);
			base.FireOperationFinished(logItem);
			return false;
		}

		private void PopulateLinkCorrector(SPWeb sourceWeb, SPWeb targetWeb)
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
								logItem = new LogItem("Comprehensive Link Mapping Generation for Site Copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem);
							}
							base.LinkCorrector.Scope = base.SharePointOptions.LinkCorrectionScope;
							base.LinkCorrector.PopulateForSiteCopy(sourceWeb, targetWeb, base.SharePointOptions.TaskCollection, base.SharePointOptions.UseComprehensiveLinkCorrection);
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
							logItem = new LogItem("Initialize Link Corrector for Site Copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
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

		protected override void RunAction(IXMLAbleList sourceCollection, IXMLAbleList targetCollection)
		{
			bool flag = false;
			base.InitializeWorkflow();
			IEnumerator enumerator = targetCollection.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					object current = enumerator.Current;
					try
					{
						using (SPWeb sPWeb = current as SPWeb)
						{
							if (sPWeb == null)
							{
								throw new Exception("Invalid target type selected");
							}
							NodeCollection nodeCollection = new NodeCollection(new Node[] { sPWeb });
							this.InitializeSharePointCopy(sourceCollection, nodeCollection, base.SharePointOptions.ForceRefresh);
							List<TaskDefinition> taskDefinitions = new List<TaskDefinition>();
							foreach (SPWeb sPWeb1 in sourceCollection)
							{
								SPWeb parent = sPWeb1.Parent as SPWeb;
								SPWebCollection sPAdHocWebCollection = null;
								if (parent == null)
								{
									sPAdHocWebCollection = new SPAdHocWebCollection(sPWeb1);
								}
								else
								{
									sPAdHocWebCollection = parent.SubWebs;
								}
								try
								{
									try
									{
										if (!base.CheckForAbort())
										{
											this.webTransformerDefinition.BeginTransformation(this, sPAdHocWebCollection, sPWeb.SubWebs, this.Options.Transformers);
											if (!flag)
											{
												flag = true;
												base.InitializeAudienceMappings(sPWeb1, sPWeb);
											}
											taskDefinitions.Add(this.CopySite(sPWeb1, sPWeb, true));
										}
										else
										{
											break;
										}
									}
									catch (Exception exception1)
									{
										Exception exception = exception1;
										LogItem logItem = new LogItem("Error copying site", "", sPWeb1.DisplayUrl, sPWeb.DisplayUrl, ActionOperationStatus.Failed)
										{
											Exception = exception
										};
										base.FireOperationStarted(logItem);
										base.FireOperationFinished(logItem);
									}
								}
								finally
								{
									this.webTransformerDefinition.EndTransformation(this, sPAdHocWebCollection, sPWeb.SubWebs, this.Options.Transformers);
									sPWeb1.Dispose();
								}
							}
							base.ThreadManager.WaitForTasks(taskDefinitions);
							base.CopySiteNavigationStructure(sPWeb);
						}
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						SPWeb sPWeb2 = current as SPWeb;
						LogItem logItem1 = new LogItem("Error copying sites", "", "", (sPWeb2 == null ? "" : sPWeb2.DisplayUrl), ActionOperationStatus.Failed)
						{
							Exception = exception2
						};
						base.FireOperationStarted(logItem1);
						base.FireOperationFinished(logItem1);
					}
				}
				while (!base.CheckForAbort());
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			base.StartCommonWorkflowBufferedTasks();
			base.ThreadManager.SetBufferedTasks("CalendarOverlayLinkCorrection", false, true);
			base.ThreadManager.SetBufferedTasks("CopyDocumentTemplatesforContentTypes", true, true);
		}
	}
}