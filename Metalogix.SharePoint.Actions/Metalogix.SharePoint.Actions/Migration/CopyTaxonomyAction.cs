using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Interfaces;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.Taxonomy.Generic;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Metalogix.Actions.AllowsSameSourceTarget(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Termstore.ico")]
	[Name("Paste Managed Metadata Term Stores")]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(ITaxonomyConnection))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(ITaxonomyConnection))]
	public class CopyTaxonomyAction : PasteAction<PasteTaxonomyOptions>
	{
		public const string C_ExclusionTermstoreName = "<Exclude>";

		public const string C_NoTermstoreName = "<No Term Store Exists>";

		protected const string C_OperationResultsKey = "Results";

		protected const string C_LabelAlreadyExistsHResult = "-2146233088";

		private const int C_HierarchyTermGroupIndex = 0;

		private const int C_HierarchyTermSetIndex = 1;

		protected TaxonomyStatistics globalStats = new TaxonomyStatistics();

		public CopyTaxonomyAction()
		{
		}

		protected void AddTermStoreGlobalStatistics(string sourceTSName, string targetTSName, string groupName = null)
		{
			if (this.globalStats.Usage > (long)0)
			{
				LogItem logItem = new LogItem(Resources.TermStoreStats, (string.IsNullOrEmpty(groupName) ? sourceTSName : groupName), sourceTSName, targetTSName, ActionOperationStatus.Running)
				{
					Information = string.Format(Resources.FS_MigrationStatsInfo, sourceTSName, targetTSName)
				};
				base.FireOperationStarted(logItem);
				logItem.Details = this.globalStats.ToString();
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			ITaxonomyConnection item;
			ITaxonomyConnection taxonomyConnection;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (sourceSelections.Count > 0)
			{
				item = sourceSelections[0] as ITaxonomyConnection;
			}
			else
			{
				item = null;
			}
			ITaxonomyConnection taxonomyConnection1 = item;
			if (taxonomyConnection1 != null && !taxonomyConnection1.Adapter.SupportsTaxonomy)
			{
				return false;
			}
			if (targetSelections.Count > 0)
			{
				taxonomyConnection = targetSelections[0] as ITaxonomyConnection;
			}
			else
			{
				taxonomyConnection = null;
			}
			ITaxonomyConnection taxonomyConnection2 = taxonomyConnection;
			if (taxonomyConnection2 != null && !taxonomyConnection2.Adapter.SupportsTaxonomy)
			{
				return false;
			}
			return true;
		}

		private void CopyReferencedManagedMetadataByIdApproach(string referencedManagedMetadataFields, SPWeb targetWeb)
		{
			LogItem logItem = null;
			try
			{
				this.globalStats.Reset();
				XmlDocument xmlDocument = new XmlDocument()
				{
					PreserveWhitespace = true
				};
				logItem = new LogItem("Adding Referenced Taxonomy Data", Resources.Taxonomy, null, targetWeb.DisplayName, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				referencedManagedMetadataFields = this.MapTaxonomyData(referencedManagedMetadataFields, targetWeb);
				string outerXml = targetWeb.Adapter.Writer.AddReferencedTaxonomyData(referencedManagedMetadataFields);
				if (!string.IsNullOrEmpty(outerXml))
				{
					xmlDocument.LoadXml(outerXml);
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
					XmlNodeList xmlNodeLists = xmlDocument.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
					XmlNode xmlNodes = xmlDocument.SelectSingleNode(string.Format("{0}/{1}", TaxonomyReportingElements.Result.ToString(), TaxonomyReportingElements.Statistics.ToString()));
					TaxonomyStatistics taxonomyStatistic = new TaxonomyStatistics(xmlNodes);
					logItem.LicenseDataUsed = taxonomyStatistic.Usage;
					this.globalStats.Add(taxonomyStatistic);
					int num = this.ReviseKnownErrors(elementsByTagName);
					if (num > 0)
					{
						outerXml = xmlDocument.OuterXml;
					}
					if (elementsByTagName.Count > 0 && num != elementsByTagName.Count)
					{
						Exception exception = new Exception(Resources.ErrorsHaveOccured);
						exception.Data.Add("Results", outerXml);
						throw exception;
					}
					if (xmlNodeLists.Count > 0 || num > 0)
					{
						logItem.Status = ActionOperationStatus.Warning;
						logItem.Information = string.Format(Resources.FS_WarningsHaveOccured, (num > 0 ? Resources.TermsExistWithDifferentGuid : string.Empty));
					}
					logItem.Details = outerXml;
				}
				if (logItem.Status == ActionOperationStatus.Running)
				{
					logItem.Status = ActionOperationStatus.Completed;
				}
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception2)
			{
				Exception exception1 = exception2;
				logItem.Exception = exception1;
				if (exception1.Data.Contains("Results"))
				{
					LogItem logItem1 = logItem;
					logItem1.Details = string.Concat(logItem1.Details, Environment.NewLine, Environment.NewLine, exception1.Data["Results"].ToString());
				}
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyRequiredManagedMetadata(IDictionary<Guid, IList<Guid>> requiredManagedMetadata, ITaxonomySiteConnection sourceSite, ITaxonomySiteConnection targetSite)
		{
			ActionOperationStatus actionOperationStatu;
			if (requiredManagedMetadata == null)
			{
				throw new ArgumentNullException("requiredManagedMetadata");
			}
			if (sourceSite == null)
			{
				throw new ArgumentNullException("sourceSite");
			}
			if (targetSite == null)
			{
				throw new ArgumentNullException("targetSite");
			}
			LogItem noSourceTermstores = null;
			LogItem logItem = null;
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			XmlDocument xmlDocument = null;
			string empty = string.Empty;
			string name = string.Empty;
			this.CreateAndStartLogItem(ref noSourceTermstores, false, Resources.MigratingAndResolvingRequiredManagedMetadata, Resources.Taxonomy, sourceSite.DisplayUrl, targetSite.DisplayUrl, ActionOperationStatus.Running);
			try
			{
				try
				{
					if (this.ActionOptions.ForceRefresh)
					{
						this.RefreshTermStores(sourceSite, targetSite);
					}
					SPTermStore item = null;
					SPTermStore defaultSiteCollectionTermStore = null;
					SPTermGroup sPTermGroup = null;
					try
					{
						if (sourceSite.TermStores.Count == 0)
						{
							noSourceTermstores.Information = Resources.NoSourceTermstores;
							flag4 = false;
							return;
						}
						else if (targetSite.TermStores.Count != 0)
						{
							defaultSiteCollectionTermStore = targetSite.TermStores.DefaultSiteCollectionTermStore;
							if (defaultSiteCollectionTermStore == null)
							{
								noSourceTermstores.Information = Resources.UnableToDetemineDefaultTargetSiteCollectionTermstore;
								noSourceTermstores.Details = Resources.UnableToDetermineSiteCollectionTermstoreDetails;
								flag4 = false;
								return;
							}
							else if (!base.SharePointOptions.ResolveManagedMetadataByName && object.Equals(defaultSiteCollectionTermStore.Id, item.Id) && string.Equals(item.Name, defaultSiteCollectionTermStore.Name))
							{
								noSourceTermstores.Information = Resources.SingleMMDServiceNotSupportedYet;
								flag4 = false;
								return;
							}
						}
						else
						{
							noSourceTermstores.Information = Resources.NoTargetTermstores;
							flag4 = false;
							return;
						}
					}
					finally
					{
						if (!flag4)
						{
							base.Cancel();
							noSourceTermstores.Status = ActionOperationStatus.Failed;
						}
					}
					if (flag4)
					{
						xmlDocument = new XmlDocument();
						XmlDocument xmlDocument1 = new XmlDocument()
						{
							PreserveWhitespace = true
						};
						flag2 = false;
						flag3 = false;
						flag4 = true;
						this.globalStats.Reset();
						try
						{
							try
							{
								foreach (KeyValuePair<Guid, IList<Guid>> requiredManagedMetadatum in requiredManagedMetadata)
								{
									item = sourceSite.TermStores[requiredManagedMetadatum.Key];
									if (item == null)
									{
										continue;
									}
									defaultSiteCollectionTermStore = this.GetTargetTermstore(item.Id, targetSite);
									IDictionary<string, IList<string>> distinctGroupAndTermSetsForTermstore = this.GetDistinctGroupAndTermSetsForTermstore(sourceSite, requiredManagedMetadatum.Key, requiredManagedMetadatum.Value);
									Metalogix.Transformers.TransformationRepository transformationRepository = base.TransformationRepository;
									string lower = item.Id.ToString("D").ToLower();
									Guid id = defaultSiteCollectionTermStore.Id;
									transformationRepository.Add("$TERMSTORE$", lower, id.ToString("D").ToLower());
									foreach (KeyValuePair<string, IList<string>> keyValuePair in distinctGroupAndTermSetsForTermstore)
									{
										sPTermGroup = item.Groups[keyValuePair.Key];
										empty = item.Name;
										name = defaultSiteCollectionTermStore.Name;
										if (!base.SharePointOptions.ResolveManagedMetadataByName && !this.SynchroniseReusedTerms(sourceSite, targetSite, item, defaultSiteCollectionTermStore, sPTermGroup.Name, keyValuePair.Value, targetSite.RootSiteGUID))
										{
											flag = true;
										}
										xmlDocument.LoadXml(sPTermGroup.ToXMLSpecific(keyValuePair.Value));
										this.ResolveGroupManagedMetadataByName(xmlDocument.FirstChild, item, defaultSiteCollectionTermStore, targetSite.RootSiteGUID);
										if (base.TransformationRepository.GetValueForKey("PropertyBag", "$TSACR$") == null)
										{
											continue;
										}
										this.MapUsersFromGlobalSettings(xmlDocument, string.Format("Transforming users for group '{0}'", sPTermGroup.Name), item.Name, defaultSiteCollectionTermStore.Name, targetSite.Adapter.SharePointVersion.IsSharePointOnline);
										this.SplitTermGroupXmlIntoChunks(targetSite, xmlDocument, logItem, sPTermGroup, item, defaultSiteCollectionTermStore, xmlDocument1, flag2, flag3, flag, flag1);
									}
								}
							}
							catch (Exception exception)
							{
								noSourceTermstores.Exception = exception;
							}
						}
						finally
						{
							this.AddTermStoreGlobalStatistics(empty, name, null);
						}
					}
					else
					{
						return;
					}
				}
				catch (Exception exception1)
				{
					noSourceTermstores.Exception = exception1;
				}
			}
			finally
			{
				if (noSourceTermstores.Status != ActionOperationStatus.Failed)
				{
					LogItem logItem1 = noSourceTermstores;
					if (flag)
					{
						actionOperationStatu = ActionOperationStatus.Failed;
					}
					else
					{
						actionOperationStatu = (flag1 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
					}
					logItem1.Status = actionOperationStatu;
					if (flag || flag1)
					{
						noSourceTermstores.Information = string.Format(Resources.FS_ReviewResults, string.Empty);
						if (string.IsNullOrEmpty(noSourceTermstores.Details))
						{
							noSourceTermstores.Details = Resources.PleaseReviewTheRelatedLogItemsForDetails;
						}
					}
				}
				base.FireOperationFinished(noSourceTermstores);
			}
		}

		private void CopyTermStores(ITaxonomyConnection sourceServer, ITaxonomyConnection targetServer)
		{
			ActionOperationStatus actionOperationStatu;
			ActionOperationStatus actionOperationStatu1;
			LogItem pleaseReviewTheRelatedLogItemsForDetails = null;
			LogItem logItem = null;
			LogItem logItem1 = null;
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			XmlDocument xmlDocument = null;
			string empty = string.Empty;
			string name = string.Empty;
			this.CreateAndStartLogItem(ref pleaseReviewTheRelatedLogItemsForDetails, false, Resources.MigratingServerTaxonomies, Resources.Taxonomy, sourceServer.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
			try
			{
				try
				{
					if (this.ActionOptions.ForceRefresh)
					{
						this.RefreshTermStores(sourceServer, targetServer);
					}
					if (sourceServer.TermStores.Count == 0 && flag4)
					{
						pleaseReviewTheRelatedLogItemsForDetails.Information = "No Term Stores exist on the source server.";
						flag4 = false;
					}
					if (targetServer.TermStores.Count == 0 && flag4)
					{
						pleaseReviewTheRelatedLogItemsForDetails.Information = "No Term Stores exist on the target server.";
						flag4 = false;
					}
					if (base.SharePointOptions.TermstoreNameMappingTable.Count == 0 && flag4)
					{
						pleaseReviewTheRelatedLogItemsForDetails.Information = "No Term stores have been mapped for migration.";
						flag4 = false;
					}
					if (flag4)
					{
						xmlDocument = new XmlDocument();
						XmlDocument xmlDocument1 = new XmlDocument()
						{
							PreserveWhitespace = true
						};
						foreach (KeyValuePair<string, string> termstoreNameMappingTable in base.SharePointOptions.TermstoreNameMappingTable)
						{
							if (termstoreNameMappingTable.Key == Guid.Empty.ToString())
							{
								continue;
							}
							if (base.CheckForAbort())
							{
								break;
							}
							flag2 = false;
							flag3 = false;
							this.globalStats.Reset();
							logItem = null;
							try
							{
								try
								{
									SPTermStore item = sourceServer.TermStores[new Guid(termstoreNameMappingTable.Key)];
									if (item == null)
									{
										flag3 = true;
										this.CreateAndStartLogItem(ref logItem, true, Resources.MigratingTermStore, termstoreNameMappingTable.Key, sourceServer.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
										logItem.Status = ActionOperationStatus.Skipped;
										logItem.Information = string.Concat("The configured source term store has been skipped because it was unavailable on the source. ", Resources.EnsurePermissionsGranted);
										continue;
									}
									else if (termstoreNameMappingTable.Value != Guid.Empty.ToString())
									{
										this.CreateAndStartLogItem(ref logItem, true, Resources.MigratingTermStore, item.Name, sourceServer.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
										SPTermStore sPTermStore = targetServer.TermStores[new Guid(termstoreNameMappingTable.Value)];
										if (sPTermStore != null)
										{
											empty = item.Name;
											name = sPTermStore.Name;
											ISharePointWriter writer = targetServer.Adapter.Writer;
											Guid id = sPTermStore.Id;
											string str = writer.AddTermstoreLanguages(id.ToString(), item.LanguageCollection);
											if (!string.IsNullOrEmpty(str))
											{
												xmlDocument1.LoadXml(str);
												XmlNodeList elementsByTagName = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
												XmlNodeList xmlNodeLists = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
												if (elementsByTagName.Count > 0)
												{
													Exception exception = new Exception(Resources.ErrorsHaveOccured);
													exception.Data.Add("Results", str);
													throw exception;
												}
												if (xmlNodeLists.Count > 0)
												{
													flag3 = true;
													logItem.Status = ActionOperationStatus.Warning;
													logItem.Information = string.Format(Resources.FS_WarningsHaveOccured, string.Empty);
													LogItem logItem2 = logItem;
													logItem2.Details = string.Concat(logItem2.Details, (string.IsNullOrEmpty(logItem.Details) ? string.Empty : string.Concat(Environment.NewLine, Environment.NewLine)), str);
												}
											}
											if (!base.CheckForAbort())
											{
												if (!base.SharePointOptions.ResolveManagedMetadataByName && !this.SynchroniseReusedTerms(sourceServer, targetServer, item, sPTermStore, null, null, null))
												{
													flag2 = true;
												}
												if (!base.CheckForAbort())
												{
													foreach (SPTermGroup group in (IEnumerable<SPTermGroup>)item.Groups)
													{
														if (base.CheckForAbort())
														{
															break;
														}
														if ((!group.IsSiteCollectionGroup ? true : string.Equals(sourceServer.Url, targetServer.Url, StringComparison.OrdinalIgnoreCase)))
														{
															xmlDocument.LoadXml(group.ToXML());
															if (base.SharePointOptions.ResolveManagedMetadataByName)
															{
																this.ResolveGroupManagedMetadataByName(xmlDocument.FirstChild, item, sPTermStore, null);
																if (base.TransformationRepository.GetValueForKey("PropertyBag", "$TSACR$") == null && group.TermSets.Count > 0)
																{
																	continue;
																}
															}
															this.MapUsersFromGlobalSettings(xmlDocument, string.Format("Transforming users for group '{0}'", group.Name), item.Name, sPTermStore.Name, targetServer.Adapter.SharePointVersion.IsSharePointOnline);
															if (group.TermSets.Count != 0)
															{
																this.SplitTermGroupXmlIntoChunks(targetServer, xmlDocument, logItem1, group, item, sPTermStore, xmlDocument1, flag2, flag3, flag, flag1);
															}
															else
															{
																XmlDocument xmlDocument2 = new XmlDocument();
																XmlNode xmlNodes = xmlDocument2.ImportNode(xmlDocument.FirstChild, false);
																xmlDocument2.AppendChild(xmlNodes);
																logItem1 = this.CreateTermsInBatch(targetServer, logItem1, group, item, sPTermStore, xmlDocument2, 1, xmlDocument1, ref flag2, ref flag3, ref flag, ref flag1);
															}
														}
														else
														{
															if (!base.SharePointOptions.LogSkippedItems)
															{
																continue;
															}
															this.CreateAndStartLogItem(ref logItem1, true, Resources.AddingTaxonomyGroup, group.Name, sourceServer.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
															logItem1.Status = ActionOperationStatus.Skipped;
															logItem1.Information = string.Format("Site Collection Term Group'{0}' has been skipped as Site Collection Term Group migration to a different server during an entire Termstore Migration operation is not supported in this version.", group.Name);
															base.FireOperationFinished(logItem1);
														}
													}
													LogItem logItem3 = logItem;
													if (flag2)
													{
														actionOperationStatu = ActionOperationStatus.Failed;
													}
													else
													{
														actionOperationStatu = (flag3 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
													}
													logItem3.Status = actionOperationStatu;
													if (flag2 || flag3)
													{
														logItem.Information = string.Format(Resources.FS_IssuesDuringMigration, string.Empty);
													}
												}
												else
												{
													break;
												}
											}
											else
											{
												break;
											}
										}
										else
										{
											flag3 = true;
											logItem.Status = ActionOperationStatus.Skipped;
											logItem.Information = string.Format(string.Concat("Term store '{0}' has been skipped because the configured target term store could not be found. ", Resources.EnsurePermissionsGranted), item.Name);
											continue;
										}
									}
									else
									{
										if (base.SharePointOptions.LogSkippedItems)
										{
											this.CreateAndStartLogItem(ref logItem, true, Resources.MigratingTermStore, item.Name, sourceServer.DisplayUrl, targetServer.DisplayUrl, ActionOperationStatus.Running);
											logItem.Status = ActionOperationStatus.Skipped;
											logItem.Information = string.Format("Term store '{0}' has been skipped due to being excluded in the mapping options.", item.Name);
										}
										continue;
									}
								}
								catch (Exception exception2)
								{
									Exception exception1 = exception2;
									flag2 = true;
									logItem.Exception = exception1;
									if (exception1.Data.Contains("Results"))
									{
										LogItem logItem4 = logItem;
										logItem4.Details = string.Concat(logItem4.Details, (string.IsNullOrEmpty(logItem.Details) ? string.Empty : string.Concat(Environment.NewLine, Environment.NewLine)), exception1.Data["Results"].ToString());
									}
								}
							}
							finally
							{
								if (flag2)
								{
									flag = true;
								}
								if (flag3)
								{
									flag1 = true;
								}
								if (logItem != null)
								{
									base.FireOperationFinished(logItem);
								}
								this.AddTermStoreGlobalStatistics(empty, name, null);
							}
						}
					}
					else
					{
						base.Cancel();
						pleaseReviewTheRelatedLogItemsForDetails.Status = ActionOperationStatus.Failed;
						return;
					}
				}
				catch (Exception exception3)
				{
					pleaseReviewTheRelatedLogItemsForDetails.Exception = exception3;
				}
			}
			finally
			{
				if (pleaseReviewTheRelatedLogItemsForDetails.Status != ActionOperationStatus.Failed)
				{
					LogItem logItem5 = pleaseReviewTheRelatedLogItemsForDetails;
					if (flag)
					{
						actionOperationStatu1 = ActionOperationStatus.Failed;
					}
					else
					{
						actionOperationStatu1 = (flag1 ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
					}
					logItem5.Status = actionOperationStatu1;
					if (flag || flag1)
					{
						pleaseReviewTheRelatedLogItemsForDetails.Information = string.Format(Resources.FS_ReviewResults, Environment.NewLine);
						if (string.IsNullOrEmpty(pleaseReviewTheRelatedLogItemsForDetails.Details))
						{
							pleaseReviewTheRelatedLogItemsForDetails.Details = Resources.PleaseReviewTheRelatedLogItemsForDetails;
						}
					}
				}
				base.FireOperationFinished(pleaseReviewTheRelatedLogItemsForDetails);
			}
		}

	    protected void CreateAndStartLogItem(ref LogItem logItem, bool alwaysCreateNew, string operation, string itemName, string source, string target, ActionOperationStatus status = ActionOperationStatus.Running)
	    {
	        if (logItem == null || alwaysCreateNew)
	        {
	            logItem = new LogItem(operation, itemName, source, target, status);
	            base.FireOperationStarted(logItem);
	        }
	    }

        private LogItem CreateTermsInBatch(ITaxonomyConnection targetConnection, LogItem migrateGroupOperationLog, SPTermGroup sourceGroup, SPTermStore sourceTermStore, SPTermStore targetTermStore, XmlDocument xmlDoc, int termsBatchNo, XmlDocument resultXmlDoc, ref bool errorOccured, ref bool warningOccured, ref bool overallErrors, ref bool overallWarnings)
		{
			this.CreateAndStartLogItem(ref migrateGroupOperationLog, true, string.Concat(Resources.AddingTaxonomyGroup, "-Batch ", termsBatchNo.ToString()), sourceGroup.Name, sourceTermStore.Name, targetTermStore.Name, ActionOperationStatus.Running);
			try
			{
				try
				{
					ISharePointWriter writer = targetConnection.Adapter.Writer;
					Guid id = targetTermStore.Id;
					string str = writer.AddTermGroup(id.ToString(), xmlDoc.OuterXml, false);
					if (!string.IsNullOrEmpty(str))
					{
						resultXmlDoc.LoadXml(str);
						XmlNodeList elementsByTagName = resultXmlDoc.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
						XmlNodeList xmlNodeLists = resultXmlDoc.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
						XmlNode xmlNodes = resultXmlDoc.SelectSingleNode(string.Format("{0}/{1}", TaxonomyReportingElements.Result.ToString(), TaxonomyReportingElements.Statistics.ToString()));
						TaxonomyStatistics taxonomyStatistic = new TaxonomyStatistics(xmlNodes);
						migrateGroupOperationLog.LicenseDataUsed = taxonomyStatistic.Usage;
						this.globalStats.Add(taxonomyStatistic);
						errorOccured = elementsByTagName.Count > 0;
						warningOccured = xmlNodeLists.Count > 0;
						if (errorOccured || warningOccured)
						{
							migrateGroupOperationLog.Information = string.Format((errorOccured ? Resources.FS_IssuesDuringMigration : Resources.FS_WarningsHaveOccured), string.Empty);
							migrateGroupOperationLog.Status = (errorOccured ? ActionOperationStatus.Failed : ActionOperationStatus.Warning);
						}
						migrateGroupOperationLog.Details = str;
					}
					if (migrateGroupOperationLog.Status == ActionOperationStatus.Running)
					{
						migrateGroupOperationLog.Status = ActionOperationStatus.Completed;
					}
				}
				catch (Exception exception)
				{
					migrateGroupOperationLog.Exception = exception;
				}
			}
			finally
			{
				if (errorOccured)
				{
					overallErrors = true;
				}
				if (warningOccured)
				{
					overallWarnings = true;
				}
				base.FireOperationFinished(migrateGroupOperationLog);
			}
			return migrateGroupOperationLog;
		}

		private IDictionary<string, IList<string>> GetDistinctGroupAndTermSetsForTermstore(ITaxonomySiteConnection sourceSite, Guid termStoreId, IList<Guid> termSets)
		{
			Dictionary<string, IList<string>> strs = new Dictionary<string, IList<string>>();
			foreach (Guid termSet in termSets)
			{
				IList<string> termSetHierarchy = null;
				SPTermStore item = null;
				if (sourceSite.TermStores != null)
				{
					item = sourceSite.TermStores[termStoreId];
					if (item != null)
					{
						termSetHierarchy = item.GetTermSetHierarchy(termSet, true);
					}
				}
				if (termSetHierarchy == null || termSetHierarchy.Count <= 1)
				{
					continue;
				}
				if (!strs.ContainsKey(termSetHierarchy[0]))
				{
					strs.Add(termSetHierarchy[0], new List<string>());
				}
				if (strs[termSetHierarchy[0]].Contains(termSetHierarchy[1]))
				{
					continue;
				}
				strs[termSetHierarchy[0]].Add(termSetHierarchy[1]);
			}
			return strs;
		}

		private string GetResolvedGuidString(SPTermSetItem termSetItem = null)
		{
			string lower = null;
			if (termSetItem != null)
			{
				lower = termSetItem.Id.ToString("D").ToLower();
			}
			else
			{
				Guid guid = Guid.NewGuid();
				lower = guid.ToString("D").ToLower();
				if (base.TransformationRepository.GetValueForKey("PropertyBag", "$TSACR$") == null)
				{
					base.TransformationRepository.Add("PropertyBag", "$TSACR$", true.ToString());
				}
			}
			return lower;
		}

		protected SPTermStore GetTargetTermstore(Guid sourceTermstoreId, ITaxonomyConnection targetSite)
		{
			SPTermStore item = null;
			if (this.ActionOptions.MapTermStores)
			{
				using (IEnumerator<KeyValuePair<string, string>> enumerator = base.SharePointOptions.TermstoreNameMappingTable.GetEnumerator())
				{
					do
					{
					Label0:
						if (!enumerator.MoveNext())
						{
							break;
						}
						KeyValuePair<string, string> current = enumerator.Current;
						if (!object.Equals(new Guid(current.Value), Guid.Empty) && object.Equals(sourceTermstoreId, new Guid(current.Key)))
						{
							item = targetSite.TermStores[new Guid(current.Value)];
						}
						else
						{
							goto Label0;
						}
					}
					while (item == null);
				}
			}
			if (item == null)
			{
				item = targetSite.TermStores.DefaultSiteCollectionTermStore;
			}
			return item;
		}

		private string MapTaxonomyData(string sTaxonomyXml, SPWeb targetWeb)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sTaxonomyXml);
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (base.SharePointOptions.TermstoreNameMappingTable.ContainsKey(childNode.Attributes["TermStoreId"].Value))
				{
					string item = base.SharePointOptions.TermstoreNameMappingTable[childNode.Attributes["TermStoreId"].Value];
					if (item != Guid.Empty.ToString())
					{
						childNode.Attributes["TermStoreId"].Value = item;
					}
				}
				if (!(childNode.Name == "SPTermSet") || childNode.Attributes["IsSiteCollectionGroup"] == null || !Convert.ToBoolean(childNode.Attributes["IsSiteCollectionGroup"].Value))
				{
					continue;
				}
				XmlNode rootSiteGUID = childNode.SelectSingleNode("./SPTermGroup/SPSiteCollectionAccessIds");
				if (rootSiteGUID == null || rootSiteGUID.ChildNodes.Count <= 0)
				{
					continue;
				}
				rootSiteGUID.FirstChild.InnerText = targetWeb.RootSiteGUID;
			}
			return xmlNode.OuterXml;
		}

		protected void MapUsersFromGlobalSettings(XmlDocument xmlDoc, string operation, string source, string target, bool isTargetSharePointOnline)
		{
			if (SPGlobalMappings.GlobalUserMappings.Count == 0 && !isTargetSharePointOnline)
			{
				return;
			}
			bool flag = false;
			LogItem logItem = null;
			try
			{
				try
				{
					StringBuilder stringBuilder = new StringBuilder(255);
					foreach (XmlElement elementsByTagName in xmlDoc.GetElementsByTagName(TaxonomyFields.User.ToString()))
					{
						if (!flag)
						{
							stringBuilder.Length = 0;
							stringBuilder.Append(elementsByTagName.InnerText);
						}
						elementsByTagName.InnerText = SPGlobalMappings.Map(elementsByTagName.InnerText, isTargetSharePointOnline);
						if (flag || string.Equals(stringBuilder.ToString(), elementsByTagName.InnerText))
						{
							continue;
						}
						flag = true;
					}
					foreach (XmlElement xmlElement in xmlDoc.GetElementsByTagName(TaxonomyClassType.SPTermSet.ToString()))
					{
						XmlAttribute itemOf = xmlElement.Attributes[TaxonomyFields.Owner.ToString()];
						if (itemOf == null)
						{
							continue;
						}
						if (!flag)
						{
							stringBuilder.Length = 0;
							stringBuilder.Append(itemOf.Value);
						}
						itemOf.Value = SPGlobalMappings.Map(itemOf.Value, isTargetSharePointOnline);
						if (flag || string.Equals(stringBuilder.ToString(), itemOf.Value))
						{
							continue;
						}
						flag = true;
					}
					foreach (XmlElement elementsByTagName1 in xmlDoc.GetElementsByTagName(TaxonomyClassType.SPTerm.ToString()))
					{
						XmlAttribute xmlAttribute = elementsByTagName1.Attributes[TaxonomyFields.Owner.ToString()];
						if (xmlAttribute == null)
						{
							continue;
						}
						if (!flag)
						{
							stringBuilder.Length = 0;
							stringBuilder.Append(xmlAttribute.Value);
						}
						xmlAttribute.Value = SPGlobalMappings.Map(xmlAttribute.Value, isTargetSharePointOnline);
						if (flag || string.Equals(stringBuilder.ToString(), xmlAttribute.Value))
						{
							continue;
						}
						flag = true;
					}
					if (flag)
					{
						logItem = new LogItem(operation, "Global User Mapping", source, target, ActionOperationStatus.Running)
						{
							Status = ActionOperationStatus.Completed
						};
						base.FireOperationStarted(logItem);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem == null)
					{
						logItem = new LogItem(operation, "Global User Mapping", source, target, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
					}
					logItem.Exception = exception;
					throw;
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

		protected void RefreshTermStores(ITaxonomyConnection sourceSite, ITaxonomyConnection targetSite)
		{
			Guid termCollectionId = sourceSite.TermStores.TermCollectionId;
			string str = string.Format("$TSRF_{0}", termCollectionId.ToString("D"));
			Guid guid = targetSite.TermStores.TermCollectionId;
			string str1 = string.Format("$TSRF_{0}", guid.ToString("D"));
			string valueForKey = base.TransformationRepository.GetValueForKey("PropertyBag", str);
			string valueForKey1 = base.TransformationRepository.GetValueForKey("PropertyBag", str1);
			if (valueForKey == null || valueForKey1 == null)
			{
				StringBuilder stringBuilder = new StringBuilder(128);
				LogItem logItem = new LogItem(Resources.RefreshingTermstores, Resources.Taxonomy, sourceSite.DisplayUrl, targetSite.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					try
					{
						if (valueForKey == null)
						{
							sourceSite.TermStores.FetchData();
							base.TransformationRepository.Add("PropertyBag", str, true.ToString());
							stringBuilder.AppendLine(Resources.SourceTermstoresRefreshed);
						}
						if (valueForKey1 == null)
						{
							targetSite.TermStores.FetchData();
							base.TransformationRepository.Add("PropertyBag", str1, true.ToString());
							stringBuilder.AppendLine(Resources.TargetTermstoresRefreshed);
						}
						logItem.Status = ActionOperationStatus.Completed;
						logItem.Details = stringBuilder.ToString();
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Warning;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
			}
		}

		private void ResolveDependencyTerms(XmlNode parentTermCollectionNode, SPTermStore sourceTermstore, SPTermStore targetTermstore, string targetRootSiteGuid)
		{
			LogItem logItem = null;
			try
			{
				try
				{
					foreach (XmlNode xmlNodes in parentTermCollectionNode)
					{
						this.ResolveTerm(xmlNodes, sourceTermstore, targetTermstore, targetRootSiteGuid);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem == null)
					{
						this.CreateAndStartLogItem(ref logItem, false, "Resolving Dependency Terms", "Managed Metadata", sourceTermstore.Name, targetTermstore.Name, ActionOperationStatus.Running);
					}
					logItem.Exception = exception;
					throw;
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

		protected void ResolveGroupManagedMetadataByName(XmlNode groupNode, SPTermStore sourceTermstore, SPTermStore targetTermstore, string targetRootSiteGuid = null)
		{
			LogItem logItem = null;
			try
			{
				try
				{
					if (base.TransformationRepository.GetValueForKey("PropertyBag", "$TSACR$") != null)
					{
						base.TransformationRepository.Remove("PropertyBag", "$TSACR$");
					}
					this.ResolveTermGroupAndContents(groupNode, sourceTermstore, targetTermstore, targetRootSiteGuid);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem == null)
					{
						this.CreateAndStartLogItem(ref logItem, false, "Resolving Group Taxonomy", "Managed Metadata", sourceTermstore.Name, targetTermstore.Name, ActionOperationStatus.Running);
					}
					logItem.Exception = exception;
					throw;
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

		private void ResolveTaxonomy(string termSetId, string termId, SPTermStore sourceTermstore, SPTermStore targetTermstore, string targetRootSiteGuid, ref string resolvedTermSetId, ref string resolvedTermId, XmlNode termNode = null)
		{
			IList<string> termHierarchy = null;
			SPTermGroup sPTermGroup = null;
			SPTermSet item = null;
			string lower = termSetId.ToLower();
			string str = termId.ToLower();
			resolvedTermSetId = base.TransformationRepository.GetValueForKey(lower, lower);
			resolvedTermId = base.TransformationRepository.GetValueForKey(lower, str);
			if (resolvedTermSetId == null)
			{
				termHierarchy = sourceTermstore.GetTermHierarchy(new Guid(termSetId), new Guid(termId));
				if (termHierarchy.Count > 1)
				{
					string item1 = termHierarchy[0];
					string str1 = termHierarchy[1];
					sPTermGroup = (item1 != "<SCG>" ? targetTermstore.Groups[item1] : targetTermstore.Groups.GetLocalSiteCollectionGroup(targetRootSiteGuid));
					if (sPTermGroup != null)
					{
						item = sPTermGroup.TermSets[str1];
					}
				}
				resolvedTermSetId = this.GetResolvedGuidString(item);
				base.TransformationRepository.Add(lower, lower, resolvedTermSetId);
			}
			if (base.TransformationRepository.GetValueForKey("$TSPKR$", resolvedTermSetId) == null)
			{
				base.TransformationRepository.Add("$TSPKR$", resolvedTermSetId, lower);
			}
			if (resolvedTermId == null)
			{
				if (termHierarchy == null)
				{
					termHierarchy = sourceTermstore.GetTermHierarchy(new Guid(termSetId), new Guid(termId));
					if (termHierarchy.Count > 1)
					{
						string item2 = termHierarchy[0];
						string str2 = termHierarchy[1];
						sPTermGroup = (item2 != "<SCG>" ? targetTermstore.Groups[item2] : targetTermstore.Groups.GetLocalSiteCollectionGroup(targetRootSiteGuid));
						if (sPTermGroup != null)
						{
							item = sPTermGroup.TermSets[str2];
						}
					}
				}
				if (item != null)
				{
					SPTermSetItem sPTermSetItem = null;
					if (termHierarchy.Count > 2)
					{
						sPTermSetItem = item;
						for (int i = 2; i < termHierarchy.Count && sPTermSetItem != null; i++)
						{
							sPTermSetItem = sPTermSetItem.Terms[termHierarchy[i]];
						}
					}
					resolvedTermId = this.GetResolvedGuidString(sPTermSetItem);
					if (sPTermSetItem != null && termNode != null && termNode.Attributes[TaxonomyFields.Skip.ToString()] == null)
					{
						XmlAttribute xmlAttribute = termNode.OwnerDocument.CreateAttribute(TaxonomyFields.Skip.ToString());
						xmlAttribute.Value = true.ToString();
						termNode.Attributes.Append(xmlAttribute);
					}
				}
				else
				{
					resolvedTermId = this.GetResolvedGuidString(null);
				}
				base.TransformationRepository.Add(lower, str, resolvedTermId);
			}
		}

		private void ResolveTerm(XmlNode termNode, SPTermStore sourceTermstore, SPTermStore targetTermstore, string targetRootSiteGuid)
		{
			bool flag = false;
			XmlNode xmlNodes = termNode.SelectSingleNode(TaxonomyClassType.DependencyCollection.ToString());
			if (flag && xmlNodes != null)
			{
				foreach (XmlNode xmlNodes1 in xmlNodes)
				{
					this.ResolveTerm(xmlNodes1, sourceTermstore, targetTermstore, targetRootSiteGuid);
				}
			}
			if (!flag)
			{
				XmlNode xmlNodes2 = termNode.SelectSingleNode(TaxonomyClassType.SPReusedTermCollection.ToString());
				if (xmlNodes2 != null)
				{
					xmlNodes2.RemoveAll();
				}
				if (xmlNodes != null)
				{
					xmlNodes.RemoveAll();
				}
			}
			string attributeValueAsString = termNode.GetAttributeValueAsString(TaxonomyFields.Id.ToString());
			string str = termNode.GetAttributeValueAsString(TaxonomyFields.TermSetId.ToString());
			string str1 = null;
			string str2 = null;
			this.ResolveTaxonomy(str, attributeValueAsString, sourceTermstore, targetTermstore, targetRootSiteGuid, ref str1, ref str2, termNode);
			termNode.Attributes[TaxonomyFields.TermSetId.ToString()].Value = str1;
			termNode.Attributes[TaxonomyFields.Id.ToString()].Value = str2;
			if (!flag)
			{
				bool flag1 = false;
				termNode.Attributes[TaxonomyFields.IsReused.ToString()].Value = flag1.ToString();
			}
			XmlNode xmlNodes3 = termNode.SelectSingleNode(TaxonomyClassType.SPTermSet.ToString());
			if (xmlNodes3 != null)
			{
				xmlNodes3.Attributes[TaxonomyFields.Id.ToString()].Value = str1;
			}
			XmlNode xmlNodes4 = termNode.SelectSingleNode(string.Format("{0}/{1}", TaxonomyClassType.SPParentTerm.ToString(), TaxonomyClassType.SPTerm.ToString()));
			if (xmlNodes4 != null)
			{
				string attributeValueAsString1 = xmlNodes4.GetAttributeValueAsString(TaxonomyFields.Id.ToString());
				string attributeValueAsString2 = xmlNodes4.GetAttributeValueAsString(TaxonomyFields.TermSetId.ToString());
				string str3 = null;
				string str4 = null;
				this.ResolveTaxonomy(attributeValueAsString2, attributeValueAsString1, sourceTermstore, targetTermstore, targetRootSiteGuid, ref str4, ref str3, null);
				xmlNodes4.Attributes[TaxonomyFields.Id.ToString()].Value = str3;
				xmlNodes4.Attributes[TaxonomyFields.TermSetId.ToString()].Value = str4;
				termNode.Attributes[TaxonomyFields.ParentTermId.ToString()].Value = str3;
			}
			XmlNode xmlNodes5 = termNode.SelectSingleNode(string.Format("{0}/{1}", TaxonomyClassType.SPSourceTerm.ToString(), TaxonomyClassType.SPTerm.ToString()));
			if (xmlNodes5 != null)
			{
				if (!flag)
				{
					xmlNodes5.ParentNode.RemoveChild(xmlNodes5);
				}
				else
				{
					string attributeValueAsString3 = xmlNodes5.GetAttributeValueAsString(TaxonomyFields.Id.ToString());
					string attributeValueAsString4 = xmlNodes5.GetAttributeValueAsString(TaxonomyFields.TermSetId.ToString());
					string str5 = null;
					string str6 = null;
					this.ResolveTaxonomy(attributeValueAsString4, attributeValueAsString3, sourceTermstore, targetTermstore, targetRootSiteGuid, ref str6, ref str5, null);
					xmlNodes5.Attributes[TaxonomyFields.Id.ToString()].Value = str5;
					xmlNodes5.Attributes[TaxonomyFields.TermSetId.ToString()].Value = str6;
				}
			}
			XmlNode xmlNodes6 = termNode.SelectSingleNode(TaxonomyClassType.SPTermCollection.ToString());
			if (xmlNodes6 != null)
			{
				foreach (XmlNode xmlNodes7 in xmlNodes6)
				{
					this.ResolveTerm(xmlNodes7, sourceTermstore, targetTermstore, targetRootSiteGuid);
				}
			}
		}

		private void ResolveTermGroupAndContents(XmlNode groupNode, SPTermStore sourceTermstore, SPTermStore targetTermstore, string targetRootSiteGuid)
		{
			XmlNode xmlNodes = groupNode.SelectSingleNode(TaxonomyClassType.SPTermSetCollection.ToString());
			string lower = sourceTermstore.Id.ToString("D").ToLower();
			string valueForKey = base.TransformationRepository.GetValueForKey("$TERMSTORE$", lower);
			if (valueForKey != null)
			{
				base.TransformationRepository.Add("$TSPKR$", valueForKey, lower);
			}
			if (xmlNodes != null)
			{
				foreach (XmlNode childNode in xmlNodes.ChildNodes)
				{
					string str = childNode.GetAttributeValueAsString(TaxonomyFields.Id.ToString()).ToLower();
					string resolvedGuidString = base.TransformationRepository.GetValueForKey(str, str);
					if (resolvedGuidString == null)
					{
						SPTermGroup sPTermGroup = null;
						SPTermSet item = null;
						IList<string> termSetHierarchy = sourceTermstore.GetTermSetHierarchy(new Guid(str), false);
						if (termSetHierarchy.Count > 1)
						{
							string item1 = termSetHierarchy[0];
							string str1 = termSetHierarchy[1];
							sPTermGroup = (item1 != "<SCG>" ? targetTermstore.Groups[item1] : targetTermstore.Groups.GetLocalSiteCollectionGroup(targetRootSiteGuid));
							if (sPTermGroup != null)
							{
								item = sPTermGroup.TermSets[str1];
							}
						}
						resolvedGuidString = this.GetResolvedGuidString(item);
						base.TransformationRepository.Add(str, str, resolvedGuidString);
						if (item != null)
						{
							XmlAttribute xmlAttribute = childNode.OwnerDocument.CreateAttribute(TaxonomyFields.Skip.ToString());
							xmlAttribute.Value = true.ToString();
							childNode.Attributes.Append(xmlAttribute);
							XmlAttribute xmlAttribute1 = groupNode.OwnerDocument.CreateAttribute(TaxonomyFields.Skip.ToString());
							xmlAttribute1.Value = true.ToString();
							groupNode.Attributes.Append(xmlAttribute1);
						}
					}
					if (base.TransformationRepository.GetValueForKey("$TSPKR$", resolvedGuidString) == null)
					{
						base.TransformationRepository.Add("$TSPKR$", resolvedGuidString, str);
					}
					childNode.Attributes[TaxonomyFields.Id.ToString()].Value = resolvedGuidString;
					XmlNode xmlNodes1 = childNode.SelectSingleNode(TaxonomyClassType.SPTermCollection.ToString());
					if (xmlNodes1 == null)
					{
						continue;
					}
					foreach (XmlNode xmlNodes2 in xmlNodes1)
					{
						this.ResolveTerm(xmlNodes2, sourceTermstore, targetTermstore, targetRootSiteGuid);
					}
				}
			}
		}

		protected int ReviseKnownErrors(XmlNodeList errorNodes)
		{
			int num = 0;
			foreach (XmlNode errorNode in errorNodes)
			{
				XmlAttribute itemOf = errorNode.Attributes[TaxonomyReportingAttributes.HResult.ToString()];
				if (itemOf == null || !(itemOf.Value == "-2146233088"))
				{
					continue;
				}
				errorNode.Attributes.Remove(errorNode.Attributes[TaxonomyReportingAttributes.Stack.ToString()]);
				errorNode.Attributes.Remove(itemOf);
				errorNode.Attributes[TaxonomyReportingAttributes.Detail.ToString()].Value = errorNode.Attributes[TaxonomyReportingAttributes.Detail.ToString()].Value.Trim();
				num++;
			}
			return num;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.CopyTermStores(source[0] as ITaxonomyConnection, target[0] as ITaxonomyConnection);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams[0] is ITaxonomyConnection)
			{
				this.CopyTermStores(oParams[0] as ITaxonomyConnection, oParams[1] as ITaxonomyConnection);
				return;
			}
			if (oParams[0] is string)
			{
				this.CopyReferencedManagedMetadataByIdApproach(oParams[0] as string, oParams[1] as SPWeb);
				return;
			}
			this.CopyRequiredManagedMetadata(oParams[0] as IDictionary<Guid, IList<Guid>>, oParams[1] as ITaxonomySiteConnection, oParams[2] as ITaxonomySiteConnection);
		}

		private void SplitTermGroupXmlIntoChunks(ITaxonomyConnection targetConnection, XmlDocument xmlDoc, LogItem migrateGroupOperationLog, SPTermGroup sourceGroup, SPTermStore sourceTermStore, SPTermStore targetTermStore, XmlDocument resultXmlDoc, bool errorOccured, bool warningOccured, bool overallErrors, bool overallWarnings)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNodes = xmlDocument.ImportNode(xmlDoc.FirstChild, false);
			xmlNodes = xmlDocument.AppendChild(xmlNodes);
			XmlNode documentElement = xmlDocument.DocumentElement;
			XmlNode documentElement1 = xmlDoc.DocumentElement;
			XmlNode xmlNodes1 = null;
			XmlNode xmlNodes2 = null;
			XmlNode xmlNodes3 = null;
			XmlNode xmlNodes4 = null;
			foreach (XmlNode childNode in documentElement1.ChildNodes)
			{
				bool flag = string.Equals(childNode.Name, TaxonomyClassType.SPTermSetCollection.ToString());
				xmlNodes1 = xmlDocument.ImportNode(childNode, !flag);
				xmlNodes2 = documentElement.AppendChild(xmlNodes1);
				if (!flag)
				{
					continue;
				}
				xmlNodes3 = childNode;
				xmlNodes4 = xmlNodes2;
			}
			foreach (XmlNode childNode1 in xmlNodes3.ChildNodes)
			{
				documentElement = xmlNodes4;
				xmlNodes2 = documentElement.AppendChild(xmlDocument.ImportNode(childNode1, false));
				documentElement = xmlNodes2;
				XmlNode xmlNodes5 = null;
				XmlNode xmlNodes6 = null;
				foreach (XmlNode childNode2 in childNode1.ChildNodes)
				{
					xmlNodes2 = documentElement.AppendChild(xmlDocument.ImportNode(childNode2, false));
					if (!string.Equals(childNode2.Name, TaxonomyClassType.SPTermCollection.ToString()))
					{
						continue;
					}
					xmlNodes5 = childNode2;
					xmlNodes6 = xmlNodes2;
				}
				documentElement = xmlNodes6;
				XmlDocument xmlDocument1 = new XmlDocument();
				xmlNodes = xmlDocument1.ImportNode(xmlDocument.FirstChild, true);
				xmlDocument1.AppendChild(xmlNodes);
				int num = 1;
				int num1 = 0;
				foreach (XmlNode childNode3 in xmlNodes5.ChildNodes)
				{
					documentElement.AppendChild(xmlDocument.ImportNode(childNode3, true));
					num1++;
					if (num1 != AdapterConfigurationVariables.MMDTermsBatchSize)
					{
						if (childNode3 != xmlNodes5.LastChild)
						{
							continue;
						}
						migrateGroupOperationLog = this.CreateTermsInBatch(targetConnection, migrateGroupOperationLog, sourceGroup, sourceTermStore, targetTermStore, xmlDocument, num, resultXmlDoc, ref errorOccured, ref warningOccured, ref overallErrors, ref overallWarnings);
					}
					else
					{
						migrateGroupOperationLog = this.CreateTermsInBatch(targetConnection, migrateGroupOperationLog, sourceGroup, sourceTermStore, targetTermStore, xmlDocument, num, resultXmlDoc, ref errorOccured, ref warningOccured, ref overallErrors, ref overallWarnings);
						xmlDocument.LoadXml(xmlDocument1.OuterXml);
						documentElement = xmlDocument.SelectSingleNode("//SPTermCollection");
						num1 = 0;
						num++;
					}
				}
				if (xmlNodes5.ChildNodes.Count != 0)
				{
					continue;
				}
				migrateGroupOperationLog = this.CreateTermsInBatch(targetConnection, migrateGroupOperationLog, sourceGroup, sourceTermStore, targetTermStore, xmlDocument, num, resultXmlDoc, ref errorOccured, ref warningOccured, ref overallErrors, ref overallWarnings);
			}
		}

		protected bool SynchroniseReusedTerms(ITaxonomyConnection sourceServer, ITaxonomyConnection targetServer, SPTermStore sourceTermstore, SPTermStore targetTermstore, string groupName = null, IList<string> termSetNames = null, string targetRootSiteGuid = null)
		{
			if (sourceServer == null)
			{
				throw new ArgumentNullException("sourceServer");
			}
			if (targetServer == null)
			{
				throw new ArgumentNullException("targetServer");
			}
			if (sourceTermstore == null)
			{
				throw new ArgumentNullException("sourceTermstore");
			}
			if (targetTermstore == null)
			{
				throw new ArgumentNullException("targetTermstore");
			}
			SharePointAdapter adapter = targetServer.Adapter;
			XmlDocument xmlDocument = new XmlDocument();
			if (!string.IsNullOrEmpty(groupName))
			{
				xmlDocument.LoadXml(sourceTermstore.ConstructReusedTermsCollectionXML(groupName, termSetNames));
			}
			else
			{
				xmlDocument.LoadXml(sourceTermstore.ConstructReusedTermsCollectionXML(null, null));
			}
			this.MapUsersFromGlobalSettings(xmlDocument, "Transforming users in dependencies", sourceTermstore.Name, targetTermstore.Name, adapter.SharePointVersion.IsSharePointOnline);
			XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(string.Format("{0}/{1}[descendant::{2}]", TaxonomyClassType.ReusedTermsCollection.ToString(), TaxonomyClassType.ParentTermCollection.ToString(), TaxonomyClassType.SPTerm.ToString()));
			int num = (xmlNodeLists != null ? xmlNodeLists.Count : 0);
			if (num == 0)
			{
				return true;
			}
			bool flag = true;
			int num1 = 1;
			LogItem usage = null;
			string empty = string.Empty;
			XmlDocument xmlDocument1 = new XmlDocument()
			{
				PreserveWhitespace = true
			};
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				if (base.SharePointOptions.ResolveManagedMetadataByName)
				{
					this.ResolveDependencyTerms(xmlNodes, sourceTermstore, targetTermstore, targetRootSiteGuid);
				}
				if (base.CheckForAbort())
				{
					break;
				}
				this.CreateAndStartLogItem(ref usage, true, string.Format(Resources.FS_SynchronisingSet, num1, num), Resources.Dependencies, sourceTermstore.Name, targetTermstore.Name, ActionOperationStatus.Running);
				try
				{
					try
					{
						ISharePointWriter writer = adapter.Writer;
						Guid id = targetTermstore.Id;
						empty = writer.AddReusedTerms(id.ToString(), xmlNodes.OuterXml);
						if (!string.IsNullOrEmpty(empty))
						{
							xmlDocument1.LoadXml(empty);
							XmlNodeList elementsByTagName = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
							XmlNodeList elementsByTagName1 = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
							XmlNode xmlNodes1 = xmlDocument1.SelectSingleNode(string.Format("{0}/{1}", TaxonomyReportingElements.Result.ToString(), TaxonomyReportingElements.Statistics.ToString()));
							TaxonomyStatistics taxonomyStatistic = new TaxonomyStatistics(xmlNodes1);
							usage.LicenseDataUsed = taxonomyStatistic.Usage;
							this.globalStats.Add(taxonomyStatistic);
							int num2 = this.ReviseKnownErrors(elementsByTagName);
							if (num2 > 0)
							{
								empty = xmlDocument1.OuterXml;
							}
							if (elementsByTagName.Count > 0 && num2 != elementsByTagName.Count)
							{
								flag = false;
								Exception exception = new Exception(Resources.ErrorsHaveOccured);
								exception.Data.Add("Results", empty);
								throw exception;
							}
							if (elementsByTagName1.Count > 0 || num2 > 0)
							{
								usage.Status = ActionOperationStatus.Warning;
								usage.Information = string.Format(Resources.FS_WarningsHaveOccured, (num2 > 0 ? Resources.TermsExistWithDifferentGuid : string.Empty));
							}
							usage.Details = empty;
						}
						if (usage.Status == ActionOperationStatus.Running)
						{
							usage.Status = ActionOperationStatus.Completed;
						}
						num1++;
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						usage.Exception = exception1;
						if (exception1.Data.Contains("Results"))
						{
							LogItem logItem = usage;
							logItem.Details = string.Concat(logItem.Details, Environment.NewLine, Environment.NewLine, exception1.Data["Results"].ToString());
						}
					}
				}
				finally
				{
					base.FireOperationFinished(usage);
				}
			}
			return flag;
		}
	}
}