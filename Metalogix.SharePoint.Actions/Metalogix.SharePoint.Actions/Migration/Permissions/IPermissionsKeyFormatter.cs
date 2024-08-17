using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration.Permissions
{
	public interface IPermissionsKeyFormatter
	{
		string GetKeyFor(SPFolder folder);

		string GetKeyFor(SPListItem item);

		string GetKeyFor(SPWeb web);
	}
}