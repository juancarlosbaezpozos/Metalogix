using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.ForCurrentMachine16.png")]
	[MenuText("2:Generate PowerShell Script {4-Configure} > 2:For Current Machine")]
	[Name("Generate PowerShell Script For Current Machine")]
	public class GeneratePowerShellScriptForMachine : GeneratePowerShellScript
	{
		public GeneratePowerShellScriptForMachine()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.CreatePowerShellScript(source, target, Cryptography.ProtectionScope.LocalMachine, null, true, false, null, false);
		}
	}
}