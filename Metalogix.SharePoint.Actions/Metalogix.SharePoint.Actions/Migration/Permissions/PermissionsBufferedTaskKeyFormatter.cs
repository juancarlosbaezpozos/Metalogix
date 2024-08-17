using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Actions.Migration.Permissions
{
	public class PermissionsBufferedTaskKeyFormatter : IPermissionsKeyFormatter
	{
		private readonly char[] _invalidCharacters = new char[] { '/', '\\', ' ' };

		private readonly string _keyPrefix = typeof(CopyRoleAssignmentsAction).Name;

		public PermissionsBufferedTaskKeyFormatter()
		{
		}

		private string GenerateUniqueKey(string nodeKey)
		{
			return string.Concat(this._keyPrefix, nodeKey);
		}

		private string GetFolderPathFor(SPListItem item)
		{
			if (item.ItemType != SPListItemType.Folder)
			{
				return item.FileDirRef;
			}
			return item.FileRef;
		}

		public string GetKeyFor(SPFolder folder)
		{
			return this.GenerateUniqueKey(this.SanitizeKey(folder.DirName));
		}

		public string GetKeyFor(SPListItem item)
		{
			return this.GenerateUniqueKey(this.SanitizeKey(this.GetFolderPathFor(item)));
		}

		public string GetKeyFor(SPWeb web)
		{
			return this.GenerateUniqueKey(this.SanitizeKey(string.Concat(web.RootSiteGUID, web.ID)));
		}

		private string SanitizeKey(string key)
		{
			return key.Trim(this._invalidCharacters);
		}
	}
}