using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public static class HttpStatusCodeExtensions
	{
		public static bool IsRequestThrottled(this HttpStatusCode statusCode)
		{
			return statusCode == (HttpStatusCode.MultipleChoices | HttpStatusCode.Ambiguous | HttpStatusCode.MovedPermanently | HttpStatusCode.Moved | HttpStatusCode.RequestedRangeNotSatisfiable | HttpStatusCode.ExpectationFailed);
		}

		public static bool IsServiceUnavailable(this HttpStatusCode statusCode)
		{
			return statusCode == HttpStatusCode.ServiceUnavailable;
		}
	}
}