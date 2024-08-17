using Metalogix.Core;
using Metalogix.Interfaces;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.Commands
{
	[Cmdlet("Get", "MetalogixConfigurationVariable")]
	public class GetConfigurationVariable : AssemblyBindingCmdlet
	{
		[Parameter(Mandatory=true, Position=0, HelpMessage="The name of the configuration variable.")]
		public string Name
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, Position=1, HelpMessage="Specify the value type of the configuration variable.  This type must be IConvertible. (Default: IConvertible).")]
		public Type ValueType
		{
			get;
			set;
		}

		public GetConfigurationVariable()
		{
			this.ValueType = typeof(IConvertible);
		}

		public static object GetConfigurationValue(string name, Type valueType)
		{
			IConfigurationVariable configurationVariable = ConfigurationVariables.GetConfigurationVariable(name);
			if (configurationVariable == null)
			{
				return null;
			}
			IConvertible value = configurationVariable.GetValue();
			if (valueType == typeof(IConvertible))
			{
				return value;
			}
			return Convert.ChangeType(value, valueType);
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			object configurationValue = GetConfigurationVariable.GetConfigurationValue(this.Name, this.ValueType);
			base.WriteObject(configurationValue);
		}
	}
}