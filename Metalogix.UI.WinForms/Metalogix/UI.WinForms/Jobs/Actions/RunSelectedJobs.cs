using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.RunJobs16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("1:Run selected job locally {0-Jobs}")]
	[MenuTextPlural("1:Run selected jobs locally {0-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class RunSelectedJobs : Metalogix.Actions.Action
	{
		public RunSelectedJobs()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (!JobHelper.ContainsRunningActions(targetSelections) && !JobHelper.ContainsUnassociatedJobs(targetSelections) && !JobHelper.ContainsPausedActions(targetSelections))
			{
				return true;
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Form topLevelControl;
			try
			{
				Job[] jobArray = JobUIHelper.GetJobArray(target);
				Control item = null;
				if (source.Count > 0)
				{
					item = source[0] as Control;
				}
				if (item == null)
				{
					topLevelControl = null;
				}
				else
				{
					topLevelControl = item.TopLevelControl as Form;
				}
				Form form = topLevelControl;
				if (FlatXtraMessageBox.Show(Resources.JobsConfirmRunMessage, Resources.JobsConfirmRunCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					JobUIHelper.RunJobs(jobArray, form, null, null);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException(Resources.JobsRunErrorCaption, string.Format(Resources.JobsRunErrorMessage, exception.Message), exception, ErrorIcon.Error);
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			this.RunAction(oParams[0] as IXMLAbleList, oParams[1] as IXMLAbleList);
		}
	}
}