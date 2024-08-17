using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "MLSharePointItem")]
	public class RemoveListItemCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(DeleteItem);
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The ListItem object retrieved by using the Get-MLSharePointItem commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPListItem Item
		{
			get;
			set;
		}

		public RemoveListItemCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters())
			{
				base.Target = this.Item;
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