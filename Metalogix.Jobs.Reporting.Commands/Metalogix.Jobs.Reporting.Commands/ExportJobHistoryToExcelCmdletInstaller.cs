using System;
using System.ComponentModel;
using System.Management.Automation;

namespace Metalogix.Jobs.Reporting.Commands
{
	[RunInstaller(true)]
	public class ExportJobHistoryToExcelCmdletInstaller : PSSnapIn
	{
		public override string Description
		{
			get
			{
				return "A cmlet for exporting Metalogix Jobs to Excel";
			}
		}

		public override string DescriptionResource
		{
			get
			{
				return "Metalogix.Jobs.Reporting.Commands, A cmlet for exporting Metalogix Jobs to Excel";
			}
		}

		public override string Name
		{
			get
			{
				return "Metalogix.Jobs.Reporting.Commands";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Metalogix";
			}
		}

		public override string VendorResource
		{
			get
			{
				return "Metalogix.Jobs.Reporting.Commands,Metalogix";
			}
		}

		public ExportJobHistoryToExcelCmdletInstaller()
		{
		}
	}
}