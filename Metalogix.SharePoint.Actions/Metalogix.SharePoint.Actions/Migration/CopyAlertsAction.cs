using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Office365;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[SubActionTypes(typeof(CopyUsersAction))]
	[UsesStickySettings(false)]
	public abstract class CopyAlertsAction : PasteAction<AlertOptions>
	{
		private SPOptimizationNode m_optimizationTree;

		protected bool HasPermissionsOptimization
		{
			get
			{
				return this.m_optimizationTree != null;
			}
		}

		public SPOptimizationNode OptimizationTree
		{
			get
			{
				return this.m_optimizationTree;
			}
		}

		protected CopyAlertsAction()
		{
		}

		private void CopyAlertProperties(SPAlert alert, ManifestAlert manifest)
		{
			foreach (KeyValuePair<string, string> property in alert.Properties)
			{
				if (property.Key.IndexOf("url", StringComparison.OrdinalIgnoreCase) != -1)
				{
					continue;
				}
				Metalogix.Office365.Field field = new Metalogix.Office365.Field()
				{
					Name = property.Key,
					Type = "String",
					Access = "ReadWrite",
					Value = property.Value
				};
				manifest.FieldValues.Add(field);
			}
		}

		public void CopyItemAlerts(SPListItem sourceItem, SPListItem targetItem, AlertOptions alertOptions)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			SPAlertCollection alerts = sourceItem.Alerts;
			if (alerts.Count == 0)
			{
				logItem = new LogItem("Copying Item Alerts", sourceItem.Name, sourceItem.Url, targetItem.Url, ActionOperationStatus.Skipped)
				{
					Information = "The source item does not have any alerts."
				};
				base.FireOperationStarted(logItem);
				logItem.Status = ActionOperationStatus.Skipped;
				base.FireOperationFinished(logItem);
			}
			else
			{
				foreach (SPAlert alert in alerts)
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(alert.XML);
					xmlNode.Attributes["ListID"].Value = targetItem.ParentList.ID;
					xmlNode.Attributes["ItemGUID"].Value = targetItem["UniqueId"];
					if (!this.MapAlertUser(ref xmlNode, sourceItem.ParentList.ParentWeb, targetItem.ParentList.ParentWeb) || targetItem.Alerts.ContainsAlertWithXML(xmlNode))
					{
						continue;
					}
					try
					{
						try
						{
							logItem = new LogItem("Adding Item Alert", alert.Title, sourceItem.DisplayUrl, targetItem.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							string str = targetItem.Alerts.AddAlert(targetItem.ParentList.ParentWeb.ServerRelativeUrl, targetItem.ParentList.ParentWeb.Url, xmlNode);
							if (str == "User can't be added.")
							{
								logItem.Status = ActionOperationStatus.Failed;
								logItem.Information = string.Concat("The user ", alert.User, " did not exist on the target and could not be added.");
							}
							else if (str != null)
							{
								logItem.Status = ActionOperationStatus.Completed;
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							logItem.Exception = exception;
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = string.Concat("Exception thrown: ", exception.Message);
							logItem.Details = exception.StackTrace;
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
			}
			sourceItem.Dispose();
			targetItem.Dispose();
		}

		public void CopyListAlerts(SPList sourceList, SPList targetList, AlertOptions alertOptions)
		{
			SPAlertCollection alerts;
			SPOptimizationNode sPOptimizationNode;
			SPOptimizationNode sPOptimizationNode1;
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			if (this.HasPermissionsOptimization)
			{
				sPOptimizationNode = this.OptimizationTree.Find(sourceList.ServerRelativeUrl);
			}
			else
			{
				sPOptimizationNode = null;
			}
			SPOptimizationNode sPOptimizationNode2 = sPOptimizationNode;
			if (!this.HasPermissionsOptimization || sPOptimizationNode2 != null)
			{
				alerts = sourceList.Alerts;
			}
			else
			{
				alerts = null;
			}
			if (alerts == null || alerts.Count < 0)
			{
				logItem = new LogItem("Copying List Alerts", sourceList.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running)
				{
					Information = "The source list does not have any alerts."
				};
				base.FireOperationStarted(logItem);
				logItem.Status = ActionOperationStatus.Skipped;
				base.FireOperationFinished(logItem);
			}
			else
			{
				if (!this.HasPermissionsOptimization || sPOptimizationNode2 != null && sPOptimizationNode2.HasUniqueValues)
				{
					foreach (SPAlert alert in alerts)
					{
						if (alert.AlertType == Metalogix.SharePoint.SPAlertType.Item)
						{
							continue;
						}
						XmlNode xmlNode = XmlUtility.StringToXmlNode(alert.XML);
						xmlNode.Attributes["ListID"].Value = targetList.ID;
						if (!this.MapAlertUser(ref xmlNode, sourceList.ParentWeb, targetList.ParentWeb) || targetList.Alerts.ContainsAlertWithXML(xmlNode))
						{
							continue;
						}
						try
						{
							try
							{
								logItem = new LogItem("Adding List Alert", alert.Title, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
								base.FireOperationStarted(logItem);
								string str = targetList.Alerts.AddAlert(targetList.ParentWeb.ServerRelativeUrl, targetList.ParentWeb.Url, xmlNode);
								if (str == "User can't be added.")
								{
									logItem.Status = ActionOperationStatus.Failed;
									logItem.Information = string.Format("The user {0} did not exist on the target and could not be added.", alert.User);
								}
								else if (str != null)
								{
									logItem.Status = ActionOperationStatus.Completed;
								}
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								logItem.Exception = exception;
								logItem.Status = ActionOperationStatus.Failed;
								logItem.Information = string.Concat("Exception thrown: ", exception.Message);
								logItem.Details = exception.StackTrace;
							}
						}
						finally
						{
							base.FireOperationFinished(logItem);
						}
					}
				}
				if (alertOptions.CopyItemAlerts && (!this.HasPermissionsOptimization || sPOptimizationNode2 != null && sPOptimizationNode2.Children.HasUniqueValuesAtThisLevel))
				{
					sourceList.GetTerseItems(true, ListItemQueryType.ListItem | ListItemQueryType.Folder, null, new GetListItemOptions());
					SPListItemCollection terseItems = null;
					foreach (SPListItem item in sourceList.Items)
					{
						if (!base.CheckForAbort())
						{
							if (this.HasPermissionsOptimization)
							{
								sPOptimizationNode1 = this.OptimizationTree.Find(item.ServerRelativeFolderLeafRef);
							}
							else
							{
								sPOptimizationNode1 = null;
							}
							SPOptimizationNode sPOptimizationNode3 = sPOptimizationNode1;
							if (this.HasPermissionsOptimization && (sPOptimizationNode3 == null || !sPOptimizationNode3.HasUniqueValues))
							{
								continue;
							}
							if (terseItems == null)
							{
								terseItems = targetList.GetTerseItems(true, ListItemQueryType.ListItem | ListItemQueryType.Folder, null, new GetListItemOptions());
							}
							SPListItem sPListItem = (SPListItem)terseItems[item.Name];
							if (sPListItem == null)
							{
								logItem = new LogItem("Skipping Alert Copying", item.Name, item.Url, targetList.Url, ActionOperationStatus.MissingOnTarget);
								base.FireOperationStarted(logItem);
								logItem.Information = "The source item does not exist on the target. Please copy the item before copying alerts.";
								logItem.Status = ActionOperationStatus.MissingOnTarget;
								base.FireOperationFinished(logItem);
							}
							else
							{
								this.CopyItemAlerts(item, sPListItem, alertOptions);
							}
						}
						else
						{
							return;
						}
					}
				}
			}
			sourceList.Dispose();
			targetList.Dispose();
		}

		public void CopyListAlertsUsingAzure(SPList sourceList, SPList targetList, AlertOptions alertOptions)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = new LogItem("Copying List Alerts", sourceList.Title, sourceList.Title, targetList.Title, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					SPAlertCollection alerts = sourceList.Alerts;
					if (alerts == null || alerts.Count <= 0)
					{
						logItem.Information = "No alerts found.";
					}
					else
					{
						IUploadManager uploadManager = AzureUploadCreator.InitializeAzurePipeline(targetList, this, this, alertOptions.UseEncryptedAzureMigration, true);
						this.CopyReferencedUsersForList(sourceList, targetList, uploadManager);
						foreach (SPAlert alert in alerts)
						{
							if (alert.AlertType == Metalogix.SharePoint.SPAlertType.Item && !alertOptions.CopyItemAlerts)
							{
								continue;
							}
							string str = string.Format("Processing {0} Alert", alert.AlertType.ToString());
							LogItem stackTrace = new LogItem(str, alert.Title, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(stackTrace);
							try
							{
								try
								{
									ManifestAlert manifestAlert = this.CreateAlertManifest(uploadManager, targetList, alert);
									if (manifestAlert == null)
									{
										stackTrace.Status = ActionOperationStatus.Failed;
										stackTrace.Information = string.Format("The user {0} did not exist on the target and could not be added.", alert.User);
									}
									else
									{
										stackTrace.Status = ActionOperationStatus.Completed;
										uploadManager.AddAlertToManifest(manifestAlert);
									}
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									stackTrace.Exception = exception;
									stackTrace.Status = ActionOperationStatus.Failed;
									stackTrace.Information = string.Concat("Exception thrown: ", exception.Message);
									stackTrace.Details = exception.StackTrace;
								}
							}
							finally
							{
								base.FireOperationFinished(stackTrace);
							}
						}
						LogItem logItem1 = new LogItem("AzureUploadManager.StatusLog", targetList.Title, string.Empty, string.Empty, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem1);
						AzureUploadCreator.EndAzurePipeline(uploadManager, logItem1);
						base.FireOperationFinished(logItem1);
					}
					sourceList.Dispose();
					targetList.Dispose();
					logItem.Status = ActionOperationStatus.Completed;
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					logItem.Exception = exception2;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Information = string.Concat("Exception thrown: ", exception2.Message);
					logItem.Details = exception2.StackTrace;
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyReferencedUsersForList(SPList sourceList, SPList targetList, IUploadManager uploadManager)
		{
			SecurityPrincipalCollection referencedPrincipals = sourceList.GetReferencedPrincipals();
			List<SPUser> sPUsers = new List<SPUser>();
			List<SPGroup> sPGroups = new List<SPGroup>();
			foreach (SecurityPrincipal referencedPrincipal in (IEnumerable<SecurityPrincipal>)referencedPrincipals)
			{
				if (!(referencedPrincipal is SPUser))
				{
					if (!(referencedPrincipal is SPGroup))
					{
						continue;
					}
					SPGroup owner = (SPGroup)referencedPrincipal;
					sPGroups.Add(owner);
					while (owner.Owner != null && (owner.OwnerIsUser && !sPUsers.Contains((SPUser)owner.Owner) || !owner.OwnerIsUser && !sPGroups.Contains((SPGroup)owner.Owner)))
					{
						if (!owner.OwnerIsUser)
						{
							owner = (SPGroup)owner.Owner;
							sPGroups.Add(owner);
						}
						else
						{
							sPUsers.Add((SPUser)owner.Owner);
						}
					}
				}
				else
				{
					sPUsers.Add((SPUser)referencedPrincipal);
				}
			}
			base.EnsurePrincipalExistence(sPUsers.ToArray(), sPGroups.ToArray(), targetList.ParentWeb, uploadManager, null);
		}

		public void CopyWebAlerts(SPWeb sourceWeb, SPWeb targetWeb, AlertOptions alertOptions)
		{
			SPOptimizationNode sPOptimizationNode;
			SPOptimizationNode sPOptimizationNode1;
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			logItem = new LogItem("Copying Site Alerts", sourceWeb.Name, sourceWeb.Name, targetWeb.Name, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					foreach (SPList list in sourceWeb.Lists)
					{
						if (this.HasPermissionsOptimization)
						{
							sPOptimizationNode = this.OptimizationTree.Find(list.ServerRelativeUrl);
						}
						else
						{
							sPOptimizationNode = null;
						}
						if (this.HasPermissionsOptimization && sPOptimizationNode == null)
						{
							continue;
						}
						SPList item = targetWeb.Lists[list.Name];
						if (item == null)
						{
							LogItem logItem1 = new LogItem("Skipping Alert Copying", list.Name, list.Url, targetWeb.Url, ActionOperationStatus.MissingOnTarget);
							base.FireOperationStarted(logItem1);
							logItem1.Information = "The source list does not exist on the target. Please copy the list before copying alerts.";
							logItem1.Status = ActionOperationStatus.MissingOnTarget;
							base.FireOperationFinished(logItem1);
						}
						else if (!targetWeb.Adapter.SharePointVersion.IsSharePointOnline)
						{
							this.CopyListAlerts(list, item, alertOptions);
						}
						else
						{
							this.CopyListAlertsUsingAzure(list, item, alertOptions);
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Exception = exception;
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Information = string.Concat("Exception thrown: ", exception.Message);
					logItem.Details = exception.StackTrace;
				}
			}
			finally
			{
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
			}
			if (alertOptions.CopyChildSiteAlerts)
			{
				foreach (SPWeb subWeb in sourceWeb.SubWebs)
				{
					if (!base.CheckForAbort())
					{
						if (this.HasPermissionsOptimization)
						{
							sPOptimizationNode1 = this.OptimizationTree.Find(subWeb.ServerRelativeUrl);
						}
						else
						{
							sPOptimizationNode1 = null;
						}
						if (this.HasPermissionsOptimization && sPOptimizationNode1 == null)
						{
							continue;
						}
						SPWeb sPWeb = (SPWeb)targetWeb.SubWebs[subWeb.Name];
						if (sPWeb == null)
						{
							logItem = new LogItem("Skipping Alert Copying", subWeb.Name, subWeb.Url, targetWeb.Url, ActionOperationStatus.MissingOnTarget);
							base.FireOperationStarted(logItem);
							logItem.Information = "The source web does not exist on the target. Please copy the web before copying alerts.";
							logItem.Status = ActionOperationStatus.MissingOnTarget;
							base.FireOperationFinished(logItem);
						}
						else
						{
							this.CopyWebAlerts(subWeb, sPWeb, alertOptions);
						}
					}
					else
					{
						return;
					}
				}
			}
			sourceWeb.Dispose();
			targetWeb.Dispose();
		}

		private ManifestAlert CreateAlertManifest(IUploadManager uploadManager, SPList targetList, SPAlert alert)
		{
			string str = base.MapPrincipal(alert.User);
			int userOrGroupIDByName = uploadManager.GetUserOrGroupIDByName(str);
			if (userOrGroupIDByName == 0)
			{
				return null;
			}
			ManifestAlert manifestAlert = new ManifestAlert()
			{
				Title = alert.Title,
				NotifyFrequency = (SPAlertFrequency)Enum.Parse(typeof(SPAlertFrequency), alert.AlertFrequency),
				EventType = (SPEventType)Enum.Parse(typeof(SPEventType), alert.EventType),
				Status = (SPAlertStatus)Enum.Parse(typeof(SPAlertStatus), alert.Status),
				AlertTemplateName = alert.AlertTemplate,
				AlertType = (Metalogix.Office365.SPAlertType)Enum.Parse(typeof(Metalogix.SharePoint.SPAlertType), alert.AlertType.ToString())
			};
			if (!string.IsNullOrEmpty(alert.AlertTime))
			{
				manifestAlert.NotifyTime = new DateTime?(DateTime.Parse(alert.AlertTime));
			}
			if (!string.IsNullOrEmpty(alert.ItemID))
			{
				manifestAlert.ListItemIntId = new int?(int.Parse(alert.ItemID));
			}
			if (!string.IsNullOrEmpty(alert.ItemGUID))
			{
				manifestAlert.DocId = new Guid(alert.ItemGUID);
			}
			manifestAlert.UserId = userOrGroupIDByName;
			manifestAlert.Filter = this.MapUserInAlertFilter(userOrGroupIDByName, alert.Filter);
			this.CopyAlertProperties(alert, manifestAlert);
			return manifestAlert;
		}

		protected void InitializeOptimizationTable(SPNode node)
		{
			this.m_optimizationTree = node.GetAlertsOptimizationTree();
		}

		private bool MapAlertUser(ref XmlNode alertNode, SPWeb sourceWeb, SPWeb targetWeb)
		{
			string value = alertNode.Attributes["User"].Value;
			if (!base.PrincipalMappings.ContainsKey(value))
			{
				CopyUsersAction copyUsersAction = new CopyUsersAction();
				copyUsersAction.Options.SetFromOptions(this.Options);
				base.SubActions.Add(copyUsersAction);
				object[] sPUserCollection = new object[2];
				SPUser[] item = new SPUser[] { (SPUser)sourceWeb.SiteUsers[value] };
				sPUserCollection[0] = new SPUserCollection(item);
				sPUserCollection[1] = targetWeb;
				copyUsersAction.RunAsSubAction(sPUserCollection, new ActionContext(null, targetWeb), null);
			}
			SPUser sPUser = targetWeb.SiteUsers[base.PrincipalMappings[value]] as SPUser;
			if (sPUser != null)
			{
				alertNode.Attributes["User"].Value = sPUser.LoginName;
				return true;
			}
			LogItem logItem = new LogItem("Adding alerts", value, value, "", ActionOperationStatus.Skipped)
			{
				SourceContent = alertNode.OuterXml,
				Information = "Could not find user on target"
			};
			base.FireOperationStarted(logItem);
			base.FireOperationFinished(logItem);
			return false;
		}

		private string MapUserInAlertFilter(int userId, string filter)
		{
			if (string.IsNullOrEmpty(filter))
			{
				return filter;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(filter);
			XmlNode str = xmlNode.SelectSingleNode("//Value");
			XmlNode parentNode = str.ParentNode;
			parentNode.RemoveChild(str);
			str.Attributes["type"].Value = "int";
			str.InnerText = userId.ToString();
			parentNode.AppendChild(str);
			return xmlNode.OuterXml;
		}
	}
}