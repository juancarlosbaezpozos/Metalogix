using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "MLSharePointItemFromDatabase")]
	public class GetSharePointItemFromDatabase : GetSharePointListFromDatabase
	{
		private string m_sFolderPath;

		private string m_sItemName;

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The list-relative path to the desired folder, separated by \"/\".")]
		public string FolderPath
		{
			get
			{
				return this.m_sFolderPath;
			}
			set
			{
				this.m_sFolderPath = value;
			}
		}

		[Parameter(Mandatory=true, Position=4, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The filename or ID of the desired item.")]
		public string ItemName
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

		public GetSharePointItemFromDatabase()
		{
		}

		protected SPFolder GetFolderFromParameters()
		{
			SPList listFromParameters = base.GetListFromParameters();
			if (this.FolderPath == null)
			{
				return listFromParameters;
			}
			string[] strArrays = this.FolderPath.Replace('\\', '/').Split(new char[] { '/' });
			SPFolder item = listFromParameters;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				item = item.SubFolders[str] as SPFolder;
				if (item == null)
				{
					base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find a folder at the specified path", ErrorCategory.InvalidArgument, this.FolderPath));
				}
			}
			return item;
		}

		protected SPListItem GetItemFromParameters()
		{
			SPListItemCollection items = this.GetFolderFromParameters().GetItems(false, ListItemQueryType.ListItem, null);
			SPListItem sPListItem = null;
			int num = 0;
			sPListItem = (!int.TryParse(this.ItemName, out num) ? items.GetItemByFileName(this.ItemName) : items.GetItemByID(num));
			if (sPListItem == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find an item with the given identifier", ErrorCategory.InvalidArgument, this.ItemName));
			}
			return sPListItem;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetItemFromParameters());
		}
	}
}