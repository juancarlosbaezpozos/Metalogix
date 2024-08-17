using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointItembyFileName")]
	public class GetSharePointItembyFileName : GetSharePointItem
	{
		private string m_sItemName;

		[Parameter(Mandatory=true, Position=3, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The filename of the SharePoint document.")]
		public string FileName
		{
			get
			{
				return this.m_sItemName;
			}
			set
			{
				this.m_sItemName = value;
			}
		}

		public GetSharePointItembyFileName()
		{
		}

		protected override SPListItem GetItemFromParameters()
		{
			SPListItemCollection items = base.GetFolderFromParameters().GetItems(false, ListItemQueryType.ListItem, null);
			SPListItem itemByFileName = null;
			itemByFileName = items.GetItemByFileName(this.FileName);
			if (itemByFileName == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find an item with the given FileName", ErrorCategory.InvalidArgument, this.FileName));
			}
			return itemByFileName;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetItemFromParameters());
		}
	}
}