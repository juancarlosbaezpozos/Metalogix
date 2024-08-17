using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Name("Paste Access Request List")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[SupportsThreeStateConfiguration(true)]
	[UsesStickySettings(true)]
	public class CopyAccessRequestListAction : PasteAction<PasteListItemOptions>
	{
		private const string ACCESS_REQUEST_LIST_URL = "{0}/pendingreq.aspx";

		public CopyAccessRequestListAction()
		{
		}

		private void CopyAccessRequestList(SPList sourceAccessList, SPWeb targetWeb)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			string xML = targetWeb.XML;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xML);
			bool attributeValueAsBoolean = xmlDocument.DocumentElement.GetAttributeValueAsBoolean("RequestAccessEnabled");
			if (sourceAccessList != null && attributeValueAsBoolean)
			{
				LogItem logItem = null;
				try
				{
					try
					{
						string str = string.Format("{0}/pendingreq.aspx", sourceAccessList.Url);
						logItem = new LogItem("Copying Access Request list", "Access Request", str, targetWeb.Url, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						AddListOptions addListOption = new AddListOptions()
						{
							CopyFields = false,
							CopyViews = false,
							Overwrite = false,
							DeletePreExistingViews = false
						};
						targetWeb.Adapter.Writer.AddList(sourceAccessList.XML, addListOption, new byte[0]);
						logItem.Status = ActionOperationStatus.Completed;
						logItem.AddCompletionDetail(Resources.Migration_Detail_ListsCopied, (long)1);
						if (!AdapterConfigurationVariables.EnableAccessRequestItemsNotifications)
						{
							string str1 = string.Format("<Web RequestAccessEmail=\"{0}\"/>", AdapterConfigurationVariables.NotificationEmailAddress);
							this.UpdateWebAccessRequestSettings(str1, targetWeb);
						}
						this.CopyRequestItems(sourceAccessList, targetWeb.AccessRequestList);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem != null)
						{
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Exception = exception;
						}
					}
				}
				finally
				{
					if (!AdapterConfigurationVariables.EnableAccessRequestItemsNotifications)
					{
						this.UpdateWebAccessRequestSettings(sourceAccessList.ParentWeb.XML, targetWeb);
					}
					base.FireOperationFinished(logItem);
				}
			}
		}

		private void CopyRequestItems(SPList sourceList, SPList targetList)
		{
			AddListItemOptions addListItemOption = new AddListItemOptions()
			{
				AllowDBWriting = false,
				Overwrite = false,
				PreserveID = false
			};
			LogItem logItem = null;
			string str = string.Format("{0}/pendingreq.aspx", targetList.Url);
			string str1 = string.Format("{0}/pendingreq.aspx", sourceList.Url);
			foreach (SPListItem allItem in sourceList.AllItems)
			{
				try
				{
					try
					{
						if (!base.CheckForAbort())
						{
							logItem = new LogItem("Copying Item", allItem.Title, str1, str, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(allItem.XML);
							if (!xmlDocument.DocumentElement.Attributes.GetAttributeValueAsBoolean("IsInvitation"))
							{
								string str2 = this.MapRequestObject(allItem, sourceList, targetList);
								string str3 = targetList.Adapter.Writer.AddListItem(targetList.ID, "", str2, null, null, string.Empty, addListItemOption);
								if (!string.IsNullOrEmpty(str3))
								{
									OperationReportingResult operationReportingResult = new OperationReportingResult(str3);
									if (operationReportingResult.ErrorOccured)
									{
										throw new OperationReportingException(operationReportingResult.GetMessageOfFirstErrorElement, operationReportingResult.AllReportElementsAsString);
									}
									if (operationReportingResult.HasInformation)
									{
										logItem.Information = operationReportingResult.AllInformationAsString;
										logItem.Status = ActionOperationStatus.Skipped;
										continue;
									}
								}
								logItem.Status = ActionOperationStatus.Completed;
								logItem.AddCompletionDetail(Resources.Migration_Detail_ItemsCopied, (long)1);
							}
							else
							{
								logItem.Information = string.Format("Skipping Item '{0}' as it is an Invitation.", allItem.Title);
								logItem.Status = ActionOperationStatus.Skipped;
								continue;
							}
						}
						else
						{
							return;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem != null)
						{
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Exception = exception;
						}
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
			}
		}

		private string MapRequestObject(SPListItem sourceItem, SPList sourceList, SPList targetList)
		{
			base.EnsurePrincipalExistence(sourceItem.GetReferencedPrincipals(), targetList.ParentWeb);
			SPFieldCollection fields = (SPFieldCollection)sourceItem.ParentList.Fields;
			string[] strArrays = new string[] { "User", "UserMulti" };
			SPFieldCollection fieldsOfTypes = fields.GetFieldsOfTypes(strArrays);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sourceItem.XMLWithVersions);
			XmlNode documentElement = xmlDocument.DocumentElement;
			XmlNodeList xmlNodeLists = documentElement.SelectNodes("./Versions/ListItem");
			if (xmlNodeLists != null && xmlNodeLists.Count > 0)
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					this.MapUsers(sourceItem, xmlNodes, fieldsOfTypes);
					this.MapRequestObjectUrl(xmlNodes);
					this.ReplaceGroupIdWithGroupName(sourceList, xmlNodes);
				}
			}
			this.MapUsers(sourceItem, documentElement, fieldsOfTypes);
			this.MapRequestObjectUrl(documentElement);
			this.ReplaceGroupIdWithGroupName(sourceList, documentElement);
			return documentElement.OuterXml;
		}

		private void MapRequestObjectUrl(XmlNode itemNode)
		{
			string attributeValueAsString = itemNode.Attributes.GetAttributeValueAsString("RequestedObjectUrl");
			string[] strArrays = attributeValueAsString.Split(new char[] { ',' });
			if ((int)strArrays.Length > 0 && !string.IsNullOrEmpty(strArrays[0]))
			{
				itemNode.Attributes["RequestedObjectUrl"].Value = HttpUtility.UrlDecode(base.LinkCorrector.CorrectUrl(strArrays[0]));
			}
		}

		private void MapUsers(SPListItem sourceItem, XmlNode itemNode, SPFieldCollection userFields)
		{
			BaseListItemDataUpdater<CopyAccessRequestListAction, PasteListItemOptions>.MapUserFields(base.PrincipalMappings, itemNode, userFields);
		}

		private void ReplaceGroupIdWithGroupName(SPList sourceList, XmlNode itemNode)
		{
			string attributeValueAsString = itemNode.Attributes.GetAttributeValueAsString("PermissionType");
			if (!string.IsNullOrEmpty(attributeValueAsString) && attributeValueAsString.Equals("SharePoint Group", StringComparison.InvariantCultureIgnoreCase))
			{
				string str = itemNode.Attributes.GetAttributeValueAsString("PermissionLevelRequested");
				if (!string.IsNullOrEmpty(str))
				{
					SPGroup groupByID = sourceList.ParentWeb.Groups.GetGroupByID(str);
					if (groupByID != null)
					{
						itemNode.Attributes["PermissionLevelRequested"].Value = groupByID.Name;
					}
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					Node[] nodeArray = new Node[] { sPWeb };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					try
					{
						this.CopyAccessRequestList(item.AccessRequestList, sPWeb);
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
			SPWeb sPWeb = oParams[0] as SPWeb;
			SPWeb sPWeb1 = oParams[1] as SPWeb;
			this.CopyAccessRequestList(sPWeb.AccessRequestList, sPWeb1);
		}

		private void UpdateWebAccessRequestSettings(string webXml, SPWeb targetWeb)
		{
			UpdateWebOptions updateWebOption = new UpdateWebOptions()
			{
				CopyAssociatedGroupSettings = false,
				CopyAccessRequestSettings = true
			};
			targetWeb.Adapter.Writer.UpdateWeb(webXml, updateWebOption);
		}
	}
}