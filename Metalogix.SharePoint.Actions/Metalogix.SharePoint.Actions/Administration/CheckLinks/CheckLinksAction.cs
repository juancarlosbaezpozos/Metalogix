using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Administration;
using Metalogix.SharePoint.Options.Administration.CheckLinks;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration.CheckLinks
{
	[CmdletEnabled(false, null, null)]
	[CompletionDetailsOrderProvider(typeof(CheckLinksCompletionDetailOrderProvider))]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.LinkCorrection.ico")]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("Check Links... {1-copy-compare}")]
	[Name("Check Links")]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPNode))]
	[UsesStickySettings(true)]
	public class CheckLinksAction : SharePointAction<CheckLinksOptions>
	{
		private const string CHECK_LINKS_DIRECTORY_NAME = "CheckLinksResults";

		private string m_sResultsFile = "";

		private ICredentials m_Credentials;

		private List<string> relativeLinkWebPartProperties = new List<string>(new string[] { "ImageLink", "HelpLink", "DetailLink", "TitleUrl", "HelpUrl", "PartImageSmall", "PartImageLarge", "CatalogIconImageUrl", "TitleIconImageUrl", "FeedUrl", "ContentLink", "ListUrl", "TasksAndToolsWebUrl", "AnchorLocation", "WebUrl" });

		public string ResultsFile
		{
			get
			{
				return this.m_sResultsFile;
			}
			set
			{
				this.m_sResultsFile = value;
			}
		}

		public CheckLinksAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections) || targetSelections == null || targetSelections.Count <= 0)
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (current is SPWeb || current is SPList || current is SPFolder || current is SPListItem)
					{
						continue;
					}
					flag = false;
					return flag;
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

		public CheckLinksResults CheckLinks(IEnumerable objectsToCheck, XmlTextWriter writer)
		{
			writer.WriteStartElement("CheckLinksResults");
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			CheckLinksResults checkLinksResult = new CheckLinksResults();
			foreach (object obj in objectsToCheck)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				checkLinksResult.Append(this.CheckLinks(obj, xmlTextWriter));
			}
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			this.WriteSummary(checkLinksResult, writer);
			writer.WriteRaw(stringBuilder.ToString());
			writer.WriteEndElement();
			writer.Flush();
			return checkLinksResult;
		}

		public CheckLinksResults CheckLinks(object oSharePointObject, XmlTextWriter writer)
		{
			CheckLinksResults checkLinksResult = new CheckLinksResults();
			if (oSharePointObject == null)
			{
				return checkLinksResult;
			}
			if (oSharePointObject is SPWeb)
			{
				this.CheckLinksInWeb((SPWeb)oSharePointObject, writer, ref checkLinksResult);
			}
			else if (oSharePointObject is SPList)
			{
				this.CheckLinksInList((SPList)oSharePointObject, writer, ref checkLinksResult);
			}
			else if (oSharePointObject is SPFolder)
			{
				this.CheckLinksInFolder((SPFolder)oSharePointObject, null, writer, ref checkLinksResult);
			}
			else if (oSharePointObject is SPListItem)
			{
				this.CheckLinksInItem((SPListItem)oSharePointObject, null, writer, ref checkLinksResult);
			}
			return checkLinksResult;
		}

		private void CheckLinksInFolder(SPFolder folderToCheck, List<string> fieldsWithLinks, XmlTextWriter writer, ref CheckLinksResults results)
		{
			if (folderToCheck == null)
			{
				return;
			}
			if (this.m_Credentials == null)
			{
				this.m_Credentials = folderToCheck.Credentials.NetworkCredentials;
			}
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Check folder links", folderToCheck.DisplayName, "", folderToCheck.LinkableUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					if (fieldsWithLinks == null)
					{
						fieldsWithLinks = this.GetFieldsWithLinks(folderToCheck.ParentList);
					}
					writer.WriteStartElement("Folder");
					writer.WriteAttributeString("Name", folderToCheck.Name);
					foreach (SPFolder subFolder in folderToCheck.SubFolders)
					{
						if (base.CheckForAbort())
						{
							break;
						}
						this.CheckLinksInFolder(subFolder, fieldsWithLinks, writer, ref results);
					}
					if (!base.CheckForAbort())
					{
						foreach (SPListItem item in folderToCheck.Items)
						{
							this.CheckLinksInItem(item, fieldsWithLinks, writer, ref results);
						}
					}
					writer.WriteEndElement();
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CheckLinksInItem(SPListItem itemToCheck, List<string> fieldsWithLinks, XmlTextWriter writer, ref CheckLinksResults results)
		{
			if (itemToCheck == null)
			{
				return;
			}
			if (this.m_Credentials == null)
			{
				this.m_Credentials = itemToCheck.Credentials.NetworkCredentials;
			}
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Check item links", itemToCheck.DisplayName, "", itemToCheck.LinkableUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					if (fieldsWithLinks == null)
					{
						fieldsWithLinks = this.GetFieldsWithLinks(itemToCheck.ParentList);
					}
					CheckLinksResults checkLinksResult = new CheckLinksResults();
					StringBuilder stringBuilder = new StringBuilder();
					foreach (string fieldsWithLink in fieldsWithLinks)
					{
						try
						{
							string item = itemToCheck[fieldsWithLink];
							if (!string.IsNullOrEmpty(item))
							{
								if (item.StartsWith("/") && item.Contains(","))
								{
									item = item.Substring(0, item.IndexOf(","));
									item = string.Concat((!string.IsNullOrEmpty(itemToCheck.Adapter.ServerUrl) ? itemToCheck.Adapter.ServerUrl.TrimEnd(new char[] { '/' }) : ""), item);
								}
								string serverUrl = itemToCheck.Adapter.ServerUrl;
								char[] chrArray = new char[] { '/' };
								List<string> strs = this.ExtractLinks(item, serverUrl.TrimEnd(chrArray));
								if (strs.Count > 0)
								{
									foreach (string str in strs)
									{
										SPCheckLink sPCheckLink = this.TestLink(str, LinkType.ListItem, null, null, itemToCheck.DisplayUrl, fieldsWithLink);
										if (sPCheckLink == null)
										{
											continue;
										}
										checkLinksResult.Add(sPCheckLink);
										this.UpdateLogItem(sPCheckLink, logItem);
									}
								}
							}
						}
						catch
						{
							stringBuilder.Append(string.Concat("An error occurred while trying to check the field '", fieldsWithLink, "'. "));
						}
					}
					StringBuilder stringBuilder1 = new StringBuilder();
					XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder1));
					CheckLinksResults checkLinksResult1 = new CheckLinksResults();
					if (base.SharePointOptions.CheckWebparts && SPWebPartPage.IsWebPartPage(itemToCheck))
					{
						this.CheckLinksInWebPartPage(new SPWebPartPage(itemToCheck, this), xmlTextWriter, ref checkLinksResult1);
					}
					xmlTextWriter.Flush();
					xmlTextWriter.Close();
					if (checkLinksResult.Count > 0 || checkLinksResult1.Count > 0)
					{
						writer.WriteStartElement("Item");
						writer.WriteAttributeString("ID", itemToCheck.ID.ToString());
						checkLinksResult.WriteXml(writer, false);
						if (base.SharePointOptions.CheckWebparts)
						{
							writer.WriteRaw(stringBuilder1.ToString());
						}
						writer.WriteEndElement();
						results.Append(checkLinksResult);
						results.Append(checkLinksResult1);
					}
					if (stringBuilder.Length <= 0)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = stringBuilder.ToString();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CheckLinksInList(SPList listToCheck, XmlTextWriter writer, ref CheckLinksResults results)
		{
			if (listToCheck == null)
			{
				return;
			}
			if (this.m_Credentials == null)
			{
				this.m_Credentials = listToCheck.Credentials.NetworkCredentials;
			}
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Check list links", listToCheck.DisplayName, "", listToCheck.LinkableUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					writer.WriteStartElement("List");
					writer.WriteAttributeString("Name", listToCheck.Name);
					writer.WriteAttributeString("ID", listToCheck.ID.ToUpper());
					List<string> fieldsWithLinks = this.GetFieldsWithLinks(listToCheck);
					foreach (SPFolder subFolder in listToCheck.SubFolders)
					{
						if (base.CheckForAbort())
						{
							break;
						}
						this.CheckLinksInFolder(subFolder, fieldsWithLinks, writer, ref results);
					}
					if (!base.CheckForAbort())
					{
						foreach (SPListItem item in listToCheck.Items)
						{
							this.CheckLinksInItem(item, fieldsWithLinks, writer, ref results);
						}
					}
					writer.WriteEndElement();
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CheckLinksInWeb(SPWeb webToCheck, XmlTextWriter writer, ref CheckLinksResults results)
		{
			CheckLinksResults checkLinksResult = new CheckLinksResults();
			if (webToCheck == null)
			{
				return;
			}
			this.m_Credentials = webToCheck.Credentials.NetworkCredentials;
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Check web links", webToCheck.DisplayName, "", webToCheck.LinkableUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					writer.WriteStartElement("Web");
					writer.WriteAttributeString("Name", webToCheck.Name);
					writer.WriteAttributeString("Url", webToCheck.LinkableUrl);
					writer.WriteAttributeString("ID", webToCheck.ID);
					if (webToCheck.SubWebs != null && base.SharePointOptions.CheckSubsites)
					{
						foreach (SPWeb subWeb in webToCheck.SubWebs)
						{
							if (base.CheckForAbort())
							{
								break;
							}
							this.CheckLinksInWeb(subWeb, writer, ref checkLinksResult);
						}
					}
					if (webToCheck.Lists != null && !base.CheckForAbort())
					{
						foreach (SPList list in webToCheck.Lists)
						{
							this.CheckLinksInList(list, writer, ref checkLinksResult);
						}
					}
					if (base.SharePointOptions.CheckWebparts && !base.CheckForAbort())
					{
						this.CheckLinksInWebPartPage((new SPWebPartPage()).GetWelcomePage(webToCheck, this), writer, ref checkLinksResult);
					}
					writer.WriteStartElement("Summary");
					writer.WriteAttributeString("Successes", checkLinksResult.Successes.Count.ToString());
					writer.WriteAttributeString("Failures", checkLinksResult.Failures.Count.ToString());
					int count = checkLinksResult.FlaggedSuccesses.Count + checkLinksResult.FlaggedFailures.Count;
					writer.WriteAttributeString("Flagged", count.ToString());
					writer.WriteEndElement();
					writer.WriteEndElement();
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
				webToCheck.Dispose();
			}
			results.Append(checkLinksResult);
		}

		private void CheckLinksInWebPartPage(SPWebPartPage wpp, XmlTextWriter writer, ref CheckLinksResults results)
		{
			if (wpp == null)
			{
				return;
			}
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Check web part page links", wpp.FileLeafRef, "", wpp.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					writer.WriteStartElement("WebParts");
					writer.WriteAttributeString("URL", wpp.DisplayUrl);
					List<string> strs = new List<string>();
					foreach (SPWebPart webPart in wpp.WebParts)
					{
						writer.WriteStartElement("WebPart");
						writer.WriteAttributeString("Name", (webPart["Title"] != null ? webPart["Title"] : ""));
						writer.WriteAttributeString("Type", webPart.TypeName ?? "");
						writer.WriteAttributeString("ID", webPart.Id.ToUpper());
						XmlNode xmlNode = XmlUtility.StringToXmlNode(webPart.Xml);
						if (xmlNode == null)
						{
							strs.Add(webPart["Title"] ?? webPart.TypeName);
						}
						else
						{
							try
							{
								XmlNodeList childNodes = null;
								if (webPart is SPWebPartV2)
								{
									childNodes = xmlNode.ChildNodes;
								}
								else if (webPart is SPWebPartV3)
								{
									childNodes = xmlNode.SelectNodes(".//*[local-name() = 'property']");
								}
								if (childNodes != null)
								{
									foreach (XmlNode childNode in childNodes)
									{
										string name = "";
										if (!childNode.Name.Equals("property", StringComparison.OrdinalIgnoreCase))
										{
											name = childNode.Name;
										}
										else if (childNode.Attributes["name"] != null)
										{
											name = childNode.Attributes["name"].Value;
										}
										if (name.Equals("webpart", StringComparison.OrdinalIgnoreCase) || name.Equals("sampledata", StringComparison.OrdinalIgnoreCase) || name.Equals("xsl", StringComparison.OrdinalIgnoreCase) || name.Equals("ListViewXml", StringComparison.OrdinalIgnoreCase))
										{
											continue;
										}
										List<string> strs1 = new List<string>();
										if (!this.relativeLinkWebPartProperties.Contains(name) || !childNode.InnerText.StartsWith("/"))
										{
											string innerText = childNode.InnerText;
											string serverUrl = wpp.Adapter.ServerUrl;
											char[] chrArray = new char[] { '/' };
											strs1 = this.ExtractLinks(innerText, serverUrl.TrimEnd(chrArray));
										}
										else
										{
											string str = wpp.Adapter.ServerUrl;
											char[] chrArray1 = new char[] { '/' };
											strs1.Add(string.Concat(str.TrimEnd(chrArray1), childNode.InnerText));
										}
										foreach (string str1 in strs1)
										{
											SPCheckLink sPCheckLink = this.TestLink(str1, LinkType.WebPart, webPart.TypeName ?? "", null, wpp.DisplayUrl, name);
											if (sPCheckLink == null)
											{
												continue;
											}
											results.Add(sPCheckLink);
											this.UpdateLogItem(sPCheckLink, logItem);
											if (sPCheckLink.IsValidLink && !base.SharePointOptions.ShowSuccesses)
											{
												continue;
											}
											sPCheckLink.WriteToXml(writer, false);
										}
									}
								}
								else
								{
									continue;
								}
							}
							catch
							{
								strs.Add(webPart["Title"] ?? webPart.TypeName);
							}
						}
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					if (strs.Count > 0)
					{
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Information = string.Concat("Some web parts on this page encountered errors when trying to check links. The titles of the problematic web parts are: ", Utils.EnumerableToString(strs, ", "));
					}
					else
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private List<string> ExtractLinks(string input, string relativePrefix)
		{
			List<string> strs = new List<string>();
			if (string.IsNullOrEmpty(input))
			{
				return strs;
			}
			string str = string.Concat("(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\\.))+(([\\w\\&amp;%_\\./-~-]+)|([\\w\\&amp;%_\\./-~-]+\\.[a-zA-Z]{2,6})|([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}))(:[0-9]{1,5})?(/[\\w\\&amp;%_\\./-~-]*)?", "|((href|src)=\"/[^\"]+\")");
			MatchCollection matchCollections = Regex.Matches(input, str, RegexOptions.IgnoreCase);
			string str1 = "";
			foreach (Match match in matchCollections)
			{
				string value = match.Value;
				if (value.StartsWith("href") || value.StartsWith("src"))
				{
					int num = value.IndexOf("\"") + 1;
					value = value.Substring(num, value.LastIndexOf("\"") - num);
				}
				if (value.StartsWith("www"))
				{
					value = string.Concat("http://", value);
				}
				if (value.Contains(","))
				{
					value = value.Substring(0, value.IndexOf(','));
				}
				if (value.Contains("\""))
				{
					value = value.Substring(0, value.IndexOf('\"'));
				}
				if (value.Contains("<"))
				{
					value = value.Substring(0, value.IndexOf("<"));
				}
				if (value.StartsWith("/"))
				{
					value = string.Concat(relativePrefix, value);
				}
				value = value.TrimEnd(new char[] { '/' });
				if (strs.Contains(value) || str1.Contains(value) && str1.TrimStart(value.ToCharArray()).StartsWith("%"))
				{
					continue;
				}
				if (value.Contains(str1) && value.TrimStart(str1.ToCharArray()).StartsWith("%"))
				{
					strs[strs.Count - 1] = value;
				}
				else if (!strs.Contains(value))
				{
					strs.Add(value);
				}
				str1 = value;
			}
			return strs;
		}

		private string FormatFilename(string name)
		{
			return name.Replace("/", "").Replace(":", "").Replace("\\", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("*", "");
		}

		private string GenerateFilename(string sPrefix)
		{
			Random random = new Random();
			object[] month = new object[] { "CheckLinks", null, null, null, null, null, null, null, null };
			month[1] = (!string.IsNullOrEmpty(sPrefix) ? string.Concat("_", this.FormatFilename(sPrefix)) : "");
			month[2] = "_";
			month[3] = DateTime.Today.Month;
			month[4] = "_";
			month[5] = DateTime.Today.Day;
			month[6] = "_";
			month[7] = random.Next();
			month[8] = ".xml";
			return string.Concat(month);
		}

		private List<string> GetFieldsWithLinks(SPList list)
		{
			List<string> strs = new List<string>();
			foreach (SPField field in list.Fields)
			{
				if (field.Name.StartsWith("_") || !field.Type.Equals("URL", StringComparison.OrdinalIgnoreCase) && !field.Type.Equals("Link", StringComparison.OrdinalIgnoreCase) && !field.Type.Equals("HTML", StringComparison.OrdinalIgnoreCase) && !field.Type.Equals("Image", StringComparison.OrdinalIgnoreCase) && !field.Type.Equals("SummaryLinks", StringComparison.OrdinalIgnoreCase) && !field.Name.Equals("WikiField", StringComparison.OrdinalIgnoreCase) && !field.Name.Equals("TemplateUrl", StringComparison.OrdinalIgnoreCase) && (!base.SharePointOptions.CheckTextFields || !field.Type.Equals("Text", StringComparison.OrdinalIgnoreCase) && (!field.Type.Equals("Note", StringComparison.OrdinalIgnoreCase) || field.FieldXML.Attributes["NumLines"] == null)))
				{
					continue;
				}
				strs.Add(field.Name);
			}
			return strs;
		}

		private bool IsFlagged(string sUrl)
		{
			bool flag = false;
			FilterExpressionList flagFilterList = base.SharePointOptions.FlagFilterList as FilterExpressionList;
			if (sUrl != null && flagFilterList != null && flagFilterList.Count > 0)
			{
				flag = base.SharePointOptions.FlagFilterList.Evaluate(sUrl);
			}
			return flag;
		}

		private bool IsValidUrl(string sUrl, out string sMessage)
		{
			bool flag = false;
			sMessage = "";
			if (string.IsNullOrEmpty(sUrl))
			{
				return false;
			}
			HttpWebResponse response = null;
			try
			{
				try
				{
					WebRequest mCredentials = WebRequest.Create(sUrl);
					mCredentials.Credentials = this.m_Credentials;
					mCredentials.Timeout = base.SharePointOptions.PageResponseTimeout;
					response = (HttpWebResponse)((HttpWebRequest)mCredentials).GetResponse();
					flag = true;
				}
				catch (WebException webException1)
				{
					WebException webException = webException1;
					if (webException.Status != WebExceptionStatus.ProtocolError || !(((HttpWebResponse)webException.Response).StatusCode.ToString() != "NotFound"))
					{
						flag = false;
						sMessage = webException.Status.ToString();
					}
					else
					{
						flag = true;
						sMessage = string.Concat("Warning: ", ((HttpWebResponse)webException.Response).StatusCode);
					}
				}
				catch (UriFormatException uriFormatException)
				{
					flag = false;
					sMessage = "Invalid Format";
				}
				catch
				{
					flag = false;
					sMessage = "Unsupported Protocol";
				}
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source == null)
			{
				base.FireSourceLinkChanged((target == null ? "" : target.ToString()));
				base.FireTargetLinkChanged("");
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			CheckLinksResults checkLinksResult = this.CheckLinks(target, xmlTextWriter);
			xmlTextWriter.Close();
			if (base.CheckForAbort())
			{
				return;
			}
			try
			{
				this.ResultsFile = this.WriteOutResultsToDisk(stringBuilder.ToString(), checkLinksResult);
				LogItem logItem = new LogItem("Results File Created", "", "", this.ResultsFile, ActionOperationStatus.Completed);
				base.FireOperationStarted(logItem);
				logItem.Information = string.Concat("The location of the results XML is: ", this.ResultsFile);
				base.FireTargetLinkChanged(this.ResultsFile);
				base.FireOperationFinished(logItem);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException("Writing Check Links Results Failed", exception.Message, exception, ErrorIcon.Error);
			}
		}

		private SPCheckLink TestLink(string sUrl, LinkType linkType, string sWebPartType, SPNode parent, string sParentUrl, string sSourcePropertyName)
		{
			string str;
			bool flag = this.IsValidUrl(sUrl, out str);
			bool flag1 = this.IsFlagged(sUrl);
			if (parent != null)
			{
				return new SPCheckLink(sUrl, linkType, sWebPartType, parent, sSourcePropertyName, flag, str, flag1);
			}
			return new SPCheckLink(sUrl, linkType, sWebPartType, sParentUrl, sSourcePropertyName, flag, str, flag1);
		}

		private void UpdateLogItem(SPCheckLink linkResult, LogItem logItem)
		{
			if (linkResult == null)
			{
				return;
			}
			if (!linkResult.IsValidLink)
			{
				logItem.AddCompletionDetail(Resources.CheckLinks_Detail_Failed, (long)1);
			}
			else
			{
				logItem.AddCompletionDetail(Resources.CheckLinks_Detail_Succeeded, (long)1);
			}
			if (linkResult.IsFlagged)
			{
				logItem.AddCompletionDetail(Resources.CheckLinks_Detail_Flagged, (long)1);
			}
		}

		private void WriteLinkList(List<SPCheckLink> links, XmlTextWriter writer)
		{
			foreach (SPCheckLink link in links)
			{
				link.WriteToXml(writer, true);
			}
		}

		private string WriteOutResultsToDisk(string sResults, CheckLinksResults linkResults)
		{
			string str = Path.Combine(ApplicationData.ApplicationPath, "CheckLinksResults");
			Directory.CreateDirectory(str);
			string str1 = Path.Combine(str, this.GenerateFilename(null));
			TextWriter streamWriter = new StreamWriter(str1, false, Encoding.Unicode);
			streamWriter.Write(sResults);
			streamWriter.Flush();
			streamWriter.Close();
			return str1;
		}

		private void WriteSummary(CheckLinksResults results, XmlTextWriter writer)
		{
			int count = results.Successes.Count;
			int num = results.Failures.Count;
			int count1 = results.FlaggedSuccesses.Count;
			int num1 = results.FlaggedFailures.Count;
			writer.WriteStartElement("Summary");
			writer.WriteAttributeString("Successes", count.ToString());
			writer.WriteAttributeString("Failures", num.ToString());
			writer.WriteAttributeString("Flagged", (count1 + num1).ToString());
			writer.WriteStartElement("Failures");
			writer.WriteAttributeString("Total", num.ToString());
			this.WriteLinkList(results.Failures, writer);
			writer.WriteEndElement();
			writer.WriteStartElement("Successes");
			writer.WriteAttributeString("Total", count.ToString());
			if (base.SharePointOptions.ShowSuccesses)
			{
				this.WriteLinkList(results.Successes, writer);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Flagged");
			writer.WriteAttributeString("Total", (count1 + num1).ToString());
			writer.WriteStartElement("Failures");
			writer.WriteAttributeString("Total", num1.ToString());
			this.WriteLinkList(results.FlaggedFailures, writer);
			writer.WriteEndElement();
			writer.WriteStartElement("Successes");
			writer.WriteAttributeString("Total", count1.ToString());
			if (base.SharePointOptions.ShowSuccesses)
			{
				this.WriteLinkList(results.FlaggedSuccesses, writer);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}