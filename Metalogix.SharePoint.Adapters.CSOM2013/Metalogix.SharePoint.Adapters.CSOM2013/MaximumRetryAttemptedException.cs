using System;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class MaximumRetryAttemptedException : Exception
	{
		public MaximumRetryAttemptedException(string message) : base(message)
		{
		}
	}
}