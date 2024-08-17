using Metalogix;
using Metalogix.Core;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.Commands
{
	[Cmdlet("Set", "MetalogixConfigurationVariable")]
	public class SetConfigurationVariable : AssemblyBindingCmdlet
	{
		[Parameter(Mandatory=true, Position=1, HelpMessage="The name of the configuration variable.")]
		public string Name
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The scope of the configuration variable. Possible values are Environment, EnvironmentSpecific, User, UserSpecific, Application, ApplicationSpecific, ApplicationAndUserSpecific.  (Default: ApplicationAndUserSpecific).")]
		public ResourceScope Scope
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, Position=2, HelpMessage="The value of the configuration variable.")]
		public IConvertible Value
		{
			get;
			set;
		}

		public SetConfigurationVariable()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			if (string.IsNullOrEmpty(this.Name))
			{
				throw new ArgumentException("Name cannot be empty.");
			}
			SetConfigurationVariable.SetConfigurationValue(this.Scope, this.Name, this.Value);
		}

		public static void SetConfigurationValue(ResourceScope scope, string name, IConvertible value)
		{
			if (ConfigurationVariables.ContainsConfigurationVariable(name))
			{
				ConfigurationVariables.SetConfigurationValue<IConvertible>(name, value);
				return;
			}
			ConfigurationVariables.InitializeConfigurationVariable<IConvertible>(scope, name, value);
		}
	}
}