using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using System;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[Name("Refresh Job")]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.ZeroOrMore)]
	[TargetType(typeof(Job))]
	public class RefreshJobAction : Metalogix.Actions.Action
	{
		public RefreshJobAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				RemoteJobScheduler.Instance.RefreshJobs(true);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = "An error occurred while refreshing jobs.";
				GlobalServices.ErrorHandler.HandleException(this.Name, str, exception, ErrorIcon.Error);
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, str, true);
			}
		}
	}
}