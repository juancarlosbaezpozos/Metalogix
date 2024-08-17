using System;

namespace Metalogix.SharePoint
{
	public enum SPWorkflowState
	{
		None = 0,
		Locked = 1,
		Running = 2,
		Completed = 4,
		Cancelled = 8,
		Expiring = 16,
		Expired = 32,
		Faulting = 64,
		Terminated = 128,
		Suspended = 256,
		Orphaned = 512,
		HasNewEvents = 1024,
		NotStarted = 2048,
		All = 4095
	}
}