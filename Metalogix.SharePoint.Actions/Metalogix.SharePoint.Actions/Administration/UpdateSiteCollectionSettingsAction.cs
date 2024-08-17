using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(true, "Update-SharePointSiteCollectionSettings", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.UpdateAdminsAndQuotas.ico")]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMWebComponents)]
	[MenuText("Change Site Collection Settings {3-Update} > 1:Admin and Quota Settings...")]
	[Name("Change Admin and Quota Settings")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class UpdateSiteCollectionSettingsAction : SharePointAction<UpdateSiteCollectionSettingsOptions>
	{
		public UpdateSiteCollectionSettingsAction()
		{
			base.SharePointOptions.CorrectingLinks = false;
			base.SharePointOptions.OverrideCheckouts = false;
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
					SPWeb current = (SPWeb)enumerator.Current;
					if (current.Adapter.DisplayedShortName == "CSOM")
					{
						flag = false;
						return flag;
					}
					else if (current is SPSite || current.Parent == null)
					{
						if (!current.Adapter.IsNws)
						{
							continue;
						}
						flag = false;
						return flag;
					}
					else
					{
						flag = false;
						return flag;
					}
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

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			ActionOperationStatus actionOperationStatu;
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("Site");
			if (base.SharePointOptions.SetSiteQuota)
			{
				if (base.SharePointOptions.QuotaID != null)
				{
					xmlTextWriter.WriteAttributeString("QuotaID", base.SharePointOptions.QuotaID);
				}
				long quotaMaximum = base.SharePointOptions.QuotaMaximum * (long)1048576;
				xmlTextWriter.WriteAttributeString("QuotaStorageLimit", quotaMaximum.ToString());
				long quotaWarning = base.SharePointOptions.QuotaWarning * (long)1048576;
				xmlTextWriter.WriteAttributeString("QuotaStorageWarning", quotaWarning.ToString());
			}
			if (base.SharePointOptions.SetSiteCollectionAdmins)
			{
				xmlTextWriter.WriteStartElement("SiteAdmins");
				string[] siteCollectionAdminsList = base.SharePointOptions.SiteCollectionAdminsList;
				for (int i = 0; i < (int)siteCollectionAdminsList.Length; i++)
				{
					string str = siteCollectionAdminsList[i];
					xmlTextWriter.WriteStartElement("SiteAdmin");
					xmlTextWriter.WriteAttributeString("LoginName", str.ToUpper());
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			UpdateSiteCollectionOptions updateSiteCollectionOption = new UpdateSiteCollectionOptions()
			{
				UpdateSiteQuota = base.SharePointOptions.SetSiteQuota,
				UpdateSiteAdmins = base.SharePointOptions.SetSiteCollectionAdmins
			};
			foreach (SPWeb sPWeb in target)
			{
				LogItem logItem = new LogItem("Updating Site Collection Settings", sPWeb.Name, null, sPWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					OperationReportingResult operationReportingResult = sPWeb.UpdateSiteCollectionSettings(stringBuilder.ToString(), updateSiteCollectionOption);
					if (operationReportingResult == null)
					{
						logItem.Status = ActionOperationStatus.Completed;
					}
					else
					{
						logItem.Details = operationReportingResult.AllReportElementsAsString;
						LogItem logItem1 = logItem;
						if (operationReportingResult.ErrorOccured)
						{
							actionOperationStatu = ActionOperationStatus.Failed;
						}
						else
						{
							actionOperationStatu = (operationReportingResult.WarningOccured ? ActionOperationStatus.Warning : ActionOperationStatus.Completed);
						}
						logItem1.Status = actionOperationStatu;
					}
					if (logItem.Status != ActionOperationStatus.Completed)
					{
						logItem.Information = "Issues have been encountered, please review details.";
					}
				}
				catch (Exception exception)
				{
					logItem.Exception = exception;
				}
				base.FireOperationFinished(logItem);
			}
		}
	}
}