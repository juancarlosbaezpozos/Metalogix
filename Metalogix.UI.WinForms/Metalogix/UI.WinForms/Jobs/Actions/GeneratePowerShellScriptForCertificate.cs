using Metalogix;
using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.Certificate16.png")]
	[MenuText("2:Generate PowerShell Script {4-Configure} > 3:For Certificate")]
	[Name("Generate PowerShell Script For Certificate")]
	public class GeneratePowerShellScriptForCertificate : GeneratePowerShellScript
	{
		public GeneratePowerShellScriptForCertificate()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			base.CreatePowerShellScript(source, target, Cryptography.ProtectionScope.Certificate, null, true, false, null, false);
		}
	}
}