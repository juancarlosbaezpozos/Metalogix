using System;
using System.ComponentModel;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[RunInstaller(true)]
	public class MetalogixSharePointCommandsSnapIn : PSSnapIn
	{
		public override string Description
		{
			get
			{
				return "A set of cmdlets for interaction with Metalogix's SharePoint explorer";
			}
		}

		public override string Name
		{
			get
			{
				return "Metalogix.SharePoint.Commands";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Metalogix";
			}
		}

		public MetalogixSharePointCommandsSnapIn()
		{
		}
	}
}