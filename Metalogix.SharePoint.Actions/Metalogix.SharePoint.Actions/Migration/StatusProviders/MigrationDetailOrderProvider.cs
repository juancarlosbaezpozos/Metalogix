using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration.StatusProviders
{
	public class MigrationDetailOrderProvider : CompletionDetailsOrderProvider
	{
		public MigrationDetailOrderProvider()
		{
			List<string> orderingList = this.OrderingList;
			string[] migrationDetailSitesCopied = new string[] { Resources.Migration_Detail_SitesCopied, Resources.Migration_Detail_ListsCopied, Resources.Migration_Detail_FoldersCopied, Resources.Migration_Detail_ItemsCopied, Resources.Migration_Detail_UsersCopied, Resources.Migration_Detail_GroupsCopied, Resources.Migration_Detail_RoleAssignments };
			orderingList.AddRange(migrationDetailSitesCopied);
		}
	}
}