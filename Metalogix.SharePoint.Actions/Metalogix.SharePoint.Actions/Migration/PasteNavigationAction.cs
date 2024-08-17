using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointNavigation", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Navigation.ico")]
	[MenuText("3:Paste Site Objects {0-Paste} > Navigation...")]
	[Name("Paste Site Navigation")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class PasteNavigationAction : PasteAction<PasteNavigationOptions>
	{
		private const string GUIDPattern = "[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}";

		private readonly static string[] GLOBAL_NAV_SPECIFIC_PROPERTIES;

		private readonly static string[] CURRENT_NAV_SPECIFIC_PROPERTIES;

		private static Dictionary<int, ListTemplateType> s_systemHeadingIDsAndAssociatedListTemplate;

		private bool _isSourceCSOMOrNWS;

		private bool _isTargetCSOM;

		private Dictionary<string, KeyValuePair<SPWeb, SPWeb>> m_renamedSiteMap;

		private Dictionary<string, KeyValuePair<SPList, SPList>> m_renamedListMap;

		private bool m_linkCorrectorFullyInitialized;

		protected bool LinkCorrectorFullyInitialized
		{
			get
			{
				return this.m_linkCorrectorFullyInitialized;
			}
			set
			{
				this.m_linkCorrectorFullyInitialized = value;
			}
		}

		protected Dictionary<string, KeyValuePair<SPList, SPList>> RenamedListMap
		{
			get
			{
				return this.m_renamedListMap;
			}
		}

		protected Dictionary<string, KeyValuePair<SPWeb, SPWeb>> RenamedSiteMap
		{
			get
			{
				return this.m_renamedSiteMap;
			}
		}

		protected static Dictionary<int, ListTemplateType> SystemHeadingIDsAndAssociatedListTemplates
		{
			get
			{
				if (PasteNavigationAction.s_systemHeadingIDsAndAssociatedListTemplate == null)
				{
					PasteNavigationAction.s_systemHeadingIDsAndAssociatedListTemplate = new Dictionary<int, ListTemplateType>(5)
					{
						{ 1007, ListTemplateType.Survey },
						{ 1005, ListTemplateType.PictureLibrary },
						{ 1004, ListTemplateType.DocumentLibrary },
						{ 1003, ListTemplateType.CustomList },
						{ 1006, ListTemplateType.DiscussionBoard }
					};
				}
				return PasteNavigationAction.s_systemHeadingIDsAndAssociatedListTemplate;
			}
		}

		static PasteNavigationAction()
		{
			PasteNavigationAction.GLOBAL_NAV_SPECIFIC_PROPERTIES = new string[] { "InheritGlobalNavigation" };
			string[] strArrays = new string[] { "InheritCurrentNavigation", "NavigationShowSiblings", "QuickLaunchEnabled", "TreeViewEnabled" };
			PasteNavigationAction.CURRENT_NAV_SPECIFIC_PROPERTIES = strArrays;
			PasteNavigationAction.s_systemHeadingIDsAndAssociatedListTemplate = null;
		}

		public PasteNavigationAction()
		{
			this.m_renamedListMap = new Dictionary<string, KeyValuePair<SPList, SPList>>();
			this.m_renamedSiteMap = new Dictionary<string, KeyValuePair<SPWeb, SPWeb>>();
		}

		private void AddListAndViewMappingsToLinkCorrection(SPWeb sourceWeb, SPWeb targetWeb)
		{
			bool flag;
			foreach (SPList list in sourceWeb.Lists)
			{
				try
				{
					using (SPList targetList = this.GetTargetList(list, targetWeb, out flag))
					{
						if (targetList != null)
						{
							if (flag)
							{
								this.AddRenamedListMapping(list, targetList);
								StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, list.DisplayUrl);
								StandardizedUrl standardizedUrl1 = StandardizedUrl.StandardizeUrl(targetWeb.Adapter, targetList.DisplayUrl);
								if (!string.IsNullOrEmpty(standardizedUrl.Full) && !string.IsNullOrEmpty(standardizedUrl1.Full))
								{
									base.LinkCorrector.RemoveMapping(standardizedUrl.Full);
									base.LinkCorrector.AddMapping(standardizedUrl.Full, standardizedUrl1.Full);
								}
								if (!string.IsNullOrEmpty(standardizedUrl.ServerRelative) && !string.IsNullOrEmpty(standardizedUrl1.ServerRelative))
								{
									base.LinkCorrector.RemoveMapping(standardizedUrl.ServerRelative);
									base.LinkCorrector.AddMapping(standardizedUrl.ServerRelative, standardizedUrl1.ServerRelative);
								}
							}
							foreach (SPView view in list.Views)
							{
								if (view.IsWebPartView)
								{
									continue;
								}
								bool isDefaultView = view.IsDefaultView;
								SPView sPView = null;
								IEnumerator enumerator = targetList.Views.GetEnumerator();
								try
								{
									do
									{
									Label0:
										if (!enumerator.MoveNext())
										{
											break;
										}
										SPView current = (SPView)enumerator.Current;
										if (!current.IsWebPartView)
										{
											if (view.Name.Equals(current.Name, StringComparison.OrdinalIgnoreCase) || view.DisplayName.Equals(current.DisplayName, StringComparison.OrdinalIgnoreCase))
											{
												sPView = current;
												if (!isDefaultView)
												{
													break;
												}
											}
											if (current.IsDefaultView)
											{
												sPView = current;
											}
											else
											{
												goto Label0;
											}
										}
										else
										{
											goto Label0;
										}
									}
									while (!isDefaultView);
								}
								finally
								{
									IDisposable disposable = enumerator as IDisposable;
									if (disposable != null)
									{
										disposable.Dispose();
									}
								}
								if (sPView == null)
								{
									continue;
								}
								StandardizedUrl standardizedUrl2 = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, view.Url);
								StandardizedUrl standardizedUrl3 = StandardizedUrl.StandardizeUrl(targetWeb.Adapter, sPView.Url);
								if (!string.IsNullOrEmpty(standardizedUrl2.Full) && !string.IsNullOrEmpty(standardizedUrl3.Full))
								{
									base.LinkCorrector.RemoveMapping(standardizedUrl2.Full);
									base.LinkCorrector.AddMapping(standardizedUrl2.Full, standardizedUrl3.Full);
								}
								if (string.IsNullOrEmpty(standardizedUrl2.ServerRelative) || string.IsNullOrEmpty(standardizedUrl3.ServerRelative))
								{
									continue;
								}
								base.LinkCorrector.RemoveMapping(standardizedUrl2.ServerRelative);
								base.LinkCorrector.AddMapping(standardizedUrl2.ServerRelative, standardizedUrl3.ServerRelative);
							}
						}
						else
						{
							continue;
						}
					}
				}
				finally
				{
					list.Dispose();
				}
			}
		}

		private void AddRenamedListMapping(SPList sourceList, SPList targetList)
		{
			StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(targetList.Adapter, targetList.ServerRelativeUrl);
			if (this.RenamedListMap.ContainsKey(standardizedUrl.ServerRelative))
			{
				this.RenamedListMap.Remove(standardizedUrl.ServerRelative);
			}
			this.RenamedListMap.Add(standardizedUrl.ServerRelative, new KeyValuePair<SPList, SPList>(sourceList, targetList));
			foreach (SPView view in targetList.Views)
			{
				if (!view.IsDefaultView)
				{
					continue;
				}
				StandardizedUrl standardizedUrl1 = StandardizedUrl.StandardizeUrl(targetList.Adapter, view.Url);
				if (this.RenamedListMap.ContainsKey(standardizedUrl1.ServerRelative))
				{
					this.RenamedListMap.Remove(standardizedUrl1.ServerRelative);
				}
				this.RenamedListMap.Add(standardizedUrl1.ServerRelative, new KeyValuePair<SPList, SPList>(sourceList, targetList));
				break;
			}
		}

		private void AddRenamedSiteMapping(SPWeb sourceWeb, SPWeb targetWeb)
		{
			StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(targetWeb.Adapter, targetWeb.Adapter.ServerRelativeUrl);
			if (this.RenamedSiteMap.ContainsKey(standardizedUrl.ServerRelative))
			{
				this.RenamedSiteMap.Remove(standardizedUrl.ServerRelative);
			}
			this.RenamedSiteMap.Add(standardizedUrl.ServerRelative, new KeyValuePair<SPWeb, SPWeb>(sourceWeb, targetWeb));
		}

		private void Clean2003Source(SPNavigationRoot sourceNode)
		{
			SPNavigationNode nodeByID = sourceNode.GetNodeByID(SPNavigationNode.HOME_NODE_ID);
			SPNavigationNode sPNavigationNode = sourceNode.GetNodeByID(SPNavigationNode.TOP_NAV_BAR_NODE_ID);
			if (nodeByID != null)
			{
				nodeByID.Delete();
			}
			if (sPNavigationNode != null)
			{
				sPNavigationNode.Delete();
			}
		}

		private void CleanTargetForFreshCopy(SPNavigationNode sourceNode, SPNavigationNode targetNode, bool b2003SourceMode)
		{
			List<SPNavigationNode> sPNavigationNodes = new List<SPNavigationNode>();
			foreach (SPNavigationNode child in targetNode.Children)
			{
				int d = child.ID;
				if (!base.SharePointOptions.CopyCurrentNavigation && d == SPNavigationNode.QUICK_LAUNCH_NODE_ID || (!base.SharePointOptions.CopyGlobalNavigation || b2003SourceMode) && d == SPNavigationNode.TOP_NAV_BAR_NODE_ID || !b2003SourceMode && child.IsHomeNode || !b2003SourceMode && child.IsSubSiteOrPageNode)
				{
					continue;
				}
				if (child.IsSystemNode)
				{
					SPNavigationNode sPNavigationNode = null;
					foreach (SPNavigationNode child1 in sourceNode.Children)
					{
						if (child1.ID != d)
						{
							continue;
						}
						sPNavigationNode = child1;
						break;
					}
					if (sPNavigationNode != null)
					{
						this.CleanTargetForFreshCopy(sPNavigationNode, child, b2003SourceMode);
					}
					else
					{
						sPNavigationNodes.Add(child);
					}
				}
				else
				{
					sPNavigationNodes.Add(child);
				}
			}
			foreach (SPNavigationNode sPNavigationNode1 in sPNavigationNodes)
			{
				sPNavigationNode1.Delete();
			}
		}

		private void CopyNavigation(Dictionary<SPWeb, SPWeb> websToCopy, bool bSyncSettings)
		{
			if (websToCopy == null)
			{
				return;
			}
			foreach (KeyValuePair<SPWeb, SPWeb> keyValuePair in websToCopy)
			{
				if (keyValuePair.Key.Title == keyValuePair.Value.Title)
				{
					continue;
				}
				this.AddRenamedSiteMapping(keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<SPWeb, SPWeb> keyValuePair1 in websToCopy)
			{
				if (!base.CheckForAbort())
				{
					this.CopyNavigation(keyValuePair1.Key, keyValuePair1.Value, bSyncSettings);
				}
				else
				{
					return;
				}
			}
		}

		private void CopyNavigation(SPWeb sourceWeb, SPWeb targetWeb, bool bSyncSettings)
		{
			bool flag;
			try
			{
				if (base.SharePointOptions.CopyGlobalNavigation || base.SharePointOptions.CopyCurrentNavigation)
				{
					this._isSourceCSOMOrNWS = (sourceWeb.Adapter.IsClientOM ? true : sourceWeb.Adapter.IsNws);
					this._isTargetCSOM = targetWeb.Adapter.IsClientOM;
					if (!this._isSourceCSOMOrNWS && this._isTargetCSOM)
					{
						base.SharePointOptions.CopyGlobalNavigation = true;
						base.SharePointOptions.CopyCurrentNavigation = true;
					}
					if (!targetWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater || !(targetWeb.Template.Name == "SRCHCENTERLITE#1"))
					{
						this.CopyNavigationAtCurrentLevel(sourceWeb, targetWeb, bSyncSettings);
						if (base.SharePointOptions.Recursive)
						{
							foreach (SPWeb subWeb in sourceWeb.SubWebs)
							{
								try
								{
									using (SPWeb sPWeb = this.GetTargetWeb(subWeb, targetWeb, out flag))
									{
										if (sPWeb != null)
										{
											this.CopyNavigation(subWeb, sPWeb, bSyncSettings);
										}
									}
								}
								finally
								{
									subWeb.Dispose();
								}
								if (!base.CheckForAbort())
								{
									continue;
								}
								return;
							}
						}
					}
					else
					{
						LogItem logItem = new LogItem("Navigation setting warning", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Warning)
						{
							Operation = "Skipping navigation settings",
							Information = string.Format("Skipped Navigation settings for '{0}' as they not supported for the Basic Search site template.", targetWeb.Url),
							Status = ActionOperationStatus.Skipped
						};
						base.FireOperationStarted(logItem);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem1 = new LogItem(string.Concat("Failed to copy navigation for site: ", sourceWeb.DisplayUrl), sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Failed);
				base.FireOperationStarted(logItem1);
				logItem1.Exception = exception;
			}
		}

		private void CopyNavigationAtCurrentLevel(SPWeb sourceWeb, SPWeb targetWeb, bool bSyncSettings)
		{
			if (bSyncSettings)
			{
				LogItem logItem = new LogItem("Fetching source web navigation settings", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					try
					{
						string webNavigationSettings = sourceWeb.Adapter.Reader.GetWebNavigationSettings();
						logItem.Operation = "Copying navigation settings to target";
						base.FireOperationUpdated(logItem);
						if (!base.SharePointOptions.CopyCurrentNavigation || !base.SharePointOptions.CopyGlobalNavigation)
						{
							XmlNode xmlNode = XmlUtility.StringToXmlNode(webNavigationSettings);
							if (!base.SharePointOptions.CopyCurrentNavigation)
							{
								string[] cURRENTNAVSPECIFICPROPERTIES = PasteNavigationAction.CURRENT_NAV_SPECIFIC_PROPERTIES;
								for (int i = 0; i < (int)cURRENTNAVSPECIFICPROPERTIES.Length; i++)
								{
									string str = cURRENTNAVSPECIFICPROPERTIES[i];
									XmlAttribute itemOf = xmlNode.Attributes[str];
									if (itemOf != null)
									{
										xmlNode.Attributes.Remove(itemOf);
									}
								}
							}
							if (!base.SharePointOptions.CopyGlobalNavigation)
							{
								string[] gLOBALNAVSPECIFICPROPERTIES = PasteNavigationAction.GLOBAL_NAV_SPECIFIC_PROPERTIES;
								for (int j = 0; j < (int)gLOBALNAVSPECIFICPROPERTIES.Length; j++)
								{
									string str1 = gLOBALNAVSPECIFICPROPERTIES[j];
									XmlAttribute xmlAttribute = xmlNode.Attributes[str1];
									if (xmlAttribute != null)
									{
										xmlNode.Attributes.Remove(xmlAttribute);
									}
								}
							}
							webNavigationSettings = xmlNode.OuterXml;
						}
						targetWeb.Adapter.Writer.ModifyWebNavigationSettings(webNavigationSettings, new ModifyNavigationOptions());
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						return;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
			}
			if (!base.CheckForAbort())
			{
				LogItem logItem1 = new LogItem("Preparing target site for navigation structure copy", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem1);
				try
				{
					try
					{
						targetWeb.EnsureLatestNavigationData();
						if (!base.CheckForAbort())
						{
							if (base.SharePointOptions.CopyCurrentNavigation)
							{
								this.SyncListQuickLaunchSettings(sourceWeb, targetWeb);
								if (base.CheckForAbort())
								{
									return;
								}
							}
							if (!this.LinkCorrectorFullyInitialized)
							{
								this.AddListAndViewMappingsToLinkCorrection(sourceWeb, targetWeb);
							}
							if (!base.CheckForAbort())
							{
								logItem1.Status = ActionOperationStatus.Completed;
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
					catch (Exception exception1)
					{
						logItem1.Exception = exception1;
						return;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem1);
				}
				if (base.CheckForAbort())
				{
					return;
				}
				StringBuilder stringBuilder = new StringBuilder();
				LogItem logItem2 = new LogItem("Copying navigation structure", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem2);
				try
				{
					try
					{
						if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2003)
						{
							this.Clean2003Source(sourceWeb.Navigation);
						}
						if (!sourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater)
						{
							this.CleanTargetForFreshCopy(sourceWeb.Navigation.QuickLaunchNode, targetWeb.Navigation.QuickLaunchNode, true);
						}
						else
						{
							this.CleanTargetForFreshCopy(sourceWeb.Navigation, targetWeb.Navigation, false);
						}
						if (!sourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater)
						{
							this.CopySourceNodesToTarget(sourceWeb.Navigation.QuickLaunchNode, targetWeb.Navigation.QuickLaunchNode, sourceWeb, targetWeb, true);
						}
						else
						{
							this.CopySourceNodesToTarget(sourceWeb.Navigation, targetWeb.Navigation, sourceWeb, targetWeb, false);
						}
						OperationReportingResult operationReportingResult = targetWeb.Navigation.CommitChanges(stringBuilder, this._isSourceCSOMOrNWS, this._isTargetCSOM, targetWeb.PublishingInfrastructureActive);
						logItem2.Details = (operationReportingResult.WarningOccured ? string.Concat(stringBuilder.ToString(), operationReportingResult.AllReportElementsAsString) : operationReportingResult.AllReportElementsAsString);
						logItem2.Status = (operationReportingResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
					}
					catch (Exception exception2)
					{
						logItem2.Exception = exception2;
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(logItem2.Details);
						logItem2.Details = stringBuilder.ToString();
					}
				}
				finally
				{
					base.FireOperationFinished(logItem2);
				}
				return;
			}
		}

		private void CopySourceNodesToTarget(SPNavigationNode sourceNode, SPNavigationNode targetNode, SPWeb sourceWeb, SPWeb targetWeb, bool b2003SourceMode)
		{
			bool flag = (b2003SourceMode ? false : sourceNode.ID == SPNavigationNode.TOP_NAV_BAR_NODE_ID);
			bool flag1 = (b2003SourceMode ? false : targetNode.ID == SPNavigationNode.TOP_NAV_BAR_NODE_ID);
			bool flag2 = (!flag || sourceNode.Children.Count <= 0 ? false : sourceNode.Children[0].IsHomeNode);
			bool flag3 = (!flag1 || targetNode.Children.Count <= 0 ? false : targetNode.Children[0].IsHomeNode);
			foreach (SPNavigationNode child in sourceNode.Children)
			{
				int d = child.ID;
				if (!base.SharePointOptions.CopyCurrentNavigation && d == SPNavigationNode.QUICK_LAUNCH_NODE_ID || (!base.SharePointOptions.CopyGlobalNavigation || b2003SourceMode) && d == SPNavigationNode.TOP_NAV_BAR_NODE_ID || d == SPNavigationNode.HOME_NODE_ID || !b2003SourceMode && child.IsHomeNode)
				{
					continue;
				}
				SPNavigationNode title = null;
				if (child.IsSystemNode)
				{
					if (this._isSourceCSOMOrNWS || !this._isTargetCSOM || d != 1031 && d != 1034)
					{
						foreach (SPNavigationNode sPNavigationNode in targetNode.Children)
						{
							if (sPNavigationNode.ID < 0 || sPNavigationNode.ID != d)
							{
								continue;
							}
							title = sPNavigationNode;
							break;
						}
					}
				}
				else if (child.IsSubSiteOrPageNode)
				{
					string areaOrPageNodeUrlTail = this.GetAreaOrPageNodeUrlTail(child.Url);
					foreach (SPNavigationNode child1 in targetNode.Children)
					{
						if (child1.ID < 0 || !child1.IsSubSiteOrPageNode || !(this.GetAreaOrPageNodeUrlTail(child1.Url) == areaOrPageNodeUrlTail))
						{
							continue;
						}
						title = child1;
						break;
					}
				}
				if (title == null)
				{
					int orderIndex = child.OrderIndex;
					if (!b2003SourceMode && flag && flag1)
					{
						if (flag2 && !flag3)
						{
							orderIndex--;
							if (orderIndex < 0)
							{
								orderIndex = 0;
							}
						}
						else if (!flag2 && flag3)
						{
							orderIndex++;
						}
					}
					title = (orderIndex >= targetNode.Children.Count ? targetNode.Children.Add(child) : targetNode.Children.Insert(child, orderIndex));
					this.CorrectNodeAndSubNodeUrls(title, sourceWeb, targetWeb);
					this.UpdateNodeTitle(title, targetWeb);
					if (b2003SourceMode && child.ID < 2000 && title.Url.Contains("sid:"))
					{
						title.Url = "";
					}
					if (!title.Properties.ContainsKey("Audience"))
					{
						continue;
					}
					string item = title.Properties["Audience"];
					string str = "";
					int num = item.IndexOf('|');
					if (num >= 0)
					{
						str = item.Substring(0, num + 1);
						item = (num == item.Length - 1 ? "" : item.Substring(num + 1));
					}
					item = string.Concat(str, base.MapAudienceString(item, sourceWeb, targetWeb));
				}
				else
				{
					if (d != SPNavigationNode.QUICK_LAUNCH_NODE_ID && d != SPNavigationNode.TOP_NAV_BAR_NODE_ID)
					{
						title.Title = child.Title;
						this.UpdateNodeTitle(title, targetWeb);
						title.OrderIndex = child.OrderIndex;
						if (title.IsSubSiteOrPageNode)
						{
							title.IsVisible = child.IsVisible;
						}
					}
					string target = child.Target;
					if (!target.Equals(string.Empty))
					{
						title.Target = target;
					}
					this.CopySourceNodesToTarget(child, title, sourceWeb, targetWeb, b2003SourceMode);
				}
			}
		}

		private void CorrectNodeAndSubNodeUrls(SPNavigationNode node, SPWeb sourceWeb, SPWeb targetWeb)
		{
			node.Url = base.LinkCorrector.CorrectUrl(node.Url, sourceWeb, targetWeb);
			try
			{
				if ((node.Url.IndexOf("WopiFrame.aspx", StringComparison.OrdinalIgnoreCase) > -1 ? true : node.Url.IndexOf("WopiFrame2.aspx", StringComparison.OrdinalIgnoreCase) > -1))
				{
					string updatedOneNoteFolderName = SPUtils.GetUpdatedOneNoteFolderName(targetWeb);
					node.Url = this.CorrectNotebookUrl(node.Url, sourceWeb.ServerRelativeUrl, targetWeb.ServerRelativeUrl, updatedOneNoteFolderName);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Format("An error occured while link correcting url '{0}'. Error '{1}'", node.Url, exception));
			}
			foreach (SPNavigationNode child in node.Children)
			{
				this.CorrectNodeAndSubNodeUrls(child, sourceWeb, targetWeb);
			}
		}

		private string CorrectNotebookUrl(string nodeUrl, string sourceRelativeUrl, string targetRelativeUrl, string targetNoteName)
		{
			string str = nodeUrl;
			string queryStringNodeValue = SPUtils.GetQueryStringNodeValue(str, "sourcedoc");
			if (string.IsNullOrEmpty(queryStringNodeValue))
			{
				return str;
			}
			if (queryStringNodeValue.IndexOf(sourceRelativeUrl, StringComparison.InvariantCultureIgnoreCase) <= -1)
			{
				Match match = Regex.Match(str, "[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}", RegexOptions.IgnoreCase);
				if (!string.IsNullOrEmpty(match.Value) && base.GuidMappings.ContainsKey(new Guid(match.Value)))
				{
					str = Regex.Replace(str, match.Value, Convert.ToString(base.GuidMappings[new Guid(match.Value)]), RegexOptions.IgnoreCase);
				}
			}
			else
			{
				str = this.CorrectNotebookUrlByPath(str, sourceRelativeUrl, targetRelativeUrl, targetNoteName, queryStringNodeValue);
			}
			return str;
		}

		private string CorrectNotebookUrlByPath(string nodeUrl, string sourceRelativeUrl, string targetRelativeUrl, string targetNoteName, string sourceNotePath)
		{
			string str = null;
			string str1 = nodeUrl;
			if (!string.IsNullOrEmpty(sourceRelativeUrl))
			{
				if (!string.Equals(sourceRelativeUrl, "/") && !string.Equals(targetRelativeUrl, "/"))
				{
					str = Regex.Replace(sourceNotePath, string.Concat(sourceRelativeUrl, "/"), string.Concat(targetRelativeUrl, "/"), RegexOptions.IgnoreCase);
				}
				else if (string.Equals(sourceRelativeUrl, "/") && !string.Equals(targetRelativeUrl, "/"))
				{
					str = string.Concat(targetRelativeUrl, sourceNotePath);
				}
				else if (!string.Equals(sourceRelativeUrl, "/") && string.Equals(targetRelativeUrl, "/"))
				{
					str = Regex.Replace(sourceNotePath, string.Concat(sourceRelativeUrl, "/"), targetRelativeUrl, RegexOptions.IgnoreCase);
				}
			}
			char[] chrArray = new char[] { '/' };
			string str2 = sourceNotePath.Split(chrArray).Last<string>();
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str2))
			{
				str = Regex.Replace(str, str2, targetNoteName, RegexOptions.IgnoreCase);
			}
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(sourceNotePath))
			{
				str1 = Regex.Replace(str1, sourceNotePath, str, RegexOptions.IgnoreCase);
			}
			return str1;
		}

		private void CreateAndDestroyTemporaryList(SPWeb targetWeb, SPListTemplate template)
		{
			Guid guid = Guid.NewGuid();
			string str = string.Concat("MLTEMP_", guid.ToString("N"));
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("List");
			xmlTextWriter.WriteAttributeString("Name", str);
			xmlTextWriter.WriteAttributeString("Title", str);
			xmlTextWriter.WriteAttributeString("Description", "");
			xmlTextWriter.WriteAttributeString("BaseTemplate", template.TemplateType.ToString());
			xmlTextWriter.WriteAttributeString("FeatureId", template.FeatureId);
			xmlTextWriter.WriteAttributeString("OnQuickLaunch", "true");
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			string str1 = stringBuilder.ToString();
			targetWeb.Lists.AddList(str1, new AddListOptions()).Delete();
		}

		private string GetAreaOrPageNodeUrlTail(string sUrl)
		{
			string str = sUrl;
			int num = str.LastIndexOf('/');
			if (num >= 0)
			{
				str = str.Substring(num);
			}
			return HttpUtility.UrlPathEncode(str.ToLower()).Trim(new char[] { '/' });
		}

		private void GetExistingSystemHeadingIDs(SPNavigationNode node, ref List<int> systemHeadings)
		{
			if (PasteNavigationAction.SystemHeadingIDsAndAssociatedListTemplates.ContainsKey(node.ID))
			{
				systemHeadings.Add(node.ID);
			}
			foreach (SPNavigationNode child in node.Children)
			{
				this.GetExistingSystemHeadingIDs(child, ref systemHeadings);
			}
		}

		private int[] GetMissingSystemHeadings(SPNavigationRoot sourceNavigation, SPNavigationRoot targetNavigation)
		{
			List<int> nums = new List<int>();
			this.GetExistingSystemHeadingIDs(sourceNavigation, ref nums);
			List<int> nums1 = new List<int>();
			this.GetExistingSystemHeadingIDs(targetNavigation, ref nums1);
			return (
				from id in nums
				where !nums1.Contains(id)
				select id).ToArray<int>();
		}

		private bool GetSystemNodesAreSynched(SPNavigationNode sourceNode, SPWeb targetWeb)
		{
			bool flag;
			if (sourceNode.IsSystemNode && targetWeb.Navigation.GetNodeByID(sourceNode.ID) == null)
			{
				return false;
			}
			using (IEnumerator<SPNavigationNode> enumerator = sourceNode.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (this.GetSystemNodesAreSynched(enumerator.Current, targetWeb))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			return flag;
		}

		private SPList GetTargetList(SPList sourceList, SPWeb targetWeb, out bool bListWasRenamed)
		{
			SPList matchingList = MigrationUtils.GetMatchingList(sourceList, targetWeb, base.SharePointOptions.TaskCollection);
			bListWasRenamed = (matchingList == null ? false : !sourceList.Name.Equals(matchingList.Name, StringComparison.OrdinalIgnoreCase));
			return matchingList;
		}

		private SPWeb GetTargetWeb(SPWeb sourceWeb, SPWeb targetParentWeb, out bool bSiteWasRenamed)
		{
			SPWeb sPWeb;
			bSiteWasRenamed = false;
			string name = sourceWeb.Name;
			foreach (TransformationTask taskCollection in base.SharePointOptions.TaskCollection)
			{
				if (!taskCollection.ApplyTo.Evaluate(sourceWeb, new CompareDatesInUtc()) || !taskCollection.ChangeOperations.ContainsKey("Name"))
				{
					continue;
				}
				name = taskCollection.ChangeOperations["Name"];
				bSiteWasRenamed = true;
			}
			using (IEnumerator<Node> enumerator = targetParentWeb.SubWebs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SPWeb current = (SPWeb)enumerator.Current;
					if (current.Name != name)
					{
						continue;
					}
					sPWeb = current;
					return sPWeb;
				}
				return null;
			}
			return sPWeb;
		}

		private void InitializeLinkCorrector(SPWeb sourceWeb, SPWeb targetWeb)
		{
			LogItem logItem = new LogItem("Initializing Link Corrector", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					if (sourceWeb.Title != targetWeb.Title)
					{
						this.AddRenamedSiteMapping(sourceWeb, targetWeb);
					}
					base.LinkCorrector.ClearMappings();
					this.UpdateLinkCorrection(sourceWeb, targetWeb, base.SharePointOptions.UsingComprehensiveLinkCorrection);
					this.LinkCorrectorFullyInitialized = base.SharePointOptions.UsingComprehensiveLinkCorrection;
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, base.SharePointOptions.ForceRefresh);
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					try
					{
						this.RenamedSiteMap.Clear();
						this.RenamedListMap.Clear();
						this.InitializeLinkCorrector(item, sPWeb);
						base.InitializeAudienceMappings(item, sPWeb);
						this.CopyNavigation(item, sPWeb, true);
					}
					finally
					{
						sPWeb.Dispose();
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			this.CopyNavigation(oParams[0] as Dictionary<SPWeb, SPWeb>, (bool)oParams[1]);
		}

		private void SyncListQuickLaunchSettings(SPWeb sourceWeb, SPWeb targetWeb)
		{
			int[] missingSystemHeadings = this.GetMissingSystemHeadings(sourceWeb.Navigation, targetWeb.Navigation);
			if ((int)missingSystemHeadings.Length == 0)
			{
				return;
			}
			bool flag = false;
			int[] numArray = missingSystemHeadings;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				int num = numArray[i];
				ListTemplateType item = PasteNavigationAction.SystemHeadingIDsAndAssociatedListTemplates[num];
				SPListTemplate byType = targetWeb.ListTemplates.GetByType(item);
				if (byType != null)
				{
					bool flag1 = false;
					foreach (SPList list in targetWeb.Lists)
					{
						if (list.BaseTemplate != item || list.OnQuickLaunch)
						{
							continue;
						}
						flag1 = true;
						XmlNode xmlNode = XmlUtility.StringToXmlNode(list.XML);
						XmlAttribute itemOf = xmlNode.Attributes["OnQuickLaunch"];
						if (itemOf == null)
						{
							itemOf = xmlNode.OwnerDocument.CreateAttribute("OnQuickLaunch");
							xmlNode.Attributes.Append(itemOf);
						}
						itemOf.Value = "True";
						list.UpdateSettings(xmlNode.OuterXml);
						flag = true;
						break;
					}
					if (!flag1)
					{
						this.CreateAndDestroyTemporaryList(targetWeb, byType);
						flag = true;
					}
				}
			}
			if (flag)
			{
				targetWeb.EnsureLatestNavigationData();
			}
		}

		private void UpdateLinkCorrection(SPWeb sourceWeb, SPWeb targetWeb, bool bRecursive)
		{
			bool flag;
			if (!string.IsNullOrEmpty(sourceWeb.Adapter.ServerUrl))
			{
				string str = sourceWeb.Adapter.ServerUrl.TrimEnd(new char[] { '/' });
				string serverRelativeUrl = sourceWeb.Adapter.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				string str1 = string.Concat(str, "/", serverRelativeUrl.TrimStart(chrArray));
				string str2 = targetWeb.Adapter.ServerUrl.TrimEnd(new char[] { '/' });
				string serverRelativeUrl1 = targetWeb.Adapter.ServerRelativeUrl;
				char[] chrArray1 = new char[] { '/' };
				string str3 = string.Concat(str2, "/", serverRelativeUrl1.TrimStart(chrArray1));
				base.LinkCorrector.RemoveMapping(str1);
				base.LinkCorrector.AddMapping(str1, str3);
			}
			string serverRelativeUrl2 = sourceWeb.Adapter.ServerRelativeUrl;
			string serverRelativeUrl3 = targetWeb.Adapter.ServerRelativeUrl;
			base.LinkCorrector.RemoveMapping(serverRelativeUrl2);
			base.LinkCorrector.AddMapping(serverRelativeUrl2, serverRelativeUrl3);
			if (bRecursive)
			{
				this.AddListAndViewMappingsToLinkCorrection(sourceWeb, targetWeb);
				foreach (SPWeb subWeb in sourceWeb.SubWebs)
				{
					SPWeb sPWeb = this.GetTargetWeb(subWeb, targetWeb, out flag);
					if (sPWeb == null)
					{
						continue;
					}
					if (flag)
					{
						this.AddRenamedSiteMapping(subWeb, sPWeb);
					}
					this.UpdateLinkCorrection(subWeb, sPWeb, bRecursive);
				}
			}
		}

		private void UpdateNodeTitle(SPNavigationNode node, SPWeb parentWeb)
		{
			if (string.IsNullOrEmpty(node.Url) || this.RenamedListMap.Count == 0 && this.RenamedSiteMap.Count == 0)
			{
				return;
			}
			StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(parentWeb.Adapter, node.Url);
			if (standardizedUrl.ServerRelative == null)
			{
				return;
			}
			if (this.RenamedSiteMap.ContainsKey(standardizedUrl.ServerRelative))
			{
				KeyValuePair<SPWeb, SPWeb> item = this.RenamedSiteMap[standardizedUrl.ServerRelative];
				if (node.Title == item.Key.Title)
				{
					node.Title = item.Value.Title;
				}
				return;
			}
			if (this.RenamedListMap.ContainsKey(standardizedUrl.ServerRelative))
			{
				KeyValuePair<SPList, SPList> keyValuePair = this.RenamedListMap[standardizedUrl.ServerRelative];
				if (node.Title == keyValuePair.Key.Title)
				{
					node.Title = keyValuePair.Value.Title;
				}
			}
		}
	}
}