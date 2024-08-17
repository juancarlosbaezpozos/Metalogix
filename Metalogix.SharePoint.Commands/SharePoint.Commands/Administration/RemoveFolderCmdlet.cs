using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "MLSharePointFolder")]
	public class RemoveFolderCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(DeleteFolder);
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=true, ValueFromPipelineByPropertyName=false, HelpMessage="The Folder object retrieved by using the Get-MLSharePointFolder commandlet.")]
		[ValidateNotNullOrEmpty]
		public SPFolder Folder
		{
			get;
			set;
		}

		public RemoveFolderCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.ProcessParameters())
			{
				base.Target = this.Folder;
				base.Source = null;
			}
			return true;
		}
	}
}