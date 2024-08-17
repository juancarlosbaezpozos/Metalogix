using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointMySites")]
	public class CopyMySitesCmdlet : CopySiteContentCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteMySitesAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if site collection level audit settings should be copied.")]
		public SwitchParameter CopyAuditSettings
		{
			get
			{
				return this.CopyMySiteOptions.CopyAuditSettings;
			}
			set
			{
				this.CopyMySiteOptions.CopyAuditSettings = value;
			}
		}

		protected virtual PasteMySiteOptions CopyMySiteOptions
		{
			get
			{
				return base.Action.Options as PasteMySiteOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if link correction should be run on the contents of master pages.")]
		public SwitchParameter CorrectMasterPageLinks
		{
			get
			{
				return this.CopyMySiteOptions.CorrectMasterPageLinks;
			}
			set
			{
				this.CopyMySiteOptions.CorrectMasterPageLinks = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Contains the source MySites to exclude.")]
		public CommonSerializableList<string> MySitesToExclude
		{
			get
			{
				return this.CopyMySiteOptions.MySitesToExclude;
			}
			set
			{
				this.CopyMySiteOptions.MySitesToExclude = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="Path of the mysite target.")]
		public string Path
		{
			get
			{
				return this.CopyMySiteOptions.Path;
			}
			set
			{
				this.CopyMySiteOptions.Path = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Self Service Create Mode may be used if Site Provisioning is turned on in Central Admin for the given Web Application.")]
		public SwitchParameter SelfServiceCreateMode
		{
			get
			{
				return this.CopyMySiteOptions.SelfServiceCreateMode;
			}
			set
			{
				this.CopyMySiteOptions.SelfServiceCreateMode = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="URL of the mysite target.")]
		public string URL
		{
			get
			{
				return this.CopyMySiteOptions.URL;
			}
			set
			{
				this.CopyMySiteOptions.URL = value;
			}
		}

		[Parameter(Mandatory=true, HelpMessage="Target Mysite Web Application Name.")]
		public string WebApplicationName
		{
			get
			{
				return this.CopyMySiteOptions.WebApplicationName;
			}
			set
			{
				this.CopyMySiteOptions.WebApplicationName = value;
			}
		}

		public CopyMySitesCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target of the copy is null, please initialize a proper target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}