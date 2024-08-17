using Metalogix;
using System;
using System.Management.Automation;

namespace Metalogix.Commands
{
	[Cmdlet("Set", "ApplicationDataPath")]
	public class SetApplicationPath : Cmdlet
	{
		private string m_sPath;

		private bool m_bIncludeCompanyName;

		[Parameter(Mandatory=false, Position=0, HelpMessage="Indicates that the Metalogix company name should still be used as part of the relative path from the specified directory to the application data folder.")]
		public SwitchParameter IncludeCompanyName
		{
			get
			{
				return this.m_bIncludeCompanyName;
			}
			set
			{
				this.m_bIncludeCompanyName = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The path to be used as the root folder for the application path. Note that relative path elements relating to the product being used will still be generated within this directory, and that the application data will be contained within those folders.")]
		public string Path
		{
			get
			{
				return this.m_sPath;
			}
			set
			{
				this.m_sPath = value;
			}
		}

		public SetApplicationPath()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
			ApplicationData.ApplicationDataFolder = this.Path;
			if (!this.m_bIncludeCompanyName)
			{
				ApplicationData.CompanyFolderName = string.Empty;
			}
		}
	}
}