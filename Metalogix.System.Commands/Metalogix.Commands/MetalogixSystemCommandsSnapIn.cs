using System;
using System.ComponentModel;
using System.Management.Automation;

namespace Metalogix.Commands
{
	[RunInstaller(true)]
	public class MetalogixSystemCommandsSnapIn : PSSnapIn
	{
		public override string Description
		{
			get
			{
				return "Defines a provider to allow PowerShell navigation of Metalogix node hierarchies";
			}
		}

		public override string Name
		{
			get
			{
				return "Metalogix.System.Commands";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Metalogix";
			}
		}

		public MetalogixSystemCommandsSnapIn()
		{
		}
	}
}