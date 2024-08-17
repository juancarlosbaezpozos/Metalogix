using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.Certificate16.png")]
	[MenuText("3:Schedule Task {4-Configure} > 3:For Certificate")]
	[Name("Schedule Task For Certificate")]
	public class ScheduleTaskForCertificate : ScheduleTask
	{
		public ScheduleTaskForCertificate()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.ProcessTask(source, target, Cryptography.ProtectionScope.Certificate);
		}
	}
}