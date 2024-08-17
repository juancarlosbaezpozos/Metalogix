using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public static class ClientContextExtension
	{
		public static void ExecuteQueryWithIncrementalRetry(this ClientRuntimeContext context, int retryCount, int delay = 30000)
		{
			int num = 0;
			int num1 = delay;
			if (retryCount <= 0)
			{
				throw new ArgumentException("Provide a retry count greater than zero.");
			}
			if (delay <= 0)
			{
				throw new ArgumentException("Provide a delay greater than zero.");
			}
			while (num < retryCount)
			{
				try
				{
					context.ExecuteQuery();
				}
				catch (WebException webException1)
				{
					WebException webException = webException1;
					Utils.LogExceptionDetails(webException, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name, null);
					HttpWebResponse response = webException.Response as HttpWebResponse;
					if (response == null)
					{
						throw;
					}
					if (!response.StatusCode.IsRequestThrottled() && !response.StatusCode.IsServiceUnavailable())
					{
						throw;
					}
					Trace.WriteLine(string.Format("CSOM request frequency exceeded usage limits. Sleeping for {0} seconds before retrying.", num1));
					Thread.Sleep(num1);
					num++;
					num1 *= 2;
					continue;
				}
				return;
			}
			throw new MaximumRetryAttemptedException(string.Format("Maximum number of retry attempts ({0}) have been attempted.", retryCount));
		}

		public static bool IsSharePointOnline(this ClientRuntimeContext context)
		{
			bool value;
			try
			{
				if (context == null)
				{
					throw new ArgumentNullException("context");
				}
				ClientResult<bool> clientResult = ServerSettings.IsSharePointOnline(context);
				context.ExecuteQuery();
				value = clientResult.Value;
			}
			catch (ServerException serverException1)
			{
				ServerException serverException = serverException1;
				Utils.LogExceptionDetails(serverException, MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name, null);
				if (!serverException.MethodDoesNotExist())
				{
					throw;
				}
				else
				{
					value = false;
				}
			}
			return value;
		}

		private static bool MethodDoesNotExist(this ServerException exception)
		{
			if (exception == null || string.IsNullOrEmpty(exception.Message))
			{
				return false;
			}
			return string.Equals(exception.Message, string.Format("Method \"{0}\" does not exist.", "IsSharePointOnline"), StringComparison.OrdinalIgnoreCase);
		}
	}
}