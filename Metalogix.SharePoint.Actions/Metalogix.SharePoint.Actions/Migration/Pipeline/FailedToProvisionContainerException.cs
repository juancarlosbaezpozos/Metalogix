using System;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public class FailedToProvisionContainerException : Exception
	{
		public FailedToProvisionContainerException(string info, string details) : base(string.Concat("Failed to provision container.  ", info, Environment.NewLine, details))
		{
		}
	}
}