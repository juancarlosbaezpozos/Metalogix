using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Interfaces;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.Transformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Site Objects {0-Paste} > Managed Metadata Local Site Collection Group...")]
	[Name("Paste Managed Metadata Local Site Collection Group")]
	[ShowInMenus(true)]
	[SourceType(typeof(ITaxonomySiteConnection))]
	[TargetType(typeof(ITaxonomySiteConnection))]
	public class CopyTaxonomyLocalSiteCollectionGroupAction : CopyTaxonomyAction
	{
		public CopyTaxonomyLocalSiteCollectionGroupAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			ITaxonomySiteConnection item = sourceSelections[0] as ITaxonomySiteConnection;
			ITaxonomySiteConnection taxonomySiteConnection = targetSelections[0] as ITaxonomySiteConnection;
			if (item == null || taxonomySiteConnection == null)
			{
				return false;
			}
			if (string.Equals(item.RootSiteGUID, taxonomySiteConnection.RootSiteGUID, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return true;
		}

		private void CopyLocalSiteCollectionTaxonomy(ITaxonomySiteConnection sourceSite, ITaxonomySiteConnection targetSite)
		{
			ActionOperationStatus actionOperationStatu;
			LogItem noSourceTermstores = null;
			LogItem usage = null;
			bool flag = false;
			bool flag1 = false;
			bool flag2 = true;
			XmlDocument xmlDocument = null;
			string empty = string.Empty;
			string name = string.Empty;
			this.globalStats.Reset();
			base.CreateAndStartLogItem(ref noSourceTermstores, false, Resources.MigratingLocalSiteCollectionManagedMetadata, Resources.Taxonomy, sourceSite.DisplayUrl, targetSite.DisplayUrl, ActionOperationStatus.Running);
			try
			{
				try
				{
					if (this.ActionOptions.ForceRefresh)
					{
						base.RefreshTermStores(sourceSite, targetSite);
					}
					SPTermStore defaultSiteCollectionTermStore = null;
					SPTermStore targetTermstore = null;
					SPTermGroup localSiteCollectionGroup = null;
					try
					{
						if (sourceSite.TermStores.Count == 0)
						{
							noSourceTermstores.Information = Resources.NoSourceTermstores;
							flag2 = false;
							return;
						}
						else if (targetSite.TermStores.Count != 0)
						{
							defaultSiteCollectionTermStore = sourceSite.TermStores.DefaultSiteCollectionTermStore;
							if (defaultSiteCollectionTermStore != null)
							{
								localSiteCollectionGroup = defaultSiteCollectionTermStore.Groups.GetLocalSiteCollectionGroup(sourceSite.RootSiteGUID);
							}
							if (localSiteCollectionGroup == null)
							{
								foreach (SPTermStore termStore in (IEnumerable<SPTermStore>)sourceSite.TermStores)
								{
									if (defaultSiteCollectionTermStore != null && termStore.Id == defaultSiteCollectionTermStore.Id)
									{
										continue;
									}
									localSiteCollectionGroup = termStore.Groups.GetLocalSiteCollectionGroup(sourceSite.RootSiteGUID);
									if (localSiteCollectionGroup == null)
									{
										continue;
									}
									defaultSiteCollectionTermStore = termStore;
									break;
								}
							}
							if (defaultSiteCollectionTermStore == null)
							{
								noSourceTermstores.Information = Resources.UnableToDetemineDefaultSourceSiteCollectionTermstore;
								noSourceTermstores.Details = Resources.UnableToDetermineSiteCollectionTermstoreDetails;
								flag2 = false;
								return;
							}
							else if (localSiteCollectionGroup != null)
							{
								targetTermstore = base.GetTargetTermstore(defaultSiteCollectionTermStore.Id, targetSite);
								if (targetTermstore == null)
								{
									noSourceTermstores.Information = Resources.UnableToDetemineDefaultTargetSiteCollectionTermstore;
									noSourceTermstores.Details = Resources.UnableToDetermineSiteCollectionTermstoreDetails;
									flag2 = false;
									return;
								}
								else if (!base.SharePointOptions.ResolveManagedMetadataByName && object.Equals(targetTermstore.Id, defaultSiteCollectionTermStore.Id) && string.Equals(defaultSiteCollectionTermStore.Name, targetTermstore.Name))
								{
									noSourceTermstores.Information = Resources.SingleMMDServiceNotSupportedYet;
									flag2 = false;
									return;
								}
							}
							else
							{
								noSourceTermstores.Information = Resources.UnableToDetermineSourceLocalGroup;
								flag2 = false;
								return;
							}
						}
						else
						{
							noSourceTermstores.Information = Resources.NoTargetTermstores;
							flag2 = false;
							return;
						}
					}
					finally
					{
						if (!flag2)
						{
							base.Cancel();
							noSourceTermstores.Status = ActionOperationStatus.Failed;
						}
					}
					if (flag2)
					{
						xmlDocument = new XmlDocument();
						XmlDocument xmlDocument1 = new XmlDocument()
						{
							PreserveWhitespace = true
						};
						try
						{
							try
							{
								empty = defaultSiteCollectionTermStore.Name;
								name = targetTermstore.Name;
								if (!base.SharePointOptions.ResolveManagedMetadataByName && !base.SynchroniseReusedTerms(sourceSite, targetSite, defaultSiteCollectionTermStore, targetTermstore, localSiteCollectionGroup.Name, null, targetSite.RootSiteGUID))
								{
									flag = true;
								}
								xmlDocument.LoadXml(localSiteCollectionGroup.ToXML());
								if (base.SharePointOptions.ResolveManagedMetadataByName)
								{
									base.ResolveGroupManagedMetadataByName(xmlDocument.FirstChild, defaultSiteCollectionTermStore, targetTermstore, targetSite.RootSiteGUID);
									if (base.TransformationRepository.GetValueForKey("PropertyBag", "$TSACR$") == null)
									{
										return;
									}
								}
								base.MapUsersFromGlobalSettings(xmlDocument, string.Format("Transforming users for group '{0}'", localSiteCollectionGroup.Name), defaultSiteCollectionTermStore.Name, targetTermstore.Name, targetSite.Adapter.SharePointVersion.IsSharePointOnline);
								base.CreateAndStartLogItem(ref usage, true, Resources.AddingTaxonomyGroup, localSiteCollectionGroup.Name, defaultSiteCollectionTermStore.Name, targetTermstore.Name, ActionOperationStatus.Running);
								try
								{
									try
									{
										ISharePointWriter writer = targetSite.Adapter.Writer;
										Guid id = targetTermstore.Id;
										string outerXml = writer.AddTermGroup(id.ToString(), xmlDocument.OuterXml, false);
										if (!string.IsNullOrEmpty(outerXml))
										{
											xmlDocument1.LoadXml(outerXml);
											XmlNodeList elementsByTagName = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
											XmlNodeList xmlNodeLists = xmlDocument1.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
											XmlNode xmlNodes = xmlDocument1.SelectSingleNode(string.Format("{0}/{1}", TaxonomyReportingElements.Result.ToString(), TaxonomyReportingElements.Statistics.ToString()));
											TaxonomyStatistics taxonomyStatistic = new TaxonomyStatistics(xmlNodes);
											usage.LicenseDataUsed = taxonomyStatistic.Usage;
											this.globalStats.Add(taxonomyStatistic);
											int num = base.ReviseKnownErrors(elementsByTagName);
											if (num > 0)
											{
												outerXml = xmlDocument1.OuterXml;
											}
											if (elementsByTagName.Count > 0 && num != elementsByTagName.Count)
											{
												Exception exception = new Exception(Resources.ErrorsHaveOccured);
												exception.Data.Add("Results", outerXml);
												throw exception;
											}
											if (xmlNodeLists.Count > 0 || num > 0)
											{
												flag1 = true;
												usage.Status = ActionOperationStatus.Warning;
												usage.Information = string.Format(Resources.FS_WarningsHaveOccured, (num > 0 ? Resources.TermsExistWithDifferentGuid : string.Empty));
											}
											usage.Details = outerXml;
										}
										if (usage.Status == ActionOperationStatus.Running)
										{
											usage.Status = ActionOperationStatus.Completed;
										}
									}
									catch (Exception exception2)
									{
										Exception exception1 = exception2;
										flag = true;
										usage.Exception = exception1;
										if (exception1.Data.Contains("Results"))
										{
											LogItem logItem = usage;
											logItem.Details = string.Concat(logItem.Details, (string.IsNullOrEmpty(usage.Details) ? string.Empty : string.Concat(Environment.NewLine, Environment.NewLine)), exception1.Data["Results"].ToString());
										}
									}
								}
								finally
								{
									base.FireOperationFinished(usage);
								}
							}
							catch (Exception exception3)
							{
								noSourceTermstores.Exception = exception3;
							}
						}
						finally
						{
							base.AddTermStoreGlobalStatistics(empty, name, localSiteCollectionGroup.Name);
						}
					}
					else
					{
						return;
					}
				}
				catch (Exception exception4)
				{
					noSourceTermstores.Exception = exception4;
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
						noSourceTermstores.Information = string.Format(Resources.FS_ReviewResults, Environment.NewLine);
						if (string.IsNullOrEmpty(noSourceTermstores.Details))
						{
							noSourceTermstores.Details = Resources.PleaseReviewTheRelatedLogItemsForDetails;
						}
					}
				}
				base.FireOperationFinished(noSourceTermstores);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.CopyLocalSiteCollectionTaxonomy(source[0] as ITaxonomySiteConnection, target[0] as ITaxonomySiteConnection);
		}
	}
}