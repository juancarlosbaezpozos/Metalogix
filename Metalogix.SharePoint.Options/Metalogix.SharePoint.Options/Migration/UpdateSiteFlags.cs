using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public enum UpdateSiteFlags
	{
		NoUpdates = 0,
		CoreMetaData = 1,
		SiteColumns = 2,
		Permissions = 4,
		PermissionLevels = 8,
		ContentTypes = 16,
		Navigation = 32,
		WebParts = 64,
		MasterPage = 128,
		MasterPageGallery = 256,
		ApplyTheme = 512,
		RequestAccessSettings = 1024,
		Features = 2048,
		Workflows = 4096,
		AssociatedGroups = 8192
	}
}