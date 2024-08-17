using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "MLSharePointList")]
	public class RemoveListCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(DeleteList);
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The list object retrieved by using the Get-MLSharePointList commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPList List
		{
			get;
			set;
		}

		public RemoveListCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters())
			{
				base.Target = this.List;
				base.Source = null;
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}