using System;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public class FailedToProvisionQueueException : Exception
	{
		public FailedToProvisionQueueException(string info, string details) : base(string.Concat("Failed to provision queue.  ", info, Environment.NewLine, details))
		{
		}
	}
}