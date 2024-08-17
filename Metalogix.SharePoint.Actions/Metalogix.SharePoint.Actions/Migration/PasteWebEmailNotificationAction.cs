using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(false, "", new string[] {  })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.NotificationSetting.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Site Objects {0-Paste} > E-Mail Notification Setting...")]
	[Name("Paste List E-mail Notification Setting")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb), true)]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb), true)]
	public class PasteWebEmailNotificationAction : PasteAction<ListEmailNotificationOptions>
	{
		public PasteWebEmailNotificationAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			if (!(sourceSelections[0] is SPWeb))
			{
				return false;
			}
			return targetSelections[0] is SPWeb;
		}

		private void CopyNotificationsAtWebLevel(SPWeb sourceWeb, SPWeb targetWeb)
		{
			foreach (SPList list in sourceWeb.Lists)
			{
				SPList matchingList = MigrationUtils.GetMatchingList(list, targetWeb, base.SharePointOptions.RenamingTransformations);
				if (matchingList == null)
				{
					continue;
				}
				PasteListEmailNotificationAction pasteListEmailNotificationAction = new PasteListEmailNotificationAction()
				{
					SharePointOptions = base.SharePointOptions
				};
				base.SubActions.Add(pasteListEmailNotificationAction);
				object[] objArray = new object[] { list, matchingList };
				pasteListEmailNotificationAction.RunAsSubAction(objArray, new ActionContext(list, matchingList), null);
			}
			if (base.SharePointOptions.Recursive)
			{
				foreach (SPWeb subWeb in sourceWeb.SubWebs)
				{
					SPWeb matchingWeb = MigrationUtils.GetMatchingWeb(subWeb, targetWeb, base.SharePointOptions.RenamingTransformations);
					if (matchingWeb == null)
					{
						continue;
					}
					this.CopyNotificationsAtWebLevel(subWeb, matchingWeb);
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPWeb item = (SPWeb)source[0];
			this.CopyNotificationsAtWebLevel(item, (SPWeb)target[0]);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new ArgumentException(string.Format(Resources.ActionOperationMissingParameter, this.Name));
			}
			this.CopyNotificationsAtWebLevel(oParams[0] as SPWeb, oParams[1] as SPWeb);
		}
	}
}