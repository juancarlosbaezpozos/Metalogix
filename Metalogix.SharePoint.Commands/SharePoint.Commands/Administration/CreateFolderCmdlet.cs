using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Add", "SharePointFolder")]
	public class CreateFolderCmdlet : ActionCmdlet
	{
		private string m_sFolderName;

		protected override Type ActionType
		{
			get
			{
				return typeof(CreateFolderAction);
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the folder to be created.")]
		public string Name
		{
			get
			{
				return this.m_sFolderName;
			}
			set
			{
				this.m_sFolderName = value;
			}
		}

		public CreateFolderCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			((CreateFolderAction)base.Action).SharePointOptions.FolderName = this.Name;
			if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target for the copy is not initialized properly. Please initialize a target and try again."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			base.WriteObject(base.Target.Children[this.Name]);
		}
	}
}