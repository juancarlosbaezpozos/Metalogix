using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.ChangeConfiguration16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("2:Change configuration for selected job {3-Jobs}")]
	[MenuTextPlural("2:Change configuration for selected jobs {3-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class ReconfigureJobs : Metalogix.Actions.Action
	{
		public ReconfigureJobs()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (JobHelper.ContainsUnassociatedJobs(targetSelections))
			{
				return false;
			}
			Type type = ((Job)targetSelections[0]).Action.GetType();
			for (int i = 0; i < targetSelections.Count; i++)
			{
				if (((Job)targetSelections[i]).Action.GetType() != type)
				{
					return false;
				}
			}
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			bool flag;
			bool flag1;
			OptionsBase optionsBase;
			Job[] jobArray = JobUIHelper.GetJobArray(target);
			try
			{
				try
				{
					Cursor.Current = Cursors.WaitCursor;
					JobCollection parent = jobArray[0].Parent;
					bool flag2 = true;
					Metalogix.Actions.Action action = jobArray[0].Action;
					Type type = action.GetType();
					Job[] jobArray1 = jobArray;
					int num = 0;
					while (num < (int)jobArray1.Length)
					{
						if (jobArray1[num].Action.GetType() == type)
						{
							num++;
						}
						else
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						parent.BeginUpdate();
						ConfigurationResult configurationResult = ConfigurationResult.Cancel;
						try
						{
							Job jobID = jobArray[0];
							IXMLAbleList sourceList = jobID.SourceList;
							IXMLAbleList targetList = jobID.TargetList;
							if (jobID.Action.Options != null)
							{
								optionsBase = jobID.Action.Options.Clone();
							}
							else
							{
								optionsBase = null;
							}
							OptionsBase optionsBase1 = optionsBase;
							jobID.Action.JobID = jobArray[0].JobID;
							configurationResult = jobID.Action.Configure(ref sourceList, ref targetList, out flag);
							if (!flag)
							{
								FlatXtraMessageBox.Show("This action has no configuration.", "No Configuration", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
								return;
							}
							else if (configurationResult != ConfigurationResult.Cancel)
							{
								bool flag3 = XMLAbleList.SerializeXMLAbleList(sourceList) != jobID.SourceXml;
								flag1 = (jobID.TargetList == null || jobID.TargetList[0] == null ? true : XMLAbleList.SerializeXMLAbleList(targetList) != jobID.TargetXml);
								OptionsBase options = null;
								if (optionsBase1 == null)
								{
									options = action.Options;
								}
								else
								{
									options = action.Options.Subtract(optionsBase1);
								}
								Job[] jobArray2 = jobArray;
								for (int i = 0; i < (int)jobArray2.Length; i++)
								{
									Job job = jobArray2[i];
									if (flag3)
									{
										job.SourceList = sourceList;
									}
									if (flag1)
									{
										job.TargetList = targetList;
									}
									if (options != null)
									{
										ActionOptions actionOption = job.Action.Options;
										actionOption.SetFromOptions(options.Clone());
										job.Action.Options = actionOption;
									}
								}
								ActionOptionsProvider.UpdateDefaultOptionsXml(jobID.Action);
							}
						}
						finally
						{
							parent.EndUpdate();
						}
						if (configurationResult == ConfigurationResult.Run)
						{
							RunSelectedJobs runSelectedJob = new RunSelectedJobs();
							base.SubActions.Add(runSelectedJob);
							object[] objArray = new object[] { source, target };
							runSelectedJob.RunAsSubAction(objArray, new ActionContext(source, target), null);
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string str = jobArray[0].SourceList.ToString();
					string empty = string.Empty;
					if (string.IsNullOrEmpty(str))
					{
						empty = "Please check your source and make sure it is valid and not been changed.";
					}
					GlobalServices.ErrorHandler.HandleException("Error Configuring Job", string.Concat("Could not configure selected job(s): ", empty, "\r\n", exception.Message), exception, ErrorIcon.Error);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}