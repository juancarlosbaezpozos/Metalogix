using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointSiteAlerts", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Alert.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Site Objects {0-Paste} > Alerts...")]
	[Name("Paste Site Alerts")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb))]
	public class CopyWebAlertsAction : CopyAlertsAction
	{
		public CopyWebAlertsAction()
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
				if (typeof(SPWeb).IsAssignableFrom(type))
				{
					return (((SPWeb)targetSelections[0]).Adapter.IsDB || ((SPWeb)targetSelections[0]).Adapter.IsNws || ((SPWeb)sourceSelections[0]).Adapter.IsNws || !(((SPWeb)sourceSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPWeb)sourceSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 || !(((SPWeb)targetSelections[0]).Adapter.AdapterShortName != "CSOM") && ((SPWeb)targetSelections[0]).Adapter.SharePointVersion.MajorVersion == SharePointMajorVersion.SharePoint2013 ? false : !((SPWeb)sourceSelections[0]).Adapter.SharePointVersion.IsSharePointOnline);
				}
			}
			return false;
		}

		public override PropertyInfo[] GetOptionParameters(object cmdletOptions)
		{
			PropertyInfo[] property = new PropertyInfo[] { cmdletOptions.GetType().GetProperty("CopyItemAlerts", BindingFlags.Instance | BindingFlags.Public), cmdletOptions.GetType().GetProperty("CopyChildSiteAlerts", BindingFlags.Instance | BindingFlags.Public) };
			return property;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source[0] is SPWeb && target[0] is SPWeb)
			{
				SPWeb item = (SPWeb)source[0];
				SPWeb sPWeb = (SPWeb)target[0];
				base.InitializeOptimizationTable(item);
				base.CopyWebAlerts(item, sPWeb, base.SharePointOptions);
			}
		}
	}
}