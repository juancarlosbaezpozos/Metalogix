using System;

namespace Metalogix.Azure
{
	[Flags]
	public enum BlobAccessPermissions
	{
		None = 0,
		Read = 1,
		Write = 2,
		Delete = 4,
		List = 8
	}
}