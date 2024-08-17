using System;

namespace Metalogix.SharePoint.Options.Migration
{
	[Flags]
	public enum UpdateListFlags
	{
		NoUpdates = 0,
		CoreMetaData = 1,
		Fields = 2,
		Views = 4,
		Permissions = 8,
		ContentTypes = 16
	}
}