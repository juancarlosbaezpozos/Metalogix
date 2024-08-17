using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.SharePoint.Migration;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("2:Rename job {2-Interact}")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Rename)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Job))]
	public class RenameJob : Metalogix.Actions.Action
	{
		public RenameJob()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				JobListControl jobListControl = JobUIHelper.GetJobListControl(source);
				if (jobListControl == null)
				{
					InputBox inputBox = new InputBox("Rename Job", "Enter new job name", false, 0, 0);
					if (inputBox.ShowDialog() == DialogResult.OK)
					{
						((Job)target[0]).Title = inputBox.InputValue;
					}
				}
				else
				{
					jobListControl.BeginRenameSelectedJob();
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}