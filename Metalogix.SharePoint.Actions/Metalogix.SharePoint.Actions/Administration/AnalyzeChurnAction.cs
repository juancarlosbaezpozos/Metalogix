using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.Utilities;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Batchable(false, "")]
	[CmdletEnabled(false, null, null)]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Churn.ico")]
	[IsAdvanced(true)]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("Analyze Churn... {5-Properties}")]
	[Name("Analyze Churn")]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	public class AnalyzeChurnAction : SharePointAction<AnalyzeChurnOptions>
	{
		public AnalyzeChurnAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.EnabledOn(sourceSelections, targetSelections);
			foreach (SPNode targetSelection in targetSelections)
			{
				bool isSearchable = false;
				Type type = targetSelections[0].GetType();
				if (typeof(SPServer).IsAssignableFrom(type))
				{
					isSearchable = true;
				}
				else if (typeof(SPWeb).IsAssignableFrom(type))
				{
					isSearchable = ((SPWeb)targetSelections[0]).IsSearchable;
				}
				else if (typeof(SPFolder).IsAssignableFrom(type))
				{
					isSearchable = ((SPFolder)targetSelections[0]).ParentList.ParentWeb.IsSearchable;
				}
				flag = (!flag ? false : isSearchable);
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			long num = (long)0;
			long num1 = (long)0;
			if (typeof(SPNode).IsAssignableFrom(target.CollectionType))
			{
				foreach (SPNode sPNode in target)
				{
					LogItem logItem = new LogItem("Analyzing Churn", sPNode.Name, sPNode.DisplayUrl, null, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					long num2 = (long)0;
					long num3 = (long)0;
					if (!sPNode.AnalyzeChurn(base.SharePointOptions.PivotDate, base.SharePointOptions.AnalyzeRecursively, out num3, out num2))
					{
						logItem.Information = "Analysis not available";
						logItem.Status = ActionOperationStatus.Skipped;
					}
					else
					{
						logItem.Information = string.Concat("Analysis: Bytes Changed - ", Format.FormatSize(new long?(num3)), "   Items Changed - ", num2.ToString());
						logItem.Status = ActionOperationStatus.Completed;
						num1 += num3;
						num += num2;
					}
					base.FireOperationFinished(logItem);
				}
				LogItem logItem1 = new LogItem("Total Detected Churn", null, null, null, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem1);
				logItem1.Information = string.Concat("Analysis: Bytes Changed: ", Format.FormatSize(new long?(num1)), "   Items Changed: ", num.ToString());
				logItem1.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem1);
			}
		}
	}
}