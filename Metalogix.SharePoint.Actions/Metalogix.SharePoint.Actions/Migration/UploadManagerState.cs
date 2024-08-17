using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	public enum UploadManagerState
	{
		None,
		Processing,
		EndProcessingCalled,
		WaitingForCompletion,
		CancelRequested
	}
}