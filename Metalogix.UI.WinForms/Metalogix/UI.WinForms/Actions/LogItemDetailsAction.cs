using Metalogix.Actions;
using Metalogix.Licensing;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.ViewLogs16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("View log item...")]
	[Name("View log item")]
	[RunAsync(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(LogItemListControl))]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(LogItem))]
	public class LogItemDetailsAction : Metalogix.Actions.Action, ILogAction
	{
		private LogItemDetails m_logItemDetailsForm;

		public LogItemDetailsAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return base.AppliesTo(sourceSelections, targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source.Count != 1 || !(source[0] is LogItemListControl))
			{
				return;
			}
			LogItemListControl item = source[0] as LogItemListControl;
			if (this.m_logItemDetailsForm == null || this.m_logItemDetailsForm.IsDisposed)
			{
				this.m_logItemDetailsForm = LogItemDetails.GetInstance(item);
			}
			if (!this.m_logItemDetailsForm.Visible)
			{
				this.m_logItemDetailsForm.Show();
			}
			else if (this.m_logItemDetailsForm.WindowState == FormWindowState.Minimized)
			{
				this.m_logItemDetailsForm.WindowState = FormWindowState.Normal;
			}
			if (!this.m_logItemDetailsForm.ContainsFocus)
			{
				this.m_logItemDetailsForm.Activate();
			}
		}
	}
}