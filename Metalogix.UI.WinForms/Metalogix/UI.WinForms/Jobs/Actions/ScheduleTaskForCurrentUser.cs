using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.ForCurrentUser16.png")]
	[MenuText("3:Schedule Task {4-Configure} > 1:For Current User and Machine")]
	[Name("Schedule Task For Current User and Machine")]
	public class ScheduleTaskForCurrentUser : ScheduleTask
	{
		public ScheduleTaskForCurrentUser()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.ProcessTask(source, target, Cryptography.ProtectionScope.CurrentUser);
		}
	}
}