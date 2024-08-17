using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[IsAdvanced(true)]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("1:Analyze selected job {1-Analyze}")]
	[MenuTextPlural("1:Analyze selected jobs {1-Analyze}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class AnalyzeJobs : Metalogix.Actions.Action
	{
		public AnalyzeJobs()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (JobHelper.ContainsUnassociatedJobs(targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (((Job)enumerator.Current).Action.Analyzable)
					{
						continue;
					}
					flag = false;
					return flag;
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
			try
			{
				try
				{
					Job[] jobArray = JobUIHelper.GetJobArray(target);
					Cursor.Current = Cursors.WaitCursor;
					AnalysisConfigurationDialog analysisConfigurationDialog = new AnalysisConfigurationDialog();
					analysisConfigurationDialog.ShowDialog();
					if (analysisConfigurationDialog.DialogResult != DialogResult.Cancel)
					{
						DateTime? pivotDate = analysisConfigurationDialog.PivotDate;
						Job[] jobArray1 = jobArray;
						for (int i = 0; i < (int)jobArray1.Length; i++)
						{
							jobArray1[i].Analyze(pivotDate);
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
					GlobalServices.ErrorHandler.HandleException("Error Analyzing Jobs", string.Concat("Failed to analyze job: ", exception.Message), exception, ErrorIcon.Error);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}