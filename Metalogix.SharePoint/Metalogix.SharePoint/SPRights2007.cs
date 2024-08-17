using System;

namespace Metalogix.SharePoint
{
	[Flags]
	public enum SPRights2007 : long
	{
		EmptyMask = 0,
		ViewListItems = 1,
		AddListItems = 2,
		EditListItems = 4,
		DeleteListItems = 8,
		ApproveItems = 16,
		OpenItems = 32,
		ViewVersions = 64,
		DeleteVersions = 128,
		CancelCheckout = 256,
		ManagePersonalViews = 512,
		ManageLists = 2048,
		ViewFormPages = 4096,
		Open = 65536,
		ViewPages = 131072,
		AddAndCustomizePages = 262144,
		ApplyThemeAndBorder = 524288,
		ApplyStyleSheets = 1048576,
		ViewUsageData = 2097152,
		CreateSSCSite = 4194304,
		ManageSubwebs = 8388608,
		CreateGroups = 16777216,
		ManagePermissions = 33554432,
		BrowseDirectories = 67108864,
		BrowseUserInfo = 134217728,
		AddDelPrivateWebParts = 268435456,
		UpdatePersonalWebParts = 536870912,
		ManageWeb = 1073741824,
		UseClientIntegration = 68719476736,
		UseRemoteAPIs = 137438953472,
		ManageAlerts = 274877906944,
		CreateAlerts = 549755813888,
		EditMyUserInfo = 1099511627776,
		EnumeratePermissions = 4611686018427387904,
		FullMask = 9223372036854775807
	}
}