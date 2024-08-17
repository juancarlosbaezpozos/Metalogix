using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[IsAdvanced(true)]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("2:Generate job groups {1-Analyze}")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.MoreThanOne)]
	[TargetType(typeof(Job))]
	public class GenerateJobGroups : Metalogix.Actions.Action
	{
		public GenerateJobGroups()
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
					object obj = MLShortTextEntryDialog.ShowDialog("Configure Job Groupings", "Number of Groups", "2", TextBoxSize.Small, TextEntryType.Integer);
					if (obj != null)
					{
						int num = int.Parse(obj.ToString());
						JobGrouping[] array = JobGrouping.GroupJobs(jobArray, num).ToArray();
						Job[] jobArray1 = jobArray;
						for (int i = 0; i < (int)jobArray1.Length; i++)
						{
							Job job = jobArray1[i];
							string title = job.Title;
							int num1 = title.IndexOf("{");
							int num2 = title.IndexOf("}");
							if (num1 >= 0 && num2 >= 0)
							{
								title = title.Substring(num2 + 1);
							}
							int num3 = 0;
							while (num3 < (int)array.Length)
							{
								if (array[num3][job.JobID] == null)
								{
									num3++;
								}
								else
								{
									int num4 = num3 + 1;
									title = string.Concat("{ Group ", num4.ToString(), " }", title);
									break;
								}
							}
							job.Title = title;
						}
						StringBuilder stringBuilder = new StringBuilder();
						int num5 = 1;
						stringBuilder.Append("Job Grouping Data:\n");
						JobGrouping[] jobGroupingArray = array;
						for (int j = 0; j < (int)jobGroupingArray.Length; j++)
						{
							JobGrouping jobGrouping = jobGroupingArray[j];
							stringBuilder.Append(string.Concat("Group ", num5.ToString(), ": "));
							Dictionary<string, string> strs = jobGrouping.ParseAnalysis();
							bool flag = false;
							foreach (KeyValuePair<string, string> keyValuePair in strs)
							{
								if (flag)
								{
									stringBuilder.Append(" \t");
								}
								stringBuilder.Append(string.Concat(keyValuePair.Key, " - ", keyValuePair.Value));
								flag = true;
							}
							stringBuilder.Append("\n");
							num5++;
						}
						FlatXtraMessageBox.Show(stringBuilder.ToString(), "Grouping Information", MessageBoxButtons.OK);
					}
					else
					{
						return;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GlobalServices.ErrorHandler.HandleException("Error Generating Job Groupings", string.Concat("Failed to generate job groupings: ", exception.Message), exception, ErrorIcon.Error);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}