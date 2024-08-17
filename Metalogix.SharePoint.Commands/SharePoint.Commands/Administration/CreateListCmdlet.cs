using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Add", "SharePointList")]
	public class CreateListCmdlet : ActionCmdlet
	{
		private string m_sTitle = "";

		private string m_sName = "";

		private string m_sDescription = "";

		private string m_sTemplateName = "101";

		private bool m_bIsOnQuickLaunch;

		private bool m_bHasVersions;

		private bool m_bMinorVersions;

		private bool m_bRequiresContentApproval;

		protected override Type ActionType
		{
			get
			{
				return typeof(CreateListAction);
			}
		}

		[Parameter(Mandatory=false, Position=2, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A description of the list.")]
		public string Description
		{
			get
			{
				return this.m_sDescription;
			}
			set
			{
				this.m_sDescription = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates if the list should enable minor versions.")]
		public SwitchParameter HasMinorVersions
		{
			get
			{
				return this.m_bMinorVersions;
			}
			set
			{
				this.m_bMinorVersions = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates if the list should enable major versions.")]
		public SwitchParameter HasVersions
		{
			get
			{
				return this.m_bHasVersions;
			}
			set
			{
				this.m_bHasVersions = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates if the list should be added to the quick launch.")]
		public SwitchParameter IsOnQuickLaunch
		{
			get
			{
				return this.m_bIsOnQuickLaunch;
			}
			set
			{
				this.m_bIsOnQuickLaunch = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name for the new list.")]
		public string Name
		{
			get
			{
				return this.m_sName;
			}
			set
			{
				this.m_sName = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates if the list should require content approval.")]
		public SwitchParameter RequiresContentApproval
		{
			get
			{
				return this.m_bRequiresContentApproval;
			}
			set
			{
				this.m_bRequiresContentApproval = value;
			}
		}

		[Parameter(Mandatory=true, Position=1, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The numeric code of the template to use.")]
		public string TemplateName
		{
			get
			{
				return this.m_sTemplateName;
			}
			set
			{
				this.m_sTemplateName = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The title for the new list.")]
		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
			set
			{
				this.m_sTitle = value;
			}
		}

		public CreateListCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			CreateListOptions createListOption = new CreateListOptions();
			if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target for the copy is not initialized properly. Please initialize a target and try again."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			createListOption.Name = this.Name;
			createListOption.Title = (!string.IsNullOrEmpty(this.Title) ? this.Title : this.Name);
			createListOption.Description = this.Description;
			createListOption.Template = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), this.TemplateName);
			createListOption.IsOnQuickLaunch = this.IsOnQuickLaunch;
			createListOption.HasVersions = this.HasVersions;
			createListOption.HasMinorVersions = this.HasMinorVersions;
			createListOption.RequiresContentApproval = this.RequiresContentApproval;
			((CreateListAction)base.Action).SharePointOptions = createListOption;
			return true;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			base.WriteObject(base.Target.Children[this.Name]);
		}
	}
}