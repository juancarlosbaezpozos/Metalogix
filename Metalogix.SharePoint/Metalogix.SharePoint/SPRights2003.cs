using System;

namespace Metalogix.SharePoint
{
	[Flags]
	public enum SPRights2003
	{
		FullMask = -1,
		EmptyMask = 0,
		ViewListItems = 1,
		AddListItems = 2,
		EditListItems = 4,
		DeleteListItems = 8,
		CancelCheckout = 256,
		ManagePersonalViews = 512,
		ManageListPermissions = 1024,
		ManageLists = 2048,
		Open = 65536,
		ViewPages = 131072,
		AddAndCustomizePages = 262144,
		ApplyThemeAndBorder = 524288,
		ApplyStyleSheets = 1048576,
		ViewUsageData = 2097152,
		CreateSSCSite = 4194304,
		ManageSubwebs = 8388608,
		CreatePersonalGroups = 16777216,
		ManageRoles = 33554432,
		BrowseDirectories = 67108864,
		BrowseUserInfo = 134217728,
		AddDelPrivateWebParts = 268435456,
		UpdatePersonalWebParts = 536870912,
		ManageWeb = 1073741824
	}
}