using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	internal class PasteCustomContent : PasteAction<PasteSiteOptions>
	{
		private readonly static List<string> DefaultFolderNamesToSkip;

		static PasteCustomContent()
		{
			List<string> strs = new List<string>()
			{
				"_app_bin",
				"_controltemplates",
				"_layouts",
				"_vti_bin",
				"_vti_pvt",
				"_wpresources",
				"_catalogs",
				"_private",
				"_themes",
				"app_browsers",
				"app_globalresources",
				"aspnet_client",
				"bin",
				"wpresources",
				"images",
				"lists",
				"m"
			};
			PasteCustomContent.DefaultFolderNamesToSkip = strs;
		}

		public PasteCustomContent()
		{
		}

		private SPFile ApplyUserMapping(SPFile sourceFile, SPWeb sourceWeb, SPWeb targetWeb)
		{
			SPFile sPFile = sourceFile;
			this.AttemptToEnsureReferencedUsersExistForFile(sourceFile, targetWeb);
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sourceFile.XML);
			try
			{
				this.MapItemUserFieldData(base.PrincipalMappings, xmlNode);
				sPFile = new SPFile(sourceWeb, xmlNode);
				sPFile.SetContent(sourceFile.GetContent());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem("Unexpected user mapping failure", sourceFile.Name, sourceFile.DisplayUrl, targetWeb.Url, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
			return sPFile;
		}

		private void AttemptToEnsureReferencedUsersExistForFile(SPFile sourceFile, SPWeb targetWeb)
		{
			try
			{
				base.EnsurePrincipalExistence(sourceFile.GetReferencedPrincipals(), targetWeb);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogItem logItem = new LogItem(string.Concat("Unable to ensure referenced users: ", exception.Message), sourceFile.Name, sourceFile.WebRelativeUrl, targetWeb.Url, ActionOperationStatus.Failed)
				{
					Exception = exception
				};
				base.FireOperationStarted(logItem);
			}
		}

		private void CopyFolder(SPFolderBasic srcFolder, SPFolderBasic trgFolder)
		{
			if (srcFolder.WebRelativeUrl != null && PasteCustomContent.DefaultFolderNamesToSkip.Contains(srcFolder.WebRelativeUrl.ToLower()))
			{
				return;
			}
			foreach (SPFolderBasic subFolder in srcFolder.SubFolders)
			{
				if (base.CheckForAbort())
				{
					break;
				}
				if (PasteCustomContent.DefaultFolderNamesToSkip.Contains(subFolder.Name.ToLower()) || subFolder.ParentListId != Guid.Empty)
				{
					continue;
				}
				LogItem logItem = new LogItem("Create folder", subFolder.Name, subFolder.DisplayUrl, trgFolder.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				if (!base.SharePointOptions.FilterCustomFolders || base.SharePointOptions.CustomFolderFilterExpression.Evaluate(subFolder))
				{
					SPFolderBasic sPFolderBasic = trgFolder.SubFolders.Find((SPFolderBasic o) => o.Name == subFolder.Name);
					try
					{
						if (sPFolderBasic != null)
						{
							logItem.Information = "Target already exists";
							logItem.Status = ActionOperationStatus.Skipped;
						}
						else
						{
							StringBuilder stringBuilder = new StringBuilder();
							XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
							xmlTextWriter.WriteStartElement("Folder");
							xmlTextWriter.WriteAttributeString("Name", subFolder.Name);
							xmlTextWriter.WriteAttributeString("ParentFolderPath", trgFolder.WebRelativeUrl);
							xmlTextWriter.WriteAttributeString("Url", (string.IsNullOrEmpty(trgFolder.WebRelativeUrl) ? subFolder.Name : string.Concat(trgFolder.WebRelativeUrl, "/", subFolder.Name)));
							xmlTextWriter.WriteEndElement();
							sPFolderBasic = trgFolder.SubFolders.Add(stringBuilder.ToString());
							logItem.Status = ActionOperationStatus.Completed;
						}
						base.FireOperationFinished(logItem);
						this.CopyFolder(subFolder, sPFolderBasic);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						logItem.SourceContent = subFolder.XML;
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Exception = exception;
						base.FireOperationFinished(logItem);
					}
				}
				else
				{
					logItem.Information = "Filtered out by Custom Folders Filter";
					logItem.Status = ActionOperationStatus.Skipped;
					base.FireOperationFinished(logItem);
				}
			}
			if (base.CheckForAbort())
			{
				return;
			}
			foreach (SPFile file in srcFolder.Files)
			{
				if (!this.ActionOptions.CopyUncustomizedFiles && file.CustomizedPageStatus == SPCustomizedPageStatus.Uncustomized)
				{
					continue;
				}
				if (base.CheckForAbort())
				{
					break;
				}
				LogItem xML = new LogItem("Copy file", file.Name, file.DisplayUrl, trgFolder.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(xML);
				if (!base.SharePointOptions.FilterCustomFiles || base.SharePointOptions.CustomFileFilterExpression.Evaluate(file))
				{
					SPFile sPFile = trgFolder.Files.Find((SPFile o) => o.Name == file.Name);
					if (sPFile != null && (!base.SharePointOptions.OverwriteSites || base.SharePointOptions.UpdateSites))
					{
						if (file.TimeLastModified < sPFile.TimeLastModified)
						{
							xML.Information = "Target is newer than source";
							xML.Status = ActionOperationStatus.Skipped;
							base.FireOperationFinished(xML);
							continue;
						}
						else if (!base.SharePointOptions.UpdateSites)
						{
							xML.Information = "Target already exists";
							xML.Status = ActionOperationStatus.Skipped;
							base.FireOperationFinished(xML);
							continue;
						}
					}
					try
					{
						try
						{
							SPFile sPFile1 = this.ApplyUserMapping(file, srcFolder.Web, trgFolder.Web);
							trgFolder.Files.Add(sPFile1, sPFile1.GetContent());
							xML.Status = ActionOperationStatus.Completed;
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							xML.SourceContent = file.XML;
							xML.Status = ActionOperationStatus.Failed;
							xML.Exception = exception2;
						}
					}
					finally
					{
						base.FireOperationFinished(xML);
					}
				}
				else
				{
					xML.Information = "Filtered out by Custom Files Filter";
					xML.Status = ActionOperationStatus.Skipped;
					base.FireOperationFinished(xML);
				}
			}
		}

		private void MapItemUserFieldData(IDictionary<string, string> principalMappings, XmlNode itemXML)
		{
			if (itemXML.Attributes["Author"] != null)
			{
				itemXML.Attributes["Author"].Value = MigrationUtils.MapPrincipals(principalMappings, itemXML.Attributes["Author"].Value);
			}
			if (itemXML.Attributes["ModifiedBy"] != null)
			{
				itemXML.Attributes["ModifiedBy"].Value = MigrationUtils.MapPrincipals(principalMappings, itemXML.Attributes["ModifiedBy"].Value);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}

		protected override void RunOperation(object[] oParams)
		{
			this.CopyFolder((oParams[0] as SPWeb).RootFolder, (oParams[1] as SPWeb).RootFolder);
		}
	}
}