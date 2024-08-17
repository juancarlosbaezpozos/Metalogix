using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Name("Paste Web Parts Page")]
	[ShowInMenus(false)]
	[SubActionTypes(typeof(CopyWebPartsAction))]
	public class CopyWebPartPageAction : PasteAction<WebPartOptions>
	{
		private string[] mySiteSPOTemplateNames = new string[] { "SPSPERS#8", "SPSPERS#2", "SPSPERS#3", "SPSPERS#6", "SPSPERS#7", "SPSPERS#8", "SPSPERS#9", "SPSPERS#7", "SPSPERS#5" };

		public CopyWebPartPageAction()
		{
		}

		private void CopyDefaultWebPartPage(SPWeb sourceWeb, SPWeb targetWeb)
		{
			SPWebPartPage welcomePage;
			if (!base.SharePointOptions.CopySiteWebParts)
			{
				return;
			}
			if (sourceWeb == null)
			{
				throw new ArgumentNullException("sourceWeb", "Could not access the source default.aspx page because the source web was unspecified.");
			}
			if (targetWeb == null)
			{
				throw new ArgumentNullException("targetWeb", "Target web was unspecified for copying the default web part page.");
			}
			try
			{
				welcomePage = (new SPWebPartPage()).GetWelcomePage(sourceWeb, this);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Cannot retrieve web part page from source", sourceWeb.Name, (sourceWeb.WelcomePageUrl != null ? sourceWeb.WelcomePageUrl : sourceWeb.DisplayUrl), targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				logItem.Exception = exception;
				base.FireOperationFinished(logItem);
				return;
			}
			if (!base.CheckForAbort())
			{
				if (welcomePage != null)
				{
					LogItem xml = new LogItem("Copying web parts on default page", welcomePage.FileLeafRef, welcomePage.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
					base.FireOperationStarted(xml);
					try
					{
						try
						{
							SPWebPartPage sPWebPartPage = (new SPWebPartPage()).GetWelcomePage(targetWeb, this);
							if (sPWebPartPage == null)
							{
								throw new Exception(string.Concat("Could not retrieve default web part page on target web: ", targetWeb.Url));
							}
							if (!this.IsWebPartMigrationSkipped())
							{
								if (!targetWeb.Adapter.SharePointVersion.IsSharePoint2016 || !targetWeb.IsMySiteTemplate || !sPWebPartPage.DisplayUrl.EndsWith("/Documents/Forms/All.aspx", StringComparison.Ordinal))
								{
									CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction()
									{
										SharePointOptions = base.SharePointOptions
									};
									base.SubActions.Add(copyWebPartsAction);
									object[] objArray = new object[] { welcomePage, sPWebPartPage, xml };
									copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(welcomePage.ParentWeb, sPWebPartPage.ParentWeb), null);
								}
								else
								{
									xml.Information = string.Format("Skipped migration of web parts page on page: {0}.", targetWeb.Url);
									xml.Status = ActionOperationStatus.Skipped;
								}
								if (xml.Status != ActionOperationStatus.Failed)
								{
									xml.Status = ActionOperationStatus.Completed;
								}
							}
							else
							{
								xml.Information = "Skipped migration of web parts because item already exists at target.";
								xml.Status = ActionOperationStatus.Skipped;
							}
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							xml.Exception = exception2;
							xml.SourceContent = welcomePage.Xml;
							if (targetWeb.Template.Name.In<string>(this.mySiteSPOTemplateNames) && exception2.Message.Contains("WEBPART-OBJECTMOVED"))
							{
								xml.Information = string.Concat("This web part skipped because could not retrieve web part page on target site: ", targetWeb.Url);
								xml.Status = ActionOperationStatus.Skipped;
							}
						}
					}
					finally
					{
						base.FireOperationFinished(xml);
					}
				}
				return;
			}
		}

		private void CopyWebPartPageRecursive(SPWeb sourceWeb, SPWeb targetWeb)
		{
			this.CopyWebPartsOnWeb(sourceWeb, targetWeb);
			foreach (SPWeb subWeb in sourceWeb.SubWebs)
			{
				if (!base.CheckForAbort())
				{
					SPWeb item = (SPWeb)targetWeb.SubWebs[subWeb.Name];
					if (item != null)
					{
						this.CopyWebPartPageRecursive(subWeb, item);
					}
					else
					{
						LogItem logItem = new LogItem("Copying WebParts", subWeb.Name, subWeb.DisplayUrl, null, ActionOperationStatus.MissingOnTarget)
						{
							Information = string.Concat("The site: '", subWeb.Name, "' does not exist on the target")
						};
					}
				}
				else
				{
					return;
				}
			}
		}

		private void CopyWebPartPages(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!base.SharePointOptions.CopySiteWebParts)
			{
				return;
			}
			if (sourceWeb == null)
			{
				throw new ArgumentNullException("sourceWeb", "Could not access the source web part pages because the source web was unspecified.");
			}
			if (targetWeb == null)
			{
				throw new ArgumentNullException("targetWeb", "Target web was unspecified for copying the web part page.");
			}
			SPWebPartPage webPartPage = null;
			SPWebPartPage sPWebPartPage = null;
			SPWebPartPage welcomePage = null;
			try
			{
				welcomePage = (new SPWebPartPage()).GetWelcomePage(sourceWeb, this);
			}
			catch (Exception exception)
			{
			}
			foreach (SPFile file in sourceWeb.RootFolder.Files)
			{
				if (!base.CheckForAbort())
				{
					if (!file.Name.Trim().ToLower().EndsWith(".aspx") || welcomePage != null && file.Name.ToLower() == welcomePage.FileLeafRef)
					{
						continue;
					}
					string serverRelative = StandardizedUrl.StandardizeUrl(sourceWeb.Adapter, file.WebRelativeUrl).ServerRelative;
					if (!SPWebPartPage.IsWebPartPage(sourceWeb.Adapter, serverRelative))
					{
						continue;
					}
					webPartPage = (new SPWebPartPage()).GetWebPartPage(sourceWeb, serverRelative, this);
					if (webPartPage == null)
					{
						throw new Exception(string.Concat("Could not retrieve web part page ", file.Name, " on source web: ", sourceWeb.Url));
					}
					LogItem logItem = null;
					try
					{
						try
						{
							logItem = new LogItem(string.Concat("Copying web parts on ", file.Name, " page"), webPartPage.FileLeafRef, webPartPage.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							SPWebPartPage sPWebPartPage1 = new SPWebPartPage();
							string serverRelativeUrl = targetWeb.ServerRelativeUrl;
							char[] chrArray = new char[] { '/' };
							sPWebPartPage = sPWebPartPage1.GetWebPartPage(targetWeb, string.Concat(serverRelativeUrl.TrimEnd(chrArray), "/", file.Name), this);
							if (sPWebPartPage == null)
							{
								throw new Exception(string.Concat("Could not retrieve web part page '", file.Name, "' on target web: ", targetWeb.Url));
							}
							CopyWebPartsAction copyWebPartsAction = new CopyWebPartsAction()
							{
								LinkCorrector = base.LinkCorrector,
								SharePointOptions = base.SharePointOptions
							};
							base.SubActions.Add(copyWebPartsAction);
							object[] objArray = new object[] { webPartPage, sPWebPartPage, logItem };
							copyWebPartsAction.RunAsSubAction(objArray, new ActionContext(webPartPage.ParentWeb, sPWebPartPage.ParentWeb), null);
							if (logItem.Status != ActionOperationStatus.Failed)
							{
								logItem.Status = ActionOperationStatus.Completed;
							}
						}
						catch (Exception exception2)
						{
							Exception exception1 = exception2;
							logItem.Exception = exception1;
							logItem.SourceContent = webPartPage.Xml;
							if (webPartPage == null)
							{
								logItem.Information = string.Concat("Could not retrieve web part page '", file.Name, "' on source web: ", sourceWeb.Url);
							}
							else if (sPWebPartPage == null)
							{
								logItem.Information = string.Concat("Could not retrieve web part page '", file.Name, "' on target web: ", targetWeb.Url);
							}
							if (targetWeb.Template.Name.In<string>(this.mySiteSPOTemplateNames) && exception1.Message.Contains("WEBPART-OBJECTMOVED"))
							{
								logItem.Information = string.Concat("This web part skipped because could not retrieve web part page '", file.Name, "' on target site: ", targetWeb.Url);
								logItem.Status = ActionOperationStatus.Skipped;
							}
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
				else
				{
					return;
				}
			}
		}

		private void CopyWebPartsOnWeb(SPWeb sourceWeb, SPWeb targetWeb)
		{
			if (!base.CheckForAbort())
			{
				this.CopyWebPartPages(sourceWeb, targetWeb);
				this.CopyDefaultWebPartPage(sourceWeb, targetWeb);
			}
			sourceWeb.Collapse();
			targetWeb.Collapse();
		}

		private bool IsWebPartMigrationSkipped()
		{
			if (base.SharePointOptions.MigrationMode == MigrationMode.Incremental)
			{
				return true;
			}
			if (base.SharePointOptions.MigrationMode != MigrationMode.Custom)
			{
				return false;
			}
			return !base.SharePointOptions.OverwriteSites;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, this.ActionOptions.ForceRefresh);
			SPWeb item = source[0] as SPWeb;
			SPWeb sPWeb = target[0] as SPWeb;
			if (item != null && sPWeb != null)
			{
				this.CopyDefaultWebPartPage(item, sPWeb);
				item.Dispose();
				sPWeb.Dispose();
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			bool copyWebPartsRecursive = base.SharePointOptions.CopyWebPartsRecursive;
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			if (copyWebPartsRecursive)
			{
				this.CopyWebPartPageRecursive(sPWeb, sPWeb1);
				return;
			}
			this.CopyWebPartsOnWeb(sPWeb, sPWeb1);
		}
	}
}