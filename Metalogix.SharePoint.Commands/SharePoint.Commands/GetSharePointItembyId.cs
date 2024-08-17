using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointItembyID")]
	public class GetSharePointItembyId : GetSharePointItem
	{
		private int m_sItemID;

		[Parameter(Mandatory=true, Position=3, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The ID of the SharePoint item.")]
		public int Id
		{
			get
			{
				return this.m_sItemID;
			}
			set
			{
				this.m_sItemID = value;
			}
		}

		public GetSharePointItembyId()
		{
		}

		protected override SPListItem GetItemFromParameters()
		{
			SPListItemCollection items = base.GetFolderFromParameters().GetItems(false, ListItemQueryType.ListItem, null);
			SPListItem itemByID = null;
			itemByID = items.GetItemByID(this.Id);
			if (itemByID == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find an item with the given ID", ErrorCategory.InvalidArgument, (object)this.Id));
			}
			return itemByID;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetItemFromParameters());
		}
	}
}