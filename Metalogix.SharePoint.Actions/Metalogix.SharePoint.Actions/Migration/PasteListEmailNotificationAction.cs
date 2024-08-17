using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(false, null, null)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.NotificationSetting.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste List Objects {0-Paste} > E-Mail Notification Setting...")]
	[Name("Paste List E-mail Notification Setting")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPList), true)]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPList), true)]
	public class PasteListEmailNotificationAction : PasteAction<ListEmailNotificationOptions>
	{
		public PasteListEmailNotificationAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			if (!(sourceSelections[0] is SPList))
			{
				return false;
			}
			return targetSelections[0] is SPList;
		}

		private void CopyNotificationSetting(SPList sourceList, SPList targetList)
		{
			LogItem logItem = new LogItem("Copying List E-mail Notification Setting", sourceList.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				try
				{
					if (sourceList.EnableAssignToEmail.HasValue)
					{
						bool? enableAssignToEmail = sourceList.EnableAssignToEmail;
						bool? nullable = targetList.EnableAssignToEmail;
						if ((enableAssignToEmail.GetValueOrDefault() != nullable.GetValueOrDefault() ? false : enableAssignToEmail.HasValue == nullable.HasValue))
						{
							logItem.Information = "Copying this setting was skipped because the source and target list already have the same value for this setting.";
							logItem.Status = ActionOperationStatus.Skipped;
						}
						else
						{
							string str = string.Format("<SPList EnableAssignToEmail='{0}'/>", (sourceList.EnableAssignToEmail.Value ? "True" : "False"));
							targetList.UpdateList(str, null, false, false, true);
							logItem.Status = ActionOperationStatus.Completed;
						}
					}
					else if (sourceList.BaseTemplate == ListTemplateType.Issues)
					{
						logItem.Information = "This setting could not be copied because the value of the setting on the source list could not be retrieved.";
						logItem.Status = ActionOperationStatus.Warning;
					}
					else if (!sourceList.Adapter.SharePointVersion.IsSharePoint2007OrLater || sourceList.BaseTemplate != ListTemplateType.Tasks && sourceList.BaseTemplate != ListTemplateType.TasksWithTimelineAndHierarchy && sourceList.BaseTemplate != ListTemplateType.ProjectTasks)
					{
						logItem.Information = "Copying this setting was skipped because the value of the setting on the source list could not be retrieved. However, this list type does not normally support this setting.";
						logItem.Status = ActionOperationStatus.Skipped;
					}
					else if (!sourceList.Adapter.IsNws)
					{
						logItem.Information = "This setting could not be copied because the value of the setting on the source list could not be retrieved.";
						logItem.Status = ActionOperationStatus.Warning;
					}
					else
					{
						logItem.Information = "The Native Web Service (NWS) adapter is not capable of retrieving this setting from task or project tasks lists. Please try another source adapter type to copy the notification setting from the source list.";
						logItem.Status = ActionOperationStatus.Skipped;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					logItem.Information = "Failure copying the list setting for allowing e-mails to be sent for assignment of item ownership.";
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
			SPList item = (SPList)source[0];
			this.CopyNotificationSetting(item, (SPList)target[0]);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new ArgumentException(string.Format(Resources.ActionOperationMissingParameter, this.Name));
			}
			this.CopyNotificationSetting(oParams[0] as SPList, oParams[1] as SPList);
		}
	}
}