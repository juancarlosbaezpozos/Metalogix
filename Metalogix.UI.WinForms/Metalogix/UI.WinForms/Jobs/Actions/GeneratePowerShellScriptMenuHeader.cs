using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.GeneratePowershellScript16.png")]
	[MenuText("2:Generate PowerShell Script {4-Configure}")]
	[Name("Generate PowerShell Script Header")]
	public class GeneratePowerShellScriptMenuHeader : GeneratePowerShellScript
	{
		public GeneratePowerShellScriptMenuHeader()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}