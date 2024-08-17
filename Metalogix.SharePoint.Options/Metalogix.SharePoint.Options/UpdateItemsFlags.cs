using System;

namespace Metalogix.SharePoint.Options
{
	[Flags]
	public enum UpdateItemsFlags
	{
		NoUpdates,
		CoreMetaData,
		Permissions
	}
}