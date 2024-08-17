using System;

namespace Metalogix.SharePoint.Actions.Migration.Exceptions
{
	public class MissingSharePointNodeException : Exception
	{
		private const string ExceptionMessage = "SharePoint node is missing.";

		public MissingSharePointNodeException() : base("SharePoint node is missing.")
		{
		}
	}
}