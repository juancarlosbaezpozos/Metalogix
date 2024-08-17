using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.CopySelectedJobsToClipboard16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("2:Copy selected job to clipboard {0-Jobs}")]
	[MenuTextPlural("2:Copy selected jobs to clipboard {0-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Copy)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class CopyJobToClipboard : Metalogix.Actions.Action
	{
		public CopyJobToClipboard()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Job Title\tSource\tTarget\tStarted\tFinished\tStatus\tLog Summary");
				foreach (Job job in target)
				{
					object[] title = new object[] { job.Title, job.Source, job.Target, job.Started, job.Finished, job.StatusMessage, job.ResultsSummary };
					stringBuilder.Append(string.Format("\r\n{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", title));
				}
				Clipboard.SetDataObject(stringBuilder.ToString());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException("Error Copying Jobs to Clipboard", string.Concat("Unable to copy to to clipboard: ", exception.Message), exception, ErrorIcon.Error);
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			this.RunAction(oParams[0] as IXMLAbleList, oParams[1] as IXMLAbleList);
		}
	}
}