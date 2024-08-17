using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	public enum QueueState
	{
		None,
		Processing,
		CancelRequested,
		Completed,
		Error
	}
}