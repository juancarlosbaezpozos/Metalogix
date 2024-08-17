using System;

namespace Metalogix.SharePoint
{
	[Flags]
	public enum SPS2003Rights
	{
		FullMask = -1,
		EmptyMask = 0,
		ViewListItems = 1,
		AddListItems = 2,
		EditListItems = 4,
		DeleteListItems = 8,
		CancelCheckout = 16,
		ManagePersonalViews = 32,
		ViewPages = 64,
		AddAndCustomizePages = 128,
		ApplyStyleSheets = 256,
		BrowseDirectories = 512,
		AddDelPrivateWebParts = 1024,
		UpdatePersonalWebParts = 2048,
		ManageSubwebs = 4096,
		ManageWeb = 8192,
		ManageRoles = 16384,
		EditMyUserInfo = 65536,
		CreateAlerts = 131072,
		ManageAudiences = 262144,
		CreatePersonalSite = 524288,
		Search = 1048576,
		ManageSearch = 2097152,
		ManageAlerts = 4194368,
		ManagePortalSite = 8388608,
		CreateSSCSite = 16777216
	}
}