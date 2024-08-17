using System;

namespace Metalogix.Azure
{
	[Flags]
	public enum QueueAccessPermissions
	{
		None = 0,
		Read = 1,
		Add = 2,
		Update = 4,
		ProcessMessages = 8
	}
}