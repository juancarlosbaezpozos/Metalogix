using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointUsers")]
	public class CopyUsersCmdlet : SharePointActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyUsersAction);
			}
		}

		public virtual CopyUserOptions CopyUsersOptions
		{
			get
			{
				return base.Action.Options as CopyUserOptions;
			}
		}

		[Parameter(HelpMessage="A collection of users to be copied to the target site.")]
		public SPUserCollection SourceUsers
		{
			get
			{
				return this.CopyUsersOptions.SourceUsers;
			}
			set
			{
				this.CopyUsersOptions.SourceUsers = value;
			}
		}

		public CopyUsersCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}