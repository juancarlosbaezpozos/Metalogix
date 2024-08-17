using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[Flags]
	public enum FilterSetupOptions
	{
		None = 0,
		Sites = 1,
		Lists = 2,
		Folders = 4,
		Items = 8,
		ListColumns = 16,
		SiteColumns = 32,
		CustomFolders = 64,
		CustomFiles = 128
	}
}