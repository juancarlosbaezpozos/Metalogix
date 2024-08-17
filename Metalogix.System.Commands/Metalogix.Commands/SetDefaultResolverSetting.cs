using Metalogix.Core.ObjectResolution;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.Commands
{
	[Cmdlet("Set", "MetalogixDefaultResolverSetting")]
	public class SetDefaultResolverSetting : AssemblyBindingCmdlet
	{
		[Parameter(Mandatory=true, Position=0, HelpMessage="The name of the setting.")]
		public string Name
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, Position=1, HelpMessage="The value for the setting.")]
		public string Value
		{
			get;
			set;
		}

		public SetDefaultResolverSetting()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			if (string.IsNullOrEmpty(this.Name))
			{
				throw new ArgumentException("Name cannot be empty.");
			}
			if (string.IsNullOrEmpty(this.Value))
			{
				throw new ArgumentException("ResolverType cannot be empty.");
			}
			DefaultResolverSettings.SetResolversFromPowershell(this.Name, this.Value);
		}
	}
}