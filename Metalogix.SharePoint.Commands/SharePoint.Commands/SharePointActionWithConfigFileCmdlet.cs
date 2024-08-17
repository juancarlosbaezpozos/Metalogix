using Metalogix;
using Metalogix.Actions;
using Metalogix.Commands;
using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands
{
	public abstract class SharePointActionWithConfigFileCmdlet : SharePointActionCmdlet
	{
		[Parameter(Mandatory=true, HelpMessage="Indicates if we should use a job config file specified.")]
		public string JobConfigFile
		{
			get;
			set;
		}

		protected SharePointActionWithConfigFileCmdlet()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
			if (!string.IsNullOrEmpty(this.JobConfigFile))
			{
				try
				{
					using (FileStream fileStream = new FileStream(this.JobConfigFile, FileMode.Open))
					{
						ActionOptionsTemplate.JobConfiguration jobConfiguration = ActionOptionsTemplate.Load(fileStream, "");
						base.Action.Options.FromXML(jobConfiguration.OptionsXml);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					base.WriteError(new ErrorRecord(exception, "Error retrieving job configuration file", ErrorCategory.InvalidOperation, this.ActionType));
				}
			}
		}

		protected override bool ProcessParameters()
		{
			if (!base.ProcessParameters())
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.JobConfigFile))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("Missing configuration file"), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return true;
		}
	}
}