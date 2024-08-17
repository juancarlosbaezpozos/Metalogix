using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[Flags]
	public enum TaxonomySetupOptions
	{
		None = 0,
		MapTermstores = 1,
		ReferencedMMD = 2,
		TransformList = 4,
		TransformSiteColumns = 8,
		ResolveByName = 16
	}
}