using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.ForCurrentMachine16.png")]
	[MenuText("3:Schedule Task {4-Configure} > 2:For Current Machine")]
	[Name("Schedule Task For Current Machine")]
	public class ScheduleTaskForCurrentMachine : ScheduleTask
	{
		public ScheduleTaskForCurrentMachine()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.ProcessTask(source, target, Cryptography.ProtectionScope.LocalMachine);
		}
	}
}