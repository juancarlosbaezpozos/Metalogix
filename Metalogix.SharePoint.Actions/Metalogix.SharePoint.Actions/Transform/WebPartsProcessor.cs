using Metalogix;
using Metalogix.Actions;
using Metalogix.Core;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class WebPartsProcessor : PreconfiguredTransformer<SPWebPart, PasteAction<WebPartOptions>, SPWebPartCollection, SPWebPartCollection>
	{
		public override string Name
		{
			get
			{
				return "Process Standard Web Parts";
			}
		}

		public WebPartsProcessor()
		{
		}

		private void AddMappingsFromDataFormDataSources(PasteAction<WebPartOptions> action, string sDataSourceProperty, Dictionary<string, string> listGuidMapper, Dictionary<string, string> webUrlMapper, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (string.IsNullOrEmpty(sDataSourceProperty))
			{
				return;
			}
			foreach (Match match in (new Regex("<%\\s*@\\s+Register[^Tt]+[Tt]ag[Pp]refix\\s*=\\s*\"(?<TagPrefix>[^\"]+)\"[^%]*%>")).Matches(sDataSourceProperty))
			{
				sDataSourceProperty = sDataSourceProperty.Replace(string.Concat(match.Groups["TagPrefix"].Value, ":"), "");
			}
			sDataSourceProperty = sDataSourceProperty.Replace("<asp:", "<");
			sDataSourceProperty = sDataSourceProperty.Replace("</asp:", "</");
			int num = sDataSourceProperty.IndexOf("<AggregateDataSource", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				num = sDataSourceProperty.IndexOf("<SPDataSource", StringComparison.OrdinalIgnoreCase);
			}
			if (num < 0)
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sDataSourceProperty.Substring(num));
			foreach (XmlNode xmlNodes in xmlDocument.FirstChild.SelectNodes("//SPDataSource[@DataSourceMode= 'List' or 'ListItem']"))
			{
				foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes(".//SelectParameters | .//UpdateParameters | .//InsertParameters | .//DeleteParameters"))
				{
					XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode(".//DataFormParameter[translate(@Name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='weburl'][@DefaultValue] | .//Parameter[@Name='WebURL'][@DefaultValue]");
					XmlNode xmlNodes3 = xmlNodes1.SelectSingleNode(".//DataFormParameter[@Name='ListID'][@DefaultValue] | .//Parameter[@Name='ListID'][@DefaultValue]");
					this.CreateDataFormMappings(action, xmlNodes2, xmlNodes3, listGuidMapper, webUrlMapper, sourceWeb, targetWeb);
				}
			}
		}

		private void AddMappingsFromDataFormParameterBindings(PasteAction<WebPartOptions> action, string sParameterBindings, Dictionary<string, string> listGuidMapper, Dictionary<string, string> webUrlMapper, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (string.IsNullOrEmpty(sParameterBindings))
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(string.Concat("<Bindings>", sParameterBindings, "</Bindings>"));
			XmlNode firstChild = xmlDocument.FirstChild;
			XmlNode xmlNodes = firstChild.SelectSingleNode(".//ParameterBinding[@Name='WebURL'][@DefaultValue]");
			XmlNode xmlNodes1 = firstChild.SelectSingleNode(".//ParameterBinding[@Name='ListID'][@DefaultValue]");
			this.CreateDataFormMappings(action, xmlNodes, xmlNodes1, listGuidMapper, webUrlMapper, sourceWeb, targetWeb);
		}

		private static void AssignTargetGroupToMembersWebpart(SPWebPart membresWebPart, SPGroup targetGroup)
		{
			if (targetGroup.ID != null)
			{
				membresWebPart["MembershipGroupId"] = targetGroup.ID;
			}
		}

		private static void AssignTargetUserToContactDetailsWebpart(SPWebPart contactDetailsWebPart, SPUser targetUser)
		{
			if (!string.IsNullOrEmpty(contactDetailsWebPart["ContactLoginName"]) && targetUser.PrincipalName != null)
			{
				contactDetailsWebPart["ContactLoginName"] = targetUser.PrincipalName;
				return;
			}
			if (targetUser.ID != null)
			{
				contactDetailsWebPart["Contact"] = targetUser.ID;
			}
		}

		public override void BeginTransformation(PasteAction<WebPartOptions> action, SPWebPartCollection sources, SPWebPartCollection targets)
		{
			this.ProcessExistingWebParts(action, targets.Parent);
			this.MapWebPartsToTargetPage(sources.Parent, targets.Parent);
		}

		private void CorrectLinksInDataFormXml(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			Dictionary<string, string> strs1 = new Dictionary<string, string>();
			string item = wp["DataSourcesString"];
			this.AddMappingsFromDataFormDataSources(action, item, strs, strs1, sourceWeb, targetWeb);
			string str = wp["ParameterBindings"];
			this.AddMappingsFromDataFormParameterBindings(action, str, strs, strs1, sourceWeb, targetWeb);
			string[] strArrays = new string[] { "DataSourcesString", "ParameterBindings", "ListName" };
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i];
				string item1 = wp[str1];
				if (!string.IsNullOrEmpty(item1))
				{
					foreach (string key in strs.Keys)
					{
						item1 = item1.Replace(key, strs[key]);
					}
					foreach (string key1 in strs1.Keys)
					{
						item1 = item1.Replace(string.Concat("DefaultValue=\"", key1, "\""), string.Concat("DefaultValue=\"", strs1[key1], "\""));
					}
					wp[str1] = item1;
				}
			}
			string item2 = wp["ListId"];
			if (!string.IsNullOrEmpty(item2))
			{
				foreach (string key2 in strs.Keys)
				{
					item2 = item2.Replace(key2.ToLower(), strs[key2]);
				}
				wp["ListId"] = item2;
			}
		}

		private void CorrectLinksInDataViewQuery(PasteAction<WebPartOptions> action, SPWebPart wp, XmlNode webPartXml, SPWebPartPage sourcePage, SPWebPartPage targetPage)
		{
			SPWeb parentWeb = sourcePage.ParentWeb;
			SPWeb sPWeb = targetPage.ParentWeb;
			if (webPartXml == null || parentWeb == null || sPWeb == null)
			{
				return;
			}
			XmlNode xmlNodes = webPartXml.SelectSingleNode("./*[local-name() = 'DataQuery']");
			if (xmlNodes == null || xmlNodes.ChildNodes.Count <= 0)
			{
				return;
			}
			try
			{
				XmlNode itemOf = xmlNodes.ChildNodes[0];
				if (itemOf != null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlDocument.NameTable);
					xmlNamespaceManagers.AddNamespace("dsp", "http://schemas.microsoft.com/sharepoint/dsp");
					xmlNamespaceManagers.AddNamespace("udcs", "http://schemas.microsoft.com/data/udc/soap");
					xmlNamespaceManagers.AddNamespace("udcs", "http://schemas.microsoft.com/data/udc");
					xmlNamespaceManagers.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
					xmlDocument.LoadXml(itemOf.InnerText);
					XmlNode firstChild = xmlDocument.FirstChild;
					XmlNodeList xmlNodeLists = firstChild.SelectNodes(".//dsQuery[@select] | .//dsp:dsQuery[@select]", xmlNamespaceManagers);
					XmlNodeList xmlNodeLists1 = firstChild.SelectNodes(".//root | .//dsp:root", xmlNamespaceManagers);
					Dictionary<string, string> strs = new Dictionary<string, string>();
					foreach (XmlNode xmlNodes1 in xmlNodeLists1)
					{
						if (string.IsNullOrEmpty(xmlNodes1.InnerText))
						{
							continue;
						}
						string str = action.LinkCorrector.CorrectUrl(xmlNodes1.InnerText);
						if (string.IsNullOrEmpty(str))
						{
							continue;
						}
						strs.Add(xmlNodes1.InnerText, str);
					}
					if (strs.Count <= 0)
					{
						strs.Add(parentWeb.ServerRelativeUrl.ToLower().Replace(" ", "%20"), sPWeb.ServerRelativeUrl.ToLower().Replace(" ", "%20"));
					}
					foreach (string key in strs.Keys)
					{
						string innerXml = xmlNodes.InnerXml;
						innerXml = innerXml.Replace(key, strs[key]);
						xmlNodes.InnerXml = innerXml;
					}
					SPSite rootSite = parentWeb.RootSite;
					SPSite sPSite = sPWeb.RootSite;
					foreach (XmlNode xmlNodes2 in xmlNodeLists)
					{
						if (xmlNodes2.Attributes["select"] == null)
						{
							continue;
						}
						string value = xmlNodes2.Attributes["select"].Value;
						Match match = (new Regex("/list\\[@id='{(?<GUID>\\w{8}-\\w{4}-\\w{4}-\\w{4}-\\w{12})}'\\]", RegexOptions.IgnoreCase)).Match(value);
						string value1 = match.Groups["GUID"].Value;
						if (string.IsNullOrEmpty(value1))
						{
							throw new Exception("The source list GUID could not be found in the query node.");
						}
						SPList sPList = null;
						string str1 = null;
						foreach (string key1 in strs.Keys)
						{
							if (string.IsNullOrEmpty(key1))
							{
								continue;
							}
							string str2 = key1;
							if (key1.StartsWith(rootSite.ServerRelativeUrl, StringComparison.OrdinalIgnoreCase))
							{
								str2 = key1.Substring(rootSite.ServerRelativeUrl.Length);
							}
							SPWeb nodeByPath = rootSite.GetNodeByPath(str2.Replace("%20", " ")) as SPWeb;
							if (nodeByPath == null)
							{
								continue;
							}
							SPList listByGuid = nodeByPath.Lists.GetListByGuid(value1);
							if (listByGuid == null)
							{
								continue;
							}
							sPList = listByGuid;
							str1 = key1;
							break;
						}
						if (sPList == null)
						{
							SPList listByGuid1 = parentWeb.Lists.GetListByGuid(value1);
							if (listByGuid1 != null)
							{
								sPList = listByGuid1;
								str1 = parentWeb.ServerRelativeUrl.ToLower().Replace(" ", "%20");
							}
						}
						if (sPList == null || string.IsNullOrEmpty(str1))
						{
							throw new Exception("The list referenced by the data view web part could not be found on the source.");
						}
						string name = sPList.Name;
						string str3 = str1.Replace(" ", "%20");
						string str4 = "";
						if (strs.ContainsKey(str3))
						{
							str4 = strs[str3].Replace("%20", " ");
							if (sPWeb.SiteCollectionServerRelativeUrl != "/")
							{
								str4 = str4.ToLower().Replace(sPWeb.SiteCollectionServerRelativeUrl.ToLower(), "");
							}
						}
						SPWeb nodeByPath1 = sPSite.GetNodeByPath(str4) as SPWeb;
						if (nodeByPath1 == null)
						{
							throw new Exception(string.Concat("The target web for the list referenced in the data view web part could not be found: ", str4));
						}
						SPList item = nodeByPath1.Lists[name];
						if (item == null)
						{
							throw new Exception(string.Concat("The list referenced by the web part does not exist on the destination site: ", nodeByPath1.ServerRelativeUrl, "."));
						}
						string innerXml1 = xmlNodes.InnerXml;
						xmlNodes.InnerXml = innerXml1.Replace(value1, item.ID);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem(Resources.ProcessingDataViewWebPart, Resources.DataViewWebPart, sourcePage.ServerRelativeUrl, targetPage.ServerRelativeUrl, ActionOperationStatus.Warning);
				base.FireOperationStarted(logItem);
				logItem.Exception = exception;
				logItem.Information = string.Format(Resources.DataViewWebPartProcessingErrorMessage, this.GetIdentityStringForErrorMessage(wp, false), exception.Message);
				base.FireOperationFinished(logItem);
			}
		}

		private string CorrectListGuid(PasteAction<WebPartOptions> action, string sSourceListGuid, SPWeb sourceWeb, SPWeb targetWeb)
		{
			SPList item;
			if (string.IsNullOrEmpty(sSourceListGuid))
			{
				return null;
			}
			string d = action.LinkCorrector.MapGuid(sSourceListGuid);
			if (string.IsNullOrEmpty(d) && sourceWeb != null && targetWeb != null)
			{
				SPList listByGuid = sourceWeb.Lists.GetListByGuid(sSourceListGuid);
				if (listByGuid != null)
				{
					item = targetWeb.Lists[listByGuid.Name];
				}
				else
				{
					item = null;
				}
				SPList sPList = item;
				if (sPList != null)
				{
					d = sPList.ID;
				}
			}
			return d;
		}

		private string CorrectListGuidWithinSiteCollectionScope(PasteAction<WebPartOptions> action, SPWeb sourceWeb, SPWeb targetWeb, string sSourceListGuid, string sSourceListWebUrl, string sTargetListWebUrl)
		{
			if (sourceWeb == null || targetWeb == null || string.IsNullOrEmpty(sSourceListGuid))
			{
				return null;
			}
			string d = null;
			d = action.LinkCorrector.MapGuid(sSourceListGuid);
			if (string.IsNullOrEmpty(d))
			{
				SPSite rootSite = sourceWeb.RootSite;
				SPWeb nodeByPath = rootSite.GetNodeByPath(this.GetSiteCollectionRelativeUrl(rootSite, sSourceListWebUrl)) as SPWeb;
				SPList listByGuid = null;
				if (nodeByPath != null)
				{
					listByGuid = nodeByPath.Lists.GetListByGuid(sSourceListGuid);
				}
				if (listByGuid != null)
				{
					SPSite sPSite = targetWeb.RootSite;
					SPWeb sPWeb = sPSite.GetNodeByPath(this.GetSiteCollectionRelativeUrl(sPSite, sTargetListWebUrl)) as SPWeb;
					if (sPWeb != null)
					{
						SPList item = sPWeb.Lists[listByGuid.Name];
						if (item != null)
						{
							d = item.ID;
						}
						else if (listByGuid.BaseTemplate == ListTemplateType.O12Pages)
						{
							foreach (SPList list in sPWeb.Lists)
							{
								if (list.BaseTemplate != ListTemplateType.O12Pages)
								{
									continue;
								}
								d = list.ID;
								break;
							}
						}
					}
				}
			}
			return d;
		}

		private void CreateDataFormMappings(PasteAction<WebPartOptions> action, XmlNode urlNode, XmlNode listIdNode, Dictionary<string, string> listGuidMapper, Dictionary<string, string> webUrlMapper, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string str;
			string value;
			string str1;
			if (listIdNode != null)
			{
				string value1 = listIdNode.Attributes["DefaultValue"].Value;
				char[] chrArray = new char[] { '{', '}' };
				str = value1.Trim(chrArray);
			}
			else
			{
				str = null;
			}
			string str2 = str;
			if (string.IsNullOrEmpty(str2) || listGuidMapper.ContainsKey(str2))
			{
				return;
			}
			string str3 = null;
			if (urlNode != null)
			{
				value = urlNode.Attributes["DefaultValue"].Value;
			}
			else
			{
				value = null;
			}
			string str4 = value;
			if (!string.IsNullOrEmpty(str4))
			{
				string str5 = action.LinkCorrector.CorrectUrl(str4);
				if (string.IsNullOrEmpty(str5) || str5.Equals(str4, StringComparison.OrdinalIgnoreCase))
				{
					str5 = LinkCorrector.CorrectOutOfScopeUrl(str4, sourceWeb, targetWeb);
				}
				if (!string.IsNullOrEmpty(str5))
				{
					if (!webUrlMapper.ContainsKey(str4))
					{
						Dictionary<string, string> strs = webUrlMapper;
						string str6 = str4;
						if (str5 != null)
						{
							str1 = str5.Replace("%20", " ");
						}
						else
						{
							str1 = null;
						}
						strs.Add(str6, str1);
					}
					str3 = this.CorrectListGuidWithinSiteCollectionScope(action, sourceWeb, targetWeb, str2, str4, str5);
				}
			}
			else
			{
				str3 = this.CorrectListGuid(action, str2, sourceWeb, targetWeb);
			}
			if (!string.IsNullOrEmpty(str3))
			{
				listGuidMapper.Add(str2, str3);
			}
		}

		public override void EndTransformation(PasteAction<WebPartOptions> action, SPWebPartCollection sources, SPWebPartCollection targets)
		{
		}

		private string GetIdentityStringForErrorMessage(SPWebPart webPart, bool bCapitalizeFirstLetter)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			if (!bCapitalizeFirstLetter)
			{
				stringBuilder.Append("the");
			}
			else
			{
				stringBuilder.Append("The");
			}
			stringBuilder.Append(" web part");
			if (!string.IsNullOrEmpty(webPart.TypeName))
			{
				stringBuilder.Append(" of type ");
				stringBuilder.Append(webPart.TypeName);
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(webPart.Id))
			{
				stringBuilder.Append(" with ID \"");
				stringBuilder.Append(webPart.Id);
				stringBuilder.Append("\"");
				flag = true;
			}
			if (webPart.HasProperty("Title") && !string.IsNullOrEmpty(webPart["Title"]))
			{
				if (!flag)
				{
					stringBuilder.Append(" with");
				}
				else
				{
					stringBuilder.Append(" and");
				}
				stringBuilder.Append(" Title \"");
				stringBuilder.Append(webPart["Title"]);
				stringBuilder.Append("\"");
			}
			return stringBuilder.ToString();
		}

		private string GetSiteCollectionRelativeUrl(SPSite siteCollection, string sSourceListWebUrl)
		{
			string str = sSourceListWebUrl;
			if (!siteCollection.SiteCollectionServerRelativeUrl.Equals("/") && sSourceListWebUrl.StartsWith(siteCollection.SiteCollectionServerRelativeUrl, StringComparison.OrdinalIgnoreCase))
			{
				str = sSourceListWebUrl.Substring(siteCollection.SiteCollectionServerRelativeUrl.Length);
			}
			return str;
		}

		private SPGroup GetTargetGroup(PasteAction<WebPartOptions> action, SPGroup sourceGroup, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!action.PrincipalMappings.ContainsKey(sourceGroup.PrincipalName))
			{
				SPGroup[] sPGroupArray = new SPGroup[] { sourceGroup };
				action.EnsurePrincipalExistence(null, sPGroupArray, targetWeb, null, null);
			}
			if (action.PrincipalMappings.ContainsKey(sourceGroup.PrincipalName))
			{
				string item = action.PrincipalMappings[sourceGroup.PrincipalName];
				if (!string.IsNullOrEmpty(item) && targetWeb.Groups[item] != null)
				{
					return targetWeb.Groups[item] as SPGroup;
				}
			}
			return null;
		}

		private SPUser GetTargetUser(PasteAction<WebPartOptions> action, SPUser sourceUser, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!action.PrincipalMappings.ContainsKey(sourceUser.PrincipalName))
			{
				CopyUsersAction copyUsersAction = new CopyUsersAction();
				copyUsersAction.Options.SetFromOptions(action.Options);
				action.SubActions.Add(copyUsersAction);
				object[] sPUserCollection = new object[2];
				SPUser[] sPUserArray = new SPUser[] { sourceUser };
				sPUserCollection[0] = new SPUserCollection(sPUserArray);
				sPUserCollection[1] = targetWeb;
				copyUsersAction.RunAsSubAction(sPUserCollection, new ActionContext(null, targetWeb), null);
			}
			if (action.PrincipalMappings.ContainsKey(sourceUser.PrincipalName))
			{
				string item = action.PrincipalMappings[sourceUser.PrincipalName];
				if (!string.IsNullOrEmpty(item) && targetWeb.SiteUsers.Contains(item))
				{
					return targetWeb.SiteUsers[item] as SPUser;
				}
			}
			return null;
		}

		private SPWebPart LinkCorrectSpecificWebPartType(PasteAction<WebPartOptions> action, SPWebPart wp, SPWebPartPage sourcePage, SPWebPartPage targetPage)
		{
			SPWebPart sPWebPart = null;
			SPWeb parentWeb = sourcePage.ParentWeb;
			SPWeb sPWeb = targetPage.ParentWeb;
			if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.XsltListViewWebPart"))
			{
				sPWebPart = this.ProcessXsltListViewWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Publishing.WebControls.SummaryLinkWebPart"))
			{
				sPWebPart = this.ProcessSummaryLinksWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.ContentEditorWebPart"))
			{
				sPWebPart = this.ProcessContentEditorWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.ImageWebPart"))
			{
				sPWebPart = this.ProcessImageWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.PageViewerWebPart"))
			{
				wp = this.ProcessPageViewerWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Publishing.WebControls.TableOfContentsWebPart"))
			{
				sPWebPart = this.ProcessTableOfContentsWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.TasksAndToolsWebPart"))
			{
				sPWebPart = this.ProcessTasksAndToolsWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.BlogAdminWebPart") || wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.BlogMonthQuickLaunch"))
			{
				sPWebPart = this.ProcessBlogWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Publishing.WebControls.ContentByQueryWebPart"))
			{
				sPWebPart = this.ProcessContentByQueryWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.IndicatorWebpart"))
			{
				sPWebPart = this.ProcessIndicatorWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.KPIListWebPart"))
			{
				sPWebPart = this.ProcessKpiListWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.DataViewWebPart"))
			{
				sPWebPart = this.ProcessDataViewWebPart(action, wp, sourcePage, targetPage);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.DataFormWebPart"))
			{
				sPWebPart = this.ProcessDataFormWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.Office.Excel.WebUI.ExcelWebRenderer"))
			{
				sPWebPart = this.ProcessExcelWebAccessWebPart(action, wp);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.ContactFieldControl"))
			{
				sPWebPart = this.ProcessContactDetailWebPart(action, wp, sourcePage, targetPage);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.MembersWebPart"))
			{
				sPWebPart = this.ProcessMembersWebPart(action, wp, sourcePage, targetPage);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.ListFormWebPart"))
			{
				this.LinkCorrectWebPartProperty(action, wp, "ListName", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.UpperCase);
				this.LinkCorrectWebPartProperty(action, wp, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.XsltListFormWebPart"))
			{
				this.LinkCorrectWebPartProperty(action, wp, "ListName", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.UpperCase);
				this.LinkCorrectWebPartProperty(action, wp, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.ListViewWebPart"))
			{
				sPWebPart = this.ProcessListViewWebPart(action, wp, parentWeb, sPWeb);
				sPWebPart = this.Process2007AnnouncementWebPart(sPWebPart, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.RSSAggregatorWebPart") && wp["FeedUrl"] != null)
			{
				wp["FeedUrl"] = HttpUtility.UrlDecode(wp["FeedUrl"]);
				wp["FeedUrl"] = action.LinkCorrector.CorrectUrl(wp["FeedUrl"]);
				wp["FeedUrl"] = Regex.Replace(wp["FeedUrl"], "{([-\\w]*?)}", (Match m) => string.Concat("{", (action.LinkCorrector.MapGuid(m.ToString()) ?? m.Groups[1].Value).ToUpper(), "}"));
				this.LinkCorrectWebPartProperty(action, wp, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.SiteDocuments") && wp["UserTabs"] != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(wp["UserTabs"]);
				foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Pair"))
				{
					string value = xmlNodes.Attributes["Url"].Value;
					if (string.IsNullOrEmpty(value))
					{
						continue;
					}
					xmlNodes.Attributes["Url"].Value = action.LinkCorrector.CorrectUrl(value);
				}
				wp["UserTabs"] = xmlDocument.InnerXml;
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.PictureLibrarySlideshowWebPart"))
			{
				this.LinkCorrectWebPartProperty(action, wp, "ViewGuid", WebPartsProcessor.WebPartLinkCorrectionType.ViewGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.LowerCase);
				this.LinkCorrectWebPartProperty(action, wp, "LibraryGuid", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.LowerCase);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.WebPartPages.SPTimelineWebPart"))
			{
				this.LinkCorrectWebPartProperty(action, wp, "CurrentTaskListWebAddress", WebPartsProcessor.WebPartLinkCorrectionType.Url, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
				this.LinkCorrectWebPartProperty(action, wp, "PageAddress", WebPartsProcessor.WebPartLinkCorrectionType.Url, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
				this.LinkCorrectWebPartProperty(action, wp, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, parentWeb, sPWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
			}
			else if (wp.TypeName.Contains("Microsoft.SharePoint.Portal.WebControls.SpListFilterWebPart"))
			{
				sPWebPart = this.ProcessListFilterWebPart(action, wp, parentWeb, sPWeb);
			}
			else if (wp.TypeName.Contains("Microsoft.Office.Server.Search.WebControls.RefinementScriptWebPart"))
			{
				sPWebPart = this.ProcessRefinementScriptWebPart(wp, sourcePage);
			}
			return sPWebPart ?? wp;
		}

		private SPWebPart LinkCorrectWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			if (sPWebPart.TitleUrl != null)
			{
				sPWebPart.TitleUrl = action.LinkCorrector.CorrectUrl(sPWebPart.TitleUrl);
			}
			if (sPWebPart.HelpUrl != null)
			{
				sPWebPart.HelpUrl = action.LinkCorrector.CorrectUrl(sPWebPart.HelpUrl);
			}
			if (sPWebPart.TitleIconUrl != null)
			{
				sPWebPart.TitleIconUrl = action.LinkCorrector.CorrectUrl(sPWebPart.TitleIconUrl);
			}
			if (sPWebPart.CatalogIconImageUrl != null)
			{
				sPWebPart.CatalogIconImageUrl = action.LinkCorrector.CorrectUrl(sPWebPart.CatalogIconImageUrl);
			}
			return sPWebPart;
		}

		private void LinkCorrectWebPartProperty(PasteAction<WebPartOptions> action, SPWebPart wp, string sPropertyName, WebPartsProcessor.WebPartLinkCorrectionType correctionType)
		{
			this.LinkCorrectWebPartProperty(action, wp, sPropertyName, correctionType, null, null, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
		}

		private void LinkCorrectWebPartProperty(PasteAction<WebPartOptions> action, SPWebPart wp, string sPropertyName, WebPartsProcessor.WebPartLinkCorrectionType correctionType, WebPartsProcessor.WebPartLinkCorrectionCase linkCase)
		{
			this.LinkCorrectWebPartProperty(action, wp, sPropertyName, correctionType, null, null, linkCase);
		}

		private void LinkCorrectWebPartProperty(PasteAction<WebPartOptions> action, SPWebPart wp, string sPropertyName, WebPartsProcessor.WebPartLinkCorrectionType correctionType, SPWeb sourceWeb, SPWeb targetWeb)
		{
			this.LinkCorrectWebPartProperty(action, wp, sPropertyName, correctionType, sourceWeb, targetWeb, WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive);
		}

		private void LinkCorrectWebPartProperty(PasteAction<WebPartOptions> action, SPWebPart wp, string sPropertyName, WebPartsProcessor.WebPartLinkCorrectionType correctionType, SPWeb sourceWeb, SPWeb targetWeb, WebPartsProcessor.WebPartLinkCorrectionCase linkCase)
		{
			string name;
			string contentTypeID;
			if (wp.HasProperty(sPropertyName))
			{
				switch (correctionType)
				{
					case WebPartsProcessor.WebPartLinkCorrectionType.Url:
					{
						string item = wp[sPropertyName];
						item = (!wp.TypeName.Contains("Microsoft.SharePoint.Publishing.WebControls.ContentByQueryWebPart") || !item.Contains("~sitecollection") ? action.LinkCorrector.CorrectUrl(item) : this.LinkCorrectWebPartUrlProperty(action, item, sourceWeb, targetWeb));
						wp[sPropertyName] = item;
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.ListGuid:
					{
						string d = action.LinkCorrector.MapGuid(wp[sPropertyName]);
						if (string.IsNullOrEmpty(d) && sourceWeb != null && targetWeb != null)
						{
							SPListCollection lists = sourceWeb.Lists;
							string str = wp[sPropertyName];
							char[] chrArray = new char[] { '{', '}' };
							SPList listByGuid = lists.GetListByGuid(str.Trim(chrArray));
							if (listByGuid != null)
							{
								SPList sPList = targetWeb.Lists[listByGuid.Name];
								if (sPList != null)
								{
									d = sPList.ID;
								}
							}
						}
						if (string.IsNullOrEmpty(d))
						{
							break;
						}
						bool flag = (!wp[sPropertyName].StartsWith("{") ? false : wp[sPropertyName].EndsWith("}"));
						wp[sPropertyName] = (flag ? string.Concat("{", d, "}") : d);
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.WebGuid:
					{
						if (sourceWeb == null || targetWeb == null || !sourceWeb.ID.Equals(wp[sPropertyName], StringComparison.OrdinalIgnoreCase))
						{
							break;
						}
						wp[sPropertyName] = targetWeb.ID;
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.ViewGuid:
					{
						if (string.IsNullOrEmpty(wp["LibraryGuid"]))
						{
							break;
						}
						string str1 = action.LinkCorrector.MapGuid(wp["LibraryGuid"]);
						if (sourceWeb != null && targetWeb != null)
						{
							SPListCollection sPListCollection = sourceWeb.Lists;
							string item1 = wp["LibraryGuid"];
							char[] chrArray1 = new char[] { '{', '}' };
							SPList listByGuid1 = sPListCollection.GetListByGuid(item1.Trim(chrArray1));
							if (listByGuid1 != null)
							{
								SPList sPList1 = (string.IsNullOrEmpty(str1) ? targetWeb.Lists[listByGuid1.Name] : targetWeb.Lists.GetListByGuid(str1));
								SPView sPView = listByGuid1.Views[string.Concat("{", wp[sPropertyName], "}")];
								if (sPView != null && sPList1 != null)
								{
									string name1 = sPList1.Views.GetViewByDisplayName(sPView.DisplayName).Name;
									char[] chrArray2 = new char[] { '{', '}' };
									str1 = name1.Trim(chrArray2);
								}
							}
						}
						if (string.IsNullOrEmpty(str1))
						{
							break;
						}
						bool flag1 = (!wp[sPropertyName].StartsWith("{") ? false : wp[sPropertyName].EndsWith("}"));
						wp[sPropertyName] = (flag1 ? string.Concat("{", str1, "}") : str1);
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.Html:
					{
						wp[sPropertyName] = action.LinkCorrector.CorrectHtml(wp[sPropertyName]);
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.Text:
					{
						wp[sPropertyName] = action.LinkCorrector.CorrectText(wp[sPropertyName].ToString());
						break;
					}
					case WebPartsProcessor.WebPartLinkCorrectionType.ContentType:
					{
						if (sourceWeb == null || targetWeb == null)
						{
							break;
						}
						string item2 = wp[sPropertyName];
						if (string.IsNullOrEmpty(item2))
						{
							break;
						}
						SPContentType sPContentType = sourceWeb.ContentTypes[item2];
						if (sPContentType != null)
						{
							name = sPContentType.Name;
						}
						else
						{
							name = null;
						}
						string str2 = name;
						if (string.IsNullOrEmpty(str2))
						{
							break;
						}
						SPContentType contentTypeByName = targetWeb.ContentTypes.GetContentTypeByName(str2);
						if (contentTypeByName != null)
						{
							contentTypeID = contentTypeByName.ContentTypeID;
						}
						else
						{
							contentTypeID = null;
						}
						string str3 = contentTypeID;
						if (string.IsNullOrEmpty(str3))
						{
							break;
						}
						wp[sPropertyName] = str3;
						break;
					}
				}
				if (linkCase == WebPartsProcessor.WebPartLinkCorrectionCase.LowerCase)
				{
					wp[sPropertyName] = wp[sPropertyName].ToLower();
					return;
				}
				if (linkCase == WebPartsProcessor.WebPartLinkCorrectionCase.UpperCase)
				{
					wp[sPropertyName] = wp[sPropertyName].ToUpper();
				}
			}
		}

		private string LinkCorrectWebPartUrlProperty(PasteAction<WebPartOptions> action, string webpartUrl, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string str = webpartUrl;
			if (sourceWeb != null)
			{
				str = str.Replace("~sitecollection", sourceWeb.RootWeb.ServerRelativeUrl);
			}
			str = action.LinkCorrector.CorrectUrl(str);
			if (!str.Contains("~sitecollection") && targetWeb != null)
			{
				str = str.Replace(targetWeb.RootWeb.ServerRelativeUrl, "~sitecollection");
			}
			return str;
		}

		private string MapGuidString(string guidString, PasteAction<WebPartOptions> action)
		{
			Guid guid;
			if (action.GuidMappings == null || !Utils.IsGuid(guidString))
			{
				return guidString;
			}
			Guid guid1 = new Guid(guidString);
			if (!action.GuidMappings.TryGetValue(guid1, out guid))
			{
				return guidString;
			}
			return guid.ToString((guidString.StartsWith("{") ? "B" : "D"));
		}

		private SPWebPart MapWebPartGuids(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp.HasProperty("WebId"))
			{
				wp["WebId"] = this.MapGuidString(wp["WebId"], action);
			}
			return wp;
		}

		private List<SPWebPart> MapWebPartsToTargetPage(SPWebPartPage sourceWebPartPage, SPWebPartPage targetWebPartPage)
		{
			List<SPWebPart> sPWebParts = new List<SPWebPart>();
			SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
			if (targetWebPartPage.HasEmbeddedWikiField)
			{
				sPWebPartZoneSet.AddZones(new string[] { "Right", "Left" });
			}
			SPWebPartZoneMapping sPWebPartZoneMapping = new SPWebPartZoneMapping(SPWebPartZoneSet.Union(targetWebPartPage.Zones, sPWebPartZoneSet));
			Dictionary<string, int> nextAvailablePartOrders = targetWebPartPage.GetNextAvailablePartOrders();
			SPWebPartZoneSet zonesInUse = targetWebPartPage.ZonesInUse;
			List<SPWebPart> sPWebParts1 = new List<SPWebPart>();
			Dictionary<string, List<SPWebPart>> webPartsSortedByZoneAndPartOrder = SPWebPartCollection.GetWebPartsSortedByZoneAndPartOrder(sourceWebPartPage.WebParts, true);
			foreach (string key in webPartsSortedByZoneAndPartOrder.Keys)
			{
				string item = sPWebPartZoneMapping[key];
				int partOrder = (nextAvailablePartOrders.ContainsKey(item) ? nextAvailablePartOrders[item] : 0);
				bool flag = (partOrder > 0 ? false : !zonesInUse.Contains(item));
				foreach (SPWebPart sPWebPart in webPartsSortedByZoneAndPartOrder[key])
				{
					if (!sPWebPart.IsClosed)
					{
						sPWebPart.Zone = item;
						if (flag)
						{
							partOrder = sPWebPart.PartOrder + 1;
						}
						else
						{
							sPWebPart.PartOrder = partOrder;
							partOrder++;
						}
						sPWebParts.Add(sPWebPart);
					}
					else
					{
						sPWebParts1.Add(sPWebPart);
					}
				}
				nextAvailablePartOrders[item] = partOrder;
			}
			if (sPWebParts1.Count > 0)
			{
				foreach (SPWebPart lower in sPWebParts1)
				{
					lower.Zone = sPWebPartZoneMapping[lower.Zone].ToLower();
					lower.PartOrder = (nextAvailablePartOrders.ContainsKey(lower.Zone) ? nextAvailablePartOrders[lower.Zone] : 0);
					nextAvailablePartOrders[lower.Zone] = lower.PartOrder + 1;
					sPWebParts.Add(lower);
				}
			}
			return sPWebParts;
		}

		private SPWebPart PerformTransformationTasks(PasteAction<WebPartOptions> action, SPWebPart spWebPart)
		{
			SPWebPart sPWebPart;
			IEnumerator enumerator = action.SharePointOptions.TaskCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TransformationTask current = (TransformationTask)enumerator.Current;
					Trace.WriteLine(current);
					if (!current.ApplyTo.Evaluate(spWebPart))
					{
						continue;
					}
					string str = current.PerformTransformation(spWebPart.Xml);
					Trace.WriteLine(str);
					sPWebPart = SPWebPart.CreateWebPart(str);
					return sPWebPart;
				}
				return spWebPart;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return sPWebPart;
		}

		private SPWebPart Process2007AnnouncementWebPart(SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			try
			{
				if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2007 && !sourceWeb.Adapter.IsDB && targetWeb.Adapter.IsClientOM)
				{
					SPWebPart outerXml = wp.Clone();
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(outerXml.Xml);
					XmlNode xmlNodes = xmlDocument.FirstChild.SelectSingleNode("./*[local-name()='ListName']");
					if (xmlNodes != null)
					{
						string innerXml = xmlNodes.InnerXml;
						if (!string.IsNullOrEmpty(innerXml))
						{
							SPList item = sourceWeb.Lists[innerXml];
							if (item != null && item.BaseTemplate == ListTemplateType.Announcements)
							{
								XmlNode xmlNode = XmlUtility.StringToXmlNode(outerXml["ListViewXml"]);
								if (xmlNode != null)
								{
									string str = xmlNode.InnerXml;
									string str1 = "_spDelayedDomUpdates[";
									string str2 = "CDATA[ ]";
									int num = str.IndexOf(str1, StringComparison.InvariantCultureIgnoreCase);
									if (num > 0)
									{
										int num1 = str.IndexOf(str2, num, StringComparison.InvariantCultureIgnoreCase);
										if (num1 > 0)
										{
											str = str.Remove(num1 + (str2.Length - 1), 1).Insert(num1 + (str2.Length - 1), ").innerHTML");
											str = str.Replace(str1, "document.getElementById(");
											xmlNode.InnerXml = str;
											outerXml["ListViewXml"] = xmlNode.OuterXml;
											return outerXml;
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, "Process2007AnnouncementWebPart", true);
			}
			return wp;
		}

		private SPWebPart ProcessBlogWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListName", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb, WebPartsProcessor.WebPartLinkCorrectionCase.UpperCase);
			return sPWebPart;
		}

		private SPWebPart ProcessContactDetailWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWebPartPage sourcePage, SPWebPartPage targetPage)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			bool flag = false;
			SPUser userByID = sourcePage.ParentWeb.SiteUsers.GetUserByID(sPWebPart["Contact"]);
			if (userByID == null && sourcePage.ParentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater && !string.IsNullOrEmpty(sPWebPart["ContactLoginName"]))
			{
				userByID = (SPUser)sourcePage.ParentWeb.SiteUsers[sPWebPart["ContactLoginName"]];
			}
			if (userByID != null)
			{
				SPUser targetUser = this.GetTargetUser(action, userByID, sourcePage.ParentWeb, targetPage.ParentWeb);
				if (targetUser != null)
				{
					WebPartsProcessor.AssignTargetUserToContactDetailsWebpart(sPWebPart, targetUser);
					flag = true;
				}
			}
			if (!flag)
			{
				if (sPWebPart.HasProperty("Contact"))
				{
					sPWebPart.DeleteProperty("Contact");
				}
				LogItem logItem = new LogItem(Resources.ProcessingWebParts, wp.TypeName, sourcePage.ServerRelativeUrl, targetPage.ServerRelativeUrl, ActionOperationStatus.Warning)
				{
					Information = string.Format(Resources.ContactDetailsWebPartProcessingErrorMessage, this.GetIdentityStringForErrorMessage(wp, true)),
					SourceContent = wp.Xml
				};
				base.FireOperationStarted(logItem);
			}
			return sPWebPart;
		}

		private SPWebPart ProcessContentByQueryWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart serverRelativeUrl = wp.Clone();
			this.LinkCorrectWebPartProperty(action, serverRelativeUrl, "WebUrl", WebPartsProcessor.WebPartLinkCorrectionType.Url, sourceWeb, targetWeb);
			this.LinkCorrectWebPartProperty(action, serverRelativeUrl, "ListGuid", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb);
			this.LinkCorrectWebPartProperty(action, serverRelativeUrl, "ContentTypeBeginsWithId", WebPartsProcessor.WebPartLinkCorrectionType.ContentType, sourceWeb, targetWeb);
			bool flag = false;
			foreach (string str in new List<string>()
			{
				"FilterField1",
				"FilterField2",
				"FilterField3"
			})
			{
				if (!serverRelativeUrl.HasProperty(str) || string.IsNullOrEmpty(serverRelativeUrl[str]))
				{
					continue;
				}
				serverRelativeUrl[str] = this.MapGuidString(serverRelativeUrl[str], action);
				flag = true;
			}
			if (flag && serverRelativeUrl.HasProperty("WebUrl") && string.IsNullOrEmpty(serverRelativeUrl["WebUrl"]))
			{
				serverRelativeUrl["WebUrl"] = targetWeb.ServerRelativeUrl;
			}
			return serverRelativeUrl;
		}

		private SPWebPart ProcessContentEditorWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "Content", WebPartsProcessor.WebPartLinkCorrectionType.Html);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "Content", WebPartsProcessor.WebPartLinkCorrectionType.Text);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ContentLink", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessDataFormWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.CorrectLinksInDataFormXml(action, sPWebPart, sourceWeb, targetWeb);
			return sPWebPart;
		}

		private SPWebPart ProcessDataViewWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWebPartPage sourcePage, SPWebPartPage targetPage)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = null;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(wp.Xml);
			if (xmlNode != null)
			{
				this.CorrectLinksInDataViewQuery(action, wp, xmlNode, sourcePage, targetPage);
				sPWebPart = SPWebPart.CreateWebPart(xmlNode);
			}
			return sPWebPart;
		}

		private SPWebPart ProcessExcelWebAccessWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			this.LinkCorrectWebPartProperty(action, wp, "WorkbookUri", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return wp;
		}

		private void ProcessExistingWebParts(PasteAction<WebPartOptions> action, SPWebPartPage targetPage)
		{
			if (action.SharePointOptions.ExistingWebPartsAction == ExistingWebPartsProtocol.Close)
			{
				targetPage.CloseAllWebParts();
				return;
			}
			if (action.SharePointOptions.ExistingWebPartsAction == ExistingWebPartsProtocol.Delete)
			{
				targetPage.DeleteAllWebParts();
				return;
			}
			if (action.SharePointOptions.ExistingWebPartsAction != ExistingWebPartsProtocol.Preserve)
			{
				throw new Exception(string.Concat("Encountered an unhandled existing web parts action while trying to process web part page '", targetPage.ServerRelativeUrl, "': ", Enum.GetName(typeof(ExistingWebPartsProtocol), action.SharePointOptions.ExistingWebPartsAction)));
			}
		}

		private SPWebPart ProcessImageWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ImageLink", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessIndicatorWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListURL", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessKpiListWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListURL", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "TitleUrl", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessListFilterWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart str = wp.Clone();
			string item = str["ListUrl"];
			if (!item.EndsWith(".aspx"))
			{
				item = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, item).Full;
			}
			else
			{
				int num = item.LastIndexOf("/");
				item = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, item.Substring(0, num)).Full;
			}
			string str1 = action.LinkCorrector.CorrectUrl(item);
			str["ListUrl"] = str1;
			SPList nodeByUrl = (SPList)sourceWeb.Lists.GetNodeByUrl(item);
			SPList sPList = (SPList)targetWeb.Lists.GetNodeByUrl(str1);
			if (nodeByUrl != null && sPList != null)
			{
				SPView sPView = nodeByUrl.Views[string.Concat("{", wp["ViewGuid"], "}")];
				if (sPView != null)
				{
					SPView viewByDisplayName = sPList.Views.GetViewByDisplayName(sPView.DisplayName);
					if (viewByDisplayName != null)
					{
						string name = viewByDisplayName.Name;
						char[] chrArray = new char[] { '{', '}' };
						str["ViewGuid"] = name.Trim(chrArray);
					}
				}
				string item1 = str["ValueFieldGuid"];
				string item2 = str["DescriptionFieldGuid"];
				Guid guid = new Guid(item1);
				Guid guid1 = new Guid(item2);
				SPField fieldById = nodeByUrl.FieldCollection.GetFieldById(guid);
				SPField sPField = nodeByUrl.FieldCollection.GetFieldById(guid1);
				if (fieldById != null)
				{
					SPField fieldByNames = sPList.FieldCollection.GetFieldByNames(fieldById.DisplayName, fieldById.Name);
					if (fieldByNames != null)
					{
						str["ValueFieldGuid"] = fieldByNames.ID.ToString();
					}
				}
				if (sPField != null)
				{
					SPField fieldByNames1 = sPList.FieldCollection.GetFieldByNames(sPField.DisplayName, sPField.Name);
					if (fieldByNames1 != null)
					{
						str["DescriptionFieldGuid"] = fieldByNames1.ID.ToString();
					}
				}
			}
			return str;
		}

		private SPWebPart ProcessListViewWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart outerXml = wp.Clone();
			if (targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				this.LinkCorrectWebPartProperty(action, outerXml, "WebId", WebPartsProcessor.WebPartLinkCorrectionType.WebGuid, sourceWeb, targetWeb, WebPartsProcessor.WebPartLinkCorrectionCase.LowerCase);
				if (sourceWeb.Adapter.SharePointVersion.IsSharePoint2007OrEarlier)
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(outerXml["ListViewXml"]);
					XmlNode xmlNodes = xmlNode.SelectSingleNode(".//Toolbar");
					if (xmlNodes != null)
					{
						string innerXml = xmlNodes.InnerXml;
						if (innerXml.Contains("<a class=\"ms-addnew\" ID=\"idAddNewDoc\" href=\""))
						{
							innerXml = innerXml.Replace("<URL Cmd=\"New\" />", "<HttpVDir/><HTML>/_layouts/Upload.aspx?List=</HTML><ListProperty Select=\"Name\" />");
							innerXml = innerXml.Replace("?RootFolder=", "&amp;RootFolder=");
						}
						Match match = Regex.Match(innerXml, "<[^< ]*img src=\"/_layouts/images/rect\\.gif\"[^>]*>", RegexOptions.IgnoreCase);
						if (match.Success)
						{
							string value = match.Value;
							value = Regex.Replace(value, " style=\"[^\"]*\"", "", RegexOptions.IgnoreCase);
							value = Regex.Replace(value, " src=\"/_layouts/images/rect.gif\"", " src=\"/_layouts/images/fgimg.png\" style=\"left:-0px !important;top:-128px !important;position:absolute;\"", RegexOptions.IgnoreCase);
							value = string.Concat("<span style=\"height:10px;width:10px;position:relative;display:inline-block;overflow:hidden;\" class=\"s4-clust\">", value, "</span>");
							innerXml = innerXml.Remove(match.Index, match.Length);
							innerXml = innerXml.Insert(match.Index, value);
						}
						xmlNodes.InnerXml = innerXml;
						outerXml["ListViewXml"] = xmlNode.OuterXml;
					}
				}
			}
			return outerXml;
		}

		private SPWebPart ProcessMembersWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWebPartPage sourcePage, SPWebPartPage targetPage)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			bool flag = false;
			if (sPWebPart["MembershipGroupId"] != null)
			{
				SPGroup groupByID = sourcePage.ParentWeb.Groups.GetGroupByID(sPWebPart["MembershipGroupId"]);
				if (groupByID != null)
				{
					SPGroup targetGroup = this.GetTargetGroup(action, groupByID, sourcePage.ParentWeb, targetPage.ParentWeb);
					if (targetGroup != null)
					{
						WebPartsProcessor.AssignTargetGroupToMembersWebpart(sPWebPart, targetGroup);
						flag = true;
					}
				}
				if (!flag)
				{
					if (sPWebPart.HasProperty("MembershipGroupId") && sPWebPart.HasProperty("DisplayType"))
					{
						sPWebPart.DeleteProperty("MembershipGroupId");
						sPWebPart.DeleteProperty("DisplayType");
					}
					LogItem logItem = new LogItem(Resources.ProcessingWebParts, wp.TypeName, sourcePage.ServerRelativeUrl, targetPage.ServerRelativeUrl, ActionOperationStatus.Warning)
					{
						Information = string.Format(Resources.MembersWebPartProcessingErrorMessage, this.GetIdentityStringForErrorMessage(wp, true)),
						SourceContent = wp.Xml
					};
					base.FireOperationStarted(logItem);
				}
			}
			return sPWebPart;
		}

		private SPWebPart ProcessPageViewerWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ContentLink", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessRefinementScriptWebPart(SPWebPart refinementWebPart, SPWebPartPage sourcePage)
		{
			SPWebPart sPWebPart;
			SPWebPart sPWebPart1 = null;
			try
			{
				string str = Regex.Replace(refinementWebPart.Xml, sourcePage.ParentWeb.RootWeb.ServerRelativeUrl, "~sitecollection", RegexOptions.IgnoreCase);
				XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
				if (xmlNode != null)
				{
					sPWebPart1 = SPWebPart.CreateWebPart(xmlNode);
				}
				return sPWebPart1;
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Format("Error occurred while correcting links for RefinementScriptWebPart. Error : '{0}'.", exception));
				sPWebPart = refinementWebPart;
			}
			return sPWebPart;
		}

		private SPWebPart ProcessSummaryLinksWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			string str = "ManagedLinks";
			if (sPWebPart.HasProperty(str))
			{
				sPWebPart.DeleteProperty(str);
			}
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListName", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "SummaryLinkStore", WebPartsProcessor.WebPartLinkCorrectionType.Html);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb);
			return sPWebPart;
		}

		private SPWebPart ProcessTableOfContentsWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "AnchorLocation", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListName", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb);
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListId", WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb);
			string str = "Xsl";
			if (sPWebPart.HasProperty(str))
			{
				sPWebPart.DeleteProperty(str);
			}
			return sPWebPart;
		}

		private SPWebPart ProcessTasksAndToolsWebPart(PasteAction<WebPartOptions> action, SPWebPart wp)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "TasksAndToolsWebUrl", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			return sPWebPart;
		}

		private SPWebPart ProcessXsltListViewWebPart(PasteAction<WebPartOptions> action, SPWebPart wp, SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (wp == null)
			{
				return null;
			}
			SPWebPart sPWebPart = wp.Clone();
			this.LinkCorrectWebPartProperty(action, sPWebPart, "ListUrl", WebPartsProcessor.WebPartLinkCorrectionType.Url);
			string[] strArrays = new string[] { "ListName", "ListId" };
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				this.LinkCorrectWebPartProperty(action, sPWebPart, str, WebPartsProcessor.WebPartLinkCorrectionType.ListGuid, sourceWeb, targetWeb, (str.Equals("ListName", StringComparison.OrdinalIgnoreCase) ? WebPartsProcessor.WebPartLinkCorrectionCase.UpperCase : WebPartsProcessor.WebPartLinkCorrectionCase.CaseInsensitive));
			}
			this.LinkCorrectWebPartProperty(action, sPWebPart, "WebId", WebPartsProcessor.WebPartLinkCorrectionType.WebGuid, sourceWeb, targetWeb, WebPartsProcessor.WebPartLinkCorrectionCase.LowerCase);
			return sPWebPart;
		}

		public static bool SkipWebPartBasedOnType(string typeName, SharePointVersion targetVersion)
		{
			if (typeName == "Microsoft.SharePoint.Meetings.CustomToolPaneManager" || typeName == "Microsoft.SharePoint.Meetings.PageTabsWebPart")
			{
				return true;
			}
			if (targetVersion.IsSharePoint2010OrLater && (typeName == "Microsoft.SharePoint.Portal.WebControls.BreadCrumbTrail" || typeName == "Microsoft.SharePoint.Portal.WebControls.ListingSummary"))
			{
				return true;
			}
			if (targetVersion.IsSharePoint2007OrEarlier && typeName == "Microsoft.SharePoint.WebPartPages.ListFormWebPart")
			{
				return true;
			}
			return false;
		}

		public static bool SkipWebPartBasedOnZone(string zoneName)
		{
			if (string.IsNullOrEmpty(zoneName))
			{
				return false;
			}
			if (zoneName.Equals("meetingsummary", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return zoneName.Equals("meetingnavigator", StringComparison.OrdinalIgnoreCase);
		}

		public override SPWebPart Transform(SPWebPart dataObject, PasteAction<WebPartOptions> action, SPWebPartCollection sources, SPWebPartCollection targets)
		{
			SPWebPart sPWebPart;
			try
			{
				dataObject = this.PerformTransformationTasks(action, dataObject);
				if (WebPartsProcessor.SkipWebPartBasedOnZone(dataObject.Zone) || WebPartsProcessor.SkipWebPartBasedOnType(dataObject.TypeName, targets.Parent.Adapter.SharePointVersion) || dataObject.IsViewWebPart)
				{
					sPWebPart = null;
				}
				else if (action.SharePointOptions.CopyClosedWebParts || !dataObject.IsClosed)
				{
					SPWebPart sPWebPart1 = dataObject.Clone();
					if (action.MapWebPartAudiences(ref sPWebPart1, sources.Parent.ParentWeb, targets.Parent.ParentWeb))
					{
						dataObject = sPWebPart1;
					}
					if (dataObject.TypeName.Equals("Microsoft.SharePoint.Portal.WebControls.SocialCommentWebPart") || dataObject.TypeName.Equals("Microsoft.SharePoint.Portal.WebControls.TagCloudWebPart"))
					{
						dataObject = this.TransformSocialWebPartToV2(dataObject);
						sPWebPart = dataObject;
					}
					else
					{
						dataObject = this.LinkCorrectWebPart(action, dataObject);
						dataObject = this.MapWebPartGuids(action, dataObject);
						dataObject = this.LinkCorrectSpecificWebPartType(action, dataObject, sources.Parent, targets.Parent);
						return dataObject;
					}
				}
				else
				{
					sPWebPart = null;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem(Resources.ProcessingWebParts, dataObject.TypeName, sources.Parent.ServerRelativeUrl, targets.Parent.ServerRelativeUrl, ActionOperationStatus.Warning);
				base.FireOperationStarted(logItem);
				logItem.Exception = exception;
				logItem.Information = string.Format(Resources.GenericWebPartProcessingErrorMessage, this.GetIdentityStringForErrorMessage(dataObject, true), exception.Message);
				logItem.SourceContent = dataObject.Xml;
				base.FireOperationFinished(logItem);
				return dataObject;
			}
			return sPWebPart;
		}

		private SPWebPart TransformSocialWebPartToV2(SPWebPart v3WebPart)
		{
			string str = "<?xml version=\"1.0\"?>\r\n                            <xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">\r\n                                <xsl:output method=\"xml\" indent=\"yes\"/>\r\n                                <xsl:template match=\"/\">\r\n                                    <WebPart xmlns=\"http://schemas.microsoft.com/WebPart/v2\">\r\n                                        <Title><xsl:value-of select=\"//properties/property[@name='Title']\"/></Title>\r\n                                        <Description><xsl:value-of select=\"//properties/property[@name='Description']\"/></Description>\r\n                                        <FrameType>TitleBarOnly</FrameType>\r\n                                        <AllowRemove>true</AllowRemove>\r\n                                        <AllowMinimize>true</AllowMinimize>\r\n                                        <Assembly><xsl:value-of select=\"normalize-space(substring-after(//type/@name, ','))\"/></Assembly>\r\n                                        <TypeName><xsl:value-of select=\"normalize-space(substring-before(//type/@name, ','))\"/></TypeName>\r\n                                        <PartOrder><xsl:value-of select=\"//PartOrder\"/></PartOrder>\r\n                                        <ZoneID><xsl:value-of select=\"//ZoneID\"/></ZoneID>\r\n                                    </WebPart>\r\n                                </xsl:template>\r\n                            </xsl:stylesheet>";
			string str1 = v3WebPart.Xml.Replace("http://microsoft.com/sharepoint/webpartpages", "").Replace("http://schemas.microsoft.com/WebPart/v3", "");
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			XPathDocument xPathDocument = new XPathDocument(new StringReader(str1));
			string empty = string.Empty;
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(str)))
			{
				xslCompiledTransform.Load(xmlReader);
				using (StringWriter stringWriter = new StringWriter())
				{
					xslCompiledTransform.Transform(xPathDocument, null, stringWriter);
					empty = stringWriter.ToString();
					empty = empty.Substring(empty.IndexOf("<WebPart"));
				}
			}
			return SPWebPart.CreateWebPart(empty);
		}

		private enum WebPartLinkCorrectionCase
		{
			UpperCase,
			LowerCase,
			CaseInsensitive
		}

		private enum WebPartLinkCorrectionType
		{
			Url,
			ListGuid,
			WebGuid,
			ViewGuid,
			Html,
			Text,
			ContentType
		}
	}
}