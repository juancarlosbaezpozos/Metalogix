using Metalogix.SharePoint;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointList")]
	public class GetSharePointList : GetSharePointSite
	{
		private string m_sListName;

		[Parameter(Mandatory=true, Position=1, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the desired list.")]
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

		public GetSharePointList()
		{
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