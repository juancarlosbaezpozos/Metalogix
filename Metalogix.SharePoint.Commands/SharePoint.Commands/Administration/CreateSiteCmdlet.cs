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
	[Cmdlet("Add", "SharePointSite")]
	public class CreateSiteCmdlet : ActionCmdlet
	{
		private string m_sUrlName;

		private string m_sTitle;

		private string m_sTemplateName;

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A description of the new site.")]
		private string m_sDescription;

		protected override Type ActionType
		{
			get
			{
				return typeof(CreateSiteAction);
			}
		}

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

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name for the new site.")]
		public string Name
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

		[Parameter(Mandatory=true, Position=1, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the template to use, either in [Name]#[Config] form or by the name of the STP file.")]
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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The URL name for the new site. If not specified, it will be automatically generated from the given name.")]
		public string URL
		{
			get
			{
				return this.m_sUrlName;
			}
			set
			{
				this.m_sUrlName = value;
			}
		}

		public CreateSiteCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			CreateSiteOptions createSiteOption = new CreateSiteOptions()
			{
				Title = this.Name,
				Description = this.Description
			};
			if (this.URL == null)
			{
				createSiteOption.URL = this.Name.Replace(" ", "");
			}
			else
			{
				createSiteOption.URL = this.URL;
			}
			createSiteOption.Template = ((SPWeb)base.Target).Templates[this.TemplateName];
			if (createSiteOption.Template == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The web template: ", (base.Source as SPWeb).Template.Name, " was not found on the target server. Please select a different template.")), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			((CreateSiteAction)base.Action).SharePointOptions = createSiteOption;
			return createSiteOption.Template != null;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target for the copy is not initialized properly. Please initialize a target and try again."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			base.WriteObject(base.Target.Children[this.Name]);
		}
	}
}