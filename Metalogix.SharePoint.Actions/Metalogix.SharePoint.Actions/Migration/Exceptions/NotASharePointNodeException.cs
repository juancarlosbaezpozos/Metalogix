using System;

namespace Metalogix.SharePoint.Actions.Migration.Exceptions
{
	public class NotASharePointNodeException : Exception
	{
		private const string ExceptionMessageFormat = "The target node type '{0}' is not a SharePoint node.";

		public NotASharePointNodeException(Type nodeType) : base(string.Format("The target node type '{0}' is not a SharePoint node.", nodeType.FullName))
		{
		}
	}
}