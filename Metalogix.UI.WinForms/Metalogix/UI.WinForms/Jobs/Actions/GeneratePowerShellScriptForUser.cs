using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.ForCurrentUser16.png")]
	[MenuText("2:Generate PowerShell Script {4-Configure} > 1:For Current User and Machine")]
	[Name("Generate PowerShell Script For Current User and Machine")]
	public class GeneratePowerShellScriptForUser : GeneratePowerShellScript
	{
		public GeneratePowerShellScriptForUser()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.CreatePowerShellScript(source, target, Cryptography.ProtectionScope.CurrentUser, null, true, false, null, false);
		}
	}
}