using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Administration.LinkManagement;
using Metalogix.SharePoint.Options.Administration.LinkManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Administration.LinkManagement
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.LinkCorrection.ico")]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.CMCFileShare | ProductFlags.CMCWebsite | ProductFlags.CMCBlogsAndWikis)]
	[MenuText("Correct Links...{2-Correct}")]
	[Name("Correct Links")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPNode), true)]
	[UsesStickySettings(true)]
	public class LinkCorrectionAction : SharePointAction<LinkCorrectionOptions>
	{
		private int m_iDuplicates;

		private int m_iErrors;

		private int m_iMappingErrors;

		private int m_iCorrectionErrors;

		private int m_iDocumentsUpdated;

		private int m_iLinksCorrected;

		private LogItem m_operation;

		private Dictionary<string, LogItem> m_itemOperations = new Dictionary<string, LogItem>();

		private List<ILinkCorrector> m_listLinkCorrectorInstances = new List<ILinkCorrector>();

		public LinkCorrectionAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (current is SPWeb || current is SPFolder)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				if ((int)LinkUtils.LinkCorrectors.Length == 0)
				{
					return false;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		private void CorrectNodeLinks(SPNode spNode, ILinkDictionary linkDictionary, bool bRecursive)
		{
			if (spNode == null)
			{
				return;
			}
			if (!spNode.ChildrenFetched)
			{
				spNode.FetchChildren();
			}
			SPFolder sPFolder = spNode as SPFolder;
			if (sPFolder != null)
			{
				sPFolder.FetchChildren();
				ListItem[] array = sPFolder.Items.ToArray();
				for (int i = 0; i < (int)array.Length; i++)
				{
					if (base.CheckForAbort())
					{
						return;
					}
					SPListItem sPListItem = array[i] as SPListItem;
					if (sPListItem != null)
					{
						if (sPListItem["CheckoutUser"] == null || sPListItem["CheckoutUser"].Length == 0)
						{
							foreach (ILinkCorrector mListLinkCorrectorInstance in this.m_listLinkCorrectorInstances)
							{
								if (!base.CheckForAbort())
								{
									LogItem logItem = null;
									try
									{
										if (mListLinkCorrectorInstance.SupportsComponent(sPListItem))
										{
											LinkCorrectorTypeWrapper linkCorrectorTypeWrapper = new LinkCorrectorTypeWrapper(mListLinkCorrectorInstance.GetType());
											logItem = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), sPListItem.DisplayUrl, string.Empty, string.Empty);
											LogItem logItem1 = logItem;
											string details = logItem1.Details;
											LogItem logItem2 = logItem;
											string str = string.Concat((this.ActionOptions.ReportOnly ? "Finding links" : "Correcting links"), " using ", linkCorrectorTypeWrapper.ToString(), Environment.NewLine);
											string str1 = str;
											logItem2.Information = str;
											logItem1.Details = string.Concat(details, str1);
											base.FireOperationUpdated(logItem);
											if (mListLinkCorrectorInstance.CorrectComponent(sPListItem, linkDictionary, this.ActionOptions.ReportOnly))
											{
												this.m_iDocumentsUpdated++;
											}
											logItem.Status = ActionOperationStatus.Completed;
											base.FireOperationFinished(logItem);
											this.m_itemOperations.Remove(logItem.ItemName);
										}
									}
									catch (Exception exception1)
									{
										Exception exception = exception1;
										this.m_iCorrectionErrors++;
										if (logItem == null)
										{
											logItem = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), sPListItem.DisplayUrl, string.Empty, string.Empty);
										}
										logItem.Status = ActionOperationStatus.Failed;
										LogItem logItem3 = logItem;
										string details1 = logItem3.Details;
										string str2 = string.Concat("Error: ", exception.Message, Environment.NewLine);
										string str3 = str2;
										logItem.Information = str2;
										logItem3.Details = string.Concat(details1, str3);
										LogItem logItem4 = logItem;
										string details2 = logItem4.Details;
										string[] newLine = new string[] { details2, "********** Extended Information **********", Environment.NewLine, exception.StackTrace, Environment.NewLine, "******************************************", Environment.NewLine };
										logItem4.Details = string.Concat(newLine);
									}
								}
								else
								{
									return;
								}
							}
							sPListItem.Dispose();
						}
						else
						{
							this.m_iCorrectionErrors++;
							LogItem logItem5 = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), sPListItem.DisplayUrl, string.Empty, string.Empty);
							LogItem logItem6 = logItem5;
							string details3 = logItem6.Details;
							string str4 = string.Concat("Error: ", sPListItem.Name, " cannot be corrected because it is currently checked out.", Environment.NewLine);
							string str5 = str4;
							logItem5.Information = str4;
							logItem6.Details = string.Concat(details3, str5);
							logItem5.Status = ActionOperationStatus.Failed;
							base.FireOperationFinished(logItem5);
							this.m_itemOperations.Remove(logItem5.ItemName);
						}
					}
				}
			}
			foreach (SPNode child in spNode.Children)
			{
				if (!base.CheckForAbort())
				{
					LogItem logItem7 = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), spNode.DisplayUrl, string.Empty, string.Empty);
					if (child is SPList || bRecursive)
					{
						try
						{
							this.CorrectNodeLinks(child, linkDictionary, bRecursive);
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							this.m_iCorrectionErrors++;
							logItem7.Status = ActionOperationStatus.Failed;
							LogItem logItem8 = logItem7;
							string details4 = logItem8.Details;
							string str6 = string.Concat("Error: ", exception2.Message, Environment.NewLine);
							string str7 = str6;
							logItem7.Information = str6;
							logItem8.Details = string.Concat(details4, str7);
							LogItem logItem9 = logItem7;
							string details5 = logItem9.Details;
							string[] strArrays = new string[] { details5, "********** Extended Information **********", Environment.NewLine, exception2.StackTrace, Environment.NewLine, "******************************************", Environment.NewLine };
							logItem9.Details = string.Concat(strArrays);
						}
					}
					child.Dispose();
				}
				else
				{
					return;
				}
			}
			spNode.ReleaseChildren();
			spNode.Dispose();
		}

		private LogItem FindOrAddLogItem(string sOperation, string sItemName, string sSource, string sTarget)
		{
			LogItem logItem = null;
			if (!this.m_itemOperations.TryGetValue(sItemName, out logItem))
			{
				logItem = new LogItem(sOperation, sItemName, sSource, sTarget, ActionOperationStatus.Running);
				this.m_itemOperations.Add(sItemName, logItem);
				base.FireOperationStarted(logItem);
			}
			return logItem;
		}

		private void MapListItems(ILinkDictionary linkDictionary, SPNode spNode)
		{
			if (linkDictionary == null || spNode == null)
			{
				return;
			}
			if (base.CheckForAbort())
			{
				return;
			}
			SPFolder sPFolder = spNode as SPFolder;
			if (sPFolder != null)
			{
				linkDictionary.MapContentSourceURLs(sPFolder);
				foreach (SPFolder subFolder in sPFolder.SubFolders)
				{
					this.MapListItems(linkDictionary, subFolder);
				}
				sPFolder.Dispose();
				return;
			}
			SPWeb sPWeb = spNode as SPWeb;
			if (sPWeb != null)
			{
				sPWeb.Lists.FetchData();
				foreach (SPList list in sPWeb.Lists)
				{
					this.MapListItems(linkDictionary, list);
					list.Dispose();
				}
				sPWeb.SubWebs.FetchData();
				foreach (SPWeb subWeb in sPWeb.SubWebs)
				{
					this.MapListItems(linkDictionary, subWeb);
					subWeb.Dispose();
				}
			}
		}

		private void On_DuplicateFound(object sender, LinkDuplicateEventArgs linkDuplicateArgs)
		{
			this.m_iDuplicates++;
			LogItem mOperation = this.m_operation;
			string details = mOperation.Details;
			LogItem logItem = this.m_operation;
			string[] sourceUri = new string[] { "Duplicate source URL found: \"", linkDuplicateArgs.SourceUri, "\" for items: \"", linkDuplicateArgs.LinkLoaded.URL, "\" and \"", linkDuplicateArgs.LinkDuplicate.URL, "\"", Environment.NewLine };
			string str = string.Concat(sourceUri);
			string str1 = str;
			logItem.Information = str;
			mOperation.Details = string.Concat(details, str1);
			base.FireOperationUpdated(this.m_operation);
		}

		private void On_ErrorEncountered(object sender, ErrorEncounteredEventArgs errorEncounteredArgs)
		{
			LogItem mOperation = null;
			if (!(sender is ILinkCorrector))
			{
				if (!(sender is ILinkDictionary))
				{
					this.m_iErrors++;
				}
				else
				{
					this.m_iMappingErrors++;
				}
				mOperation = this.m_operation;
			}
			else
			{
				this.m_iCorrectionErrors++;
				mOperation = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), errorEncounteredArgs.URL, string.Empty, string.Empty);
			}
			LogItem logItem = mOperation;
			string details = logItem.Details;
			string[] message = new string[] { "Error: ", errorEncounteredArgs.Message, " (", sender.GetType().Name, ")", Environment.NewLine };
			string str = string.Concat(message);
			string str1 = str;
			mOperation.Information = str;
			logItem.Details = string.Concat(details, str1);
		}

		private void On_LinkCorrected(object sender, LinkCorrectedEventArgs linkUpdateArgs)
		{
			this.m_iLinksCorrected++;
			LogItem logItem = this.FindOrAddLogItem(string.Concat((this.ActionOptions.ReportOnly ? "Reporting" : "Correcting"), " Item Links"), linkUpdateArgs.ContentItem.URL, string.Empty, string.Empty);
			LogItem logItem1 = logItem;
			string details = logItem1.Details;
			LogItem logItem2 = logItem;
			string[] oldLink = new string[] { (this.ActionOptions.ReportOnly ? "Found correctable link" : "Corrected link"), ": \"", linkUpdateArgs.OldLink, " to ", linkUpdateArgs.NewLink.URL, Environment.NewLine };
			string str = string.Concat(oldLink);
			string str1 = str;
			logItem2.Information = str;
			logItem1.Details = string.Concat(details, str1);
			base.FireOperationUpdated(logItem);
		}

		private void On_MappingItem(object sender, MappingItemEventArgs mappingItemArgs)
		{
			string empty = string.Empty;
			SPList tag = mappingItemArgs.Tag as SPList;
			if (tag != null)
			{
				empty = string.Concat(" (", tag.ItemCount, ")");
			}
			LogItem mOperation = this.m_operation;
			string details = mOperation.Details;
			LogItem logItem = this.m_operation;
			string[] itemUrl = new string[] { "Mapping: \"", mappingItemArgs.ItemUrl, "\"", empty, Environment.NewLine };
			string str = string.Concat(itemUrl);
			string str1 = str;
			logItem.Information = str;
			mOperation.Details = string.Concat(details, str1);
			base.FireOperationUpdated(this.m_operation);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			string str;
			this.m_iCorrectionErrors = 0;
			this.m_iDocumentsUpdated = 0;
			this.m_iDuplicates = 0;
			this.m_iErrors = 0;
			this.m_iLinksCorrected = 0;
			this.m_iMappingErrors = 0;
			SPNode item = null;
			for (int i = 0; i < target.Count; i++)
			{
				item = target[i] as SPNode;
				if (item != null)
				{
					break;
				}
			}
			this.m_operation = new LogItem("Link Correction", string.Empty, string.Empty, item.LinkableUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(this.m_operation);
			if (this.ActionOptions == null)
			{
				this.m_operation.Status = ActionOperationStatus.Failed;
				LogItem mOperation = this.m_operation;
				string details = mOperation.Details;
				string str1 = "Error: Unable to load Link Correction settings.";
				str = str1;
				this.m_operation.Information = str1;
				mOperation.Details = string.Concat(details, str);
				base.FireOperationFinished(this.m_operation);
				return;
			}
			string str2 = null;
			if (!this.ActionOptions.ReportOnly)
			{
				LogItem logItem = this.m_operation;
				string details1 = logItem.Details;
				LogItem mOperation1 = this.m_operation;
				string str3 = string.Concat("Correcting Links", Environment.NewLine);
				string str4 = str3;
				mOperation1.Information = str3;
				logItem.Details = string.Concat(details1, str4);
				str2 = "Correcting";
			}
			else
			{
				LogItem logItem1 = this.m_operation;
				string details2 = logItem1.Details;
				LogItem mOperation2 = this.m_operation;
				string str5 = string.Concat("Building Link Correction Report", Environment.NewLine);
				string str6 = str5;
				mOperation2.Information = str5;
				logItem1.Details = string.Concat(details2, str6);
				str2 = "Reporting";
			}
			base.FireOperationUpdated(this.m_operation);
			if (string.IsNullOrEmpty(this.ActionOptions.LookupProperty))
			{
				this.m_operation.Status = ActionOperationStatus.Failed;
				LogItem logItem2 = this.m_operation;
				string details3 = logItem2.Details;
				string str7 = "Error: Lookup property is null or empty.";
				string str8 = str7;
				this.m_operation.Information = str7;
				logItem2.Details = string.Concat(details3, str8);
				base.FireOperationFinished(this.m_operation);
				return;
			}
			if (this.ActionOptions.LocationLinkDictionary == null)
			{
				this.m_operation.Status = ActionOperationStatus.Failed;
				LogItem mOperation3 = this.m_operation;
				string details4 = mOperation3.Details;
				string str9 = "Error: Location Link Dictionary is null.";
				string str10 = str9;
				this.m_operation.Information = str9;
				mOperation3.Details = string.Concat(details4, str10);
				base.FireOperationFinished(this.m_operation);
				return;
			}
			LinkDictionary linkDictionary = new LinkDictionary(this.ActionOptions.LookupProperty)
			{
				IgnoreQueryString = this.ActionOptions.IgnoreQueryStrings
			};
			linkDictionary.MappingItem += new MappingItemEventHandler(this.On_MappingItem);
			linkDictionary.LinkDuplicateFound += new LinkDuplicateHandler(this.On_DuplicateFound);
			LogItem logItem3 = this.m_operation;
			string details5 = logItem3.Details;
			string newLine = Environment.NewLine;
			LogItem mOperation4 = this.m_operation;
			string str11 = string.Concat("Mapping Links", Environment.NewLine);
			string str12 = str11;
			mOperation4.Information = str11;
			logItem3.Details = string.Concat(details5, newLine, str12);
			base.FireOperationUpdated(this.m_operation);
			NodeCollection nodeCollection = new NodeCollection();
			if (!this.ActionOptions.UseActionTargetAsLinkDictionary)
			{
				foreach (object locationLinkDictionary in this.ActionOptions.LocationLinkDictionary)
				{
					nodeCollection.Add(locationLinkDictionary as Node);
				}
			}
			else
			{
				foreach (object obj in target)
				{
					nodeCollection.Add(obj as Node);
				}
			}
			foreach (object obj1 in nodeCollection)
			{
				if (!base.CheckForAbort())
				{
					SPNode sPNode = obj1 as SPNode;
					if (sPNode == null)
					{
						continue;
					}
					LogItem logItem4 = this.FindOrAddLogItem(string.Concat(str2, " Item Links"), sPNode.DisplayUrl, string.Empty, string.Empty);
					LogItem logItem5 = logItem4;
					string details6 = logItem5.Details;
					string str13 = string.Concat("Mapping Links", Environment.NewLine);
					string str14 = str13;
					logItem4.Information = str13;
					logItem5.Details = string.Concat(details6, str14);
					base.FireOperationUpdated(logItem4);
					this.MapListItems(linkDictionary, sPNode);
					sPNode.Dispose();
				}
				else
				{
					return;
				}
			}
			if (this.m_iDuplicates > 0 || this.m_iMappingErrors > 0)
			{
				if (this.m_iDuplicates > 0)
				{
					LogItem mOperation5 = this.m_operation;
					string details7 = mOperation5.Details;
					LogItem mOperation6 = this.m_operation;
					object[] mIDuplicates = new object[] { this.m_iDuplicates, " duplicate source ", null, null, null };
					mIDuplicates[2] = (this.m_iDuplicates == 1 ? "URL was" : "URLs were");
					mIDuplicates[3] = " found";
					mIDuplicates[4] = Environment.NewLine;
					string str15 = string.Concat(mIDuplicates);
					string str16 = str15;
					mOperation6.Information = str15;
					mOperation5.Details = string.Concat(details7, str16);
				}
				if (this.m_iMappingErrors > 0)
				{
					LogItem logItem6 = this.m_operation;
					string details8 = logItem6.Details;
					LogItem mOperation7 = this.m_operation;
					object[] mIMappingErrors = new object[] { "Mapping completed with ", this.m_iMappingErrors, null, null, null, null, null, null };
					mIMappingErrors[2] = (this.m_iMappingErrors == 1 ? " error" : " errors");
					mIMappingErrors[3] = " (";
					mIMappingErrors[4] = linkDictionary.KeyCount;
					mIMappingErrors[5] = (linkDictionary.KeyCount == 1 ? " source URL" : " source URLs");
					mIMappingErrors[6] = " mapped)";
					mIMappingErrors[7] = Environment.NewLine;
					string str17 = string.Concat(mIMappingErrors);
					string str18 = str17;
					mOperation7.Information = str17;
					logItem6.Details = string.Concat(details8, str18);
				}
				this.m_operation.Status = ActionOperationStatus.Warning;
			}
			else
			{
				LogItem logItem7 = this.m_operation;
				string details9 = logItem7.Details;
				LogItem mOperation8 = this.m_operation;
				object[] keyCount = new object[] { "Mapping completed successfully (", linkDictionary.KeyCount, null, null, null };
				keyCount[2] = (linkDictionary.KeyCount == 1 ? " source URL" : " source URLs");
				keyCount[3] = " mapped)";
				keyCount[4] = Environment.NewLine;
				string str19 = string.Concat(keyCount);
				string str20 = str19;
				mOperation8.Information = str19;
				logItem7.Details = string.Concat(details9, str20);
			}
			base.FireOperationUpdated(this.m_operation);
			LogItem logItem8 = this.m_operation;
			string details10 = logItem8.Details;
			string newLine1 = Environment.NewLine;
			LogItem mOperation9 = this.m_operation;
			string str21 = string.Concat("Initialising Link Correctors", Environment.NewLine);
			string str22 = str21;
			mOperation9.Information = str21;
			logItem8.Details = string.Concat(details10, newLine1, str22);
			base.FireOperationUpdated(this.m_operation);
			this.m_listLinkCorrectorInstances.Clear();
			foreach (Type selectedLinkCorrector in this.ActionOptions.SelectedLinkCorrectors)
			{
				LinkCorrectorTypeWrapper linkCorrectorTypeWrapper = new LinkCorrectorTypeWrapper(selectedLinkCorrector);
				LogItem logItem9 = this.m_operation;
				string details11 = logItem9.Details;
				LogItem mOperation10 = this.m_operation;
				string str23 = string.Concat("Initialising ", linkCorrectorTypeWrapper.ToString(), " link corrector", Environment.NewLine);
				string str24 = str23;
				mOperation10.Information = str23;
				logItem9.Details = string.Concat(details11, str24);
				try
				{
					object obj2 = Activator.CreateInstance(selectedLinkCorrector);
					if (obj2 == null)
					{
						this.m_operation.Status = ActionOperationStatus.Failed;
						throw new Exception(string.Concat("Error while creating instance of Link Corrector: ", selectedLinkCorrector.FullName));
					}
					ILinkCorrector linkCorrector = (ILinkCorrector)obj2;
					linkCorrector.LinkCorrected += new LinkCorrectedEventHandler(this.On_LinkCorrected);
					linkCorrector.ErrorEncountered += new ErrorEncounteredEventHandler(this.On_ErrorEncountered);
					this.m_listLinkCorrectorInstances.Add(linkCorrector);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem10 = this.m_operation;
					string details12 = logItem10.Details;
					LogItem mOperation11 = this.m_operation;
					string str25 = string.Concat("Error: ", exception.Message, Environment.NewLine);
					string str26 = str25;
					mOperation11.Information = str25;
					logItem10.Details = string.Concat(details12, str26);
					LogItem logItem11 = this.m_operation;
					string details13 = logItem11.Details;
					string[] strArrays = new string[] { details13, "********** Extended Information **********", Environment.NewLine, exception.StackTrace, Environment.NewLine, "******************************************", Environment.NewLine };
					logItem11.Details = string.Concat(strArrays);
				}
			}
			LogItem mOperation12 = this.m_operation;
			string details14 = mOperation12.Details;
			string newLine2 = Environment.NewLine;
			LogItem logItem12 = this.m_operation;
			string str27 = string.Concat((this.ActionOptions.ReportOnly ? "Finding Links" : "Correcting Links"), Environment.NewLine);
			string str28 = str27;
			logItem12.Information = str27;
			mOperation12.Details = string.Concat(details14, newLine2, str28);
			base.FireOperationUpdated(this.m_operation);
			foreach (object obj3 in target)
			{
				if (!base.CheckForAbort())
				{
					SPNode sPNode1 = obj3 as SPNode;
					if (sPNode1 == null)
					{
						continue;
					}
					LogItem logItem13 = this.FindOrAddLogItem(string.Concat(str2, " Item Links"), sPNode1.DisplayUrl, string.Empty, string.Empty);
					LogItem logItem14 = logItem13;
					string details15 = logItem14.Details;
					string str29 = string.Concat(logItem13.Operation, Environment.NewLine);
					string str30 = str29;
					logItem13.Information = str29;
					logItem14.Details = string.Concat(details15, str30);
					base.FireOperationUpdated(logItem13);
					this.CorrectNodeLinks(sPNode1, linkDictionary, this.ActionOptions.Recursive);
					sPNode1.Dispose();
				}
				else
				{
					return;
				}
			}
			foreach (ILinkCorrector mListLinkCorrectorInstance in this.m_listLinkCorrectorInstances)
			{
				if (!(mListLinkCorrectorInstance is IDisposable))
				{
					continue;
				}
				((IDisposable)mListLinkCorrectorInstance).Dispose();
			}
			this.m_listLinkCorrectorInstances.Clear();
			string str31 = (this.ActionOptions.ReportOnly ? "search" : "correction");
			string str32 = (this.ActionOptions.ReportOnly ? "found for correcting " : "corrected ");
			string str33 = (this.m_iLinksCorrected == 1 ? " link " : " links ");
			string str34 = (this.m_iDocumentsUpdated == 1 ? " document" : " documents");
			foreach (LogItem value in this.m_itemOperations.Values)
			{
				if (value.Status == ActionOperationStatus.Running)
				{
					value.Status = ActionOperationStatus.Completed;
				}
				base.FireOperationFinished(value);
			}
			this.m_itemOperations.Clear();
			if (this.m_iCorrectionErrors > 0 || this.m_iErrors > 0)
			{
				if (this.m_iCorrectionErrors > 0)
				{
					string str35 = (this.m_iCorrectionErrors == 1 ? " error " : " errors ");
					LogItem mOperation13 = this.m_operation;
					string details16 = mOperation13.Details;
					LogItem mOperation14 = this.m_operation;
					object[] mICorrectionErrors = new object[] { "Link ", str31, " completed with ", this.m_iCorrectionErrors, str35, this.m_iLinksCorrected, str33, str32, this.m_iDocumentsUpdated, str34, Environment.NewLine };
					string str36 = string.Concat(mICorrectionErrors);
					string str37 = str36;
					mOperation14.Information = str36;
					mOperation13.Details = string.Concat(details16, str37);
					this.m_operation.Status = ActionOperationStatus.Warning;
				}
				if (this.m_iErrors > 0)
				{
					string str38 = (this.m_iErrors == 1 ? " error " : " errors ");
					LogItem mOperation15 = this.m_operation;
					string details17 = mOperation15.Details;
					LogItem logItem15 = this.m_operation;
					object[] mIErrors = new object[] { "Link ", str31, " completed with ", this.m_iErrors, str38, this.m_iLinksCorrected, str33, str32, "in ", this.m_iDocumentsUpdated, str34, Environment.NewLine };
					string str39 = string.Concat(mIErrors);
					string str40 = str39;
					logItem15.Information = str39;
					mOperation15.Details = string.Concat(details17, str40);
					this.m_operation.Status = ActionOperationStatus.Warning;
				}
			}
			else
			{
				LogItem mOperation16 = this.m_operation;
				string details18 = mOperation16.Details;
				LogItem logItem16 = this.m_operation;
				object[] mILinksCorrected = new object[] { "Link ", str31, " completed successfully (", this.m_iLinksCorrected, str33, str32, this.m_iDocumentsUpdated, str34, ")", Environment.NewLine };
				string str41 = string.Concat(mILinksCorrected);
				str = str41;
				logItem16.Information = str41;
				mOperation16.Details = string.Concat(details18, str);
			}
			if (this.m_operation.Status == ActionOperationStatus.Running)
			{
				this.m_operation.Status = ActionOperationStatus.Completed;
			}
			base.FireOperationFinished(this.m_operation);
		}
	}
}