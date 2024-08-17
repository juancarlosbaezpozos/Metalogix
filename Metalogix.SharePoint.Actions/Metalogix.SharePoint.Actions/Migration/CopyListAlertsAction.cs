using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointListAlerts", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Alert.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste List Objects {0-Paste} > Alerts...")]
	[Name("Paste List Alerts")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPList))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPList))]
	public class CopyListAlertsAction : CopyAlertsAction
	{
		public CopyListAlertsAction()
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
				if (typeof(SPList).IsAssignableFrom(type))
				{
					return (!(((SPList)targetSelections[0]).Adapter.AdapterShortName != "NW") || !(((SPList)targetSelections[0]).Adapter.AdapterShortName != "DB") || !(((SPList)sourceSelections[0]).Adapter.AdapterShortName != "NW") || !(((SPList)sourceSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPList)sourceSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 || !(((SPList)targetSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPList)targetSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 ? false : !((SPList)sourceSelections[0]).Adapter.SharePointVersion.IsSharePointOnline);
				}
			}
			return false;
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			PropertyInfo[] property = new PropertyInfo[] { cmdletOptions.GetType().GetProperty("CopyItemAlerts", BindingFlags.Instance | BindingFlags.Public) };
			return property;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source[0] is SPList && target[0] is SPList)
			{
				SPList item = (SPList)source[0];
				SPList sPList = (SPList)target[0];
				base.InitializeOptimizationTable(item);
				if (sPList.Adapter.SharePointVersion.IsSharePointOnline)
				{
					base.CopyListAlertsUsingAzure(item, sPList, base.SharePointOptions);
					return;
				}
				base.CopyListAlerts(item, sPList, base.SharePointOptions);
			}
		}
	}
}