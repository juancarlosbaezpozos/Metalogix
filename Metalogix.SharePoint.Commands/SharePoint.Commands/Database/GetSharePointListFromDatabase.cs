using Metalogix.SharePoint;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "MLSharePointListFromDatabase")]
	public class GetSharePointListFromDatabase : GetSharePointSiteFromDatabase
	{
		private string m_sListName;

		[Parameter(Mandatory=true, Position=3, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the desired list.")]
		public string ListName
		{
			get
			{
				return this.m_sListName;
			}
			set
			{
				this.m_sListName = value;
			}
		}

		public GetSharePointListFromDatabase()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
		}

		protected override void EndProcessing()
		{
			base.EndProcessing();
		}

		protected SPList GetListFromParameters()
		{
			SPList item = base.GetWebFromParameters().Lists[this.ListName];
			if (item == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find a list at the specified path", ErrorCategory.InvalidArgument, this.ListName));
			}
			return item;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetListFromParameters());
		}
	}
}