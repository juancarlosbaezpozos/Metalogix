using System;
using System.ComponentModel;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[RunInstaller(true)]
	public class MetalogixSharePointMigrationCommandsSnapIn : PSSnapIn
	{
		public override string Description
		{
			get
			{
				return "A set of cmdlets for executing Metalogix's SharePoint migration actions";
			}
		}

		public override string Name
		{
			get
			{
				return "Metalogix.SharePoint.Migration.Commands";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Metalogix";
			}
		}

		public MetalogixSharePointMigrationCommandsSnapIn()
		{
		}
	}
}