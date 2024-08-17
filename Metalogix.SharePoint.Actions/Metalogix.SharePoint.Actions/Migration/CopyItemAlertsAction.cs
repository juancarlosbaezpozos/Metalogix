using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointItemAlerts", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Alert.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Item Objects {0-Paste} > Alerts...")]
	[Name("Paste Item Alerts")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPListItem))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPListItem))]
	public class CopyItemAlertsAction : CopyAlertsAction
	{
		public CopyItemAlertsAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			if (targetSelections.Count > 0)
			{
				Type type = targetSelections[0].GetType();
				if (typeof(SPListItem).IsAssignableFrom(type))
				{
					return (!(((SPListItem)targetSelections[0]).Adapter.AdapterShortName != "NW") || !(((SPListItem)sourceSelections[0]).Adapter.AdapterShortName != "NW") || !(((SPListItem)targetSelections[0]).Adapter.AdapterShortName != "DB") || !(((SPListItem)sourceSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPListItem)sourceSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 || !(((SPListItem)targetSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPListItem)targetSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 ? false : !((SPListItem)targetSelections[0]).Adapter.SharePointVersion.IsSharePointOnline);
				}
			}
			return false;
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			return new PropertyInfo[0];
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source[0] is SPListItem && target[0] is SPListItem)
			{
				SPListItem item = (SPListItem)source[0];
				SPListItem sPListItem = (SPListItem)target[0];
				base.CopyItemAlerts(item, sPListItem, base.SharePointOptions);
			}
		}
	}
}