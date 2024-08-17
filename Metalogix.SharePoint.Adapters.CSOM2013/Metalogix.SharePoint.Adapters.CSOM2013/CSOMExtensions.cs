using Microsoft.Online.SharePoint.TenantAdministration;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public static class CSOMExtensions
	{
		private const int DefaultSleepInterval = 30000;

		private const int DefaultMaxRetries = 10;

		private const int MaxSleepInterval = 48000;

		public static void ExecuteWithRetry(Func<bool> methodToRetry, int sleepInterval, int maxRetries)
		{
			if (sleepInterval <= 0)
			{
				sleepInterval = 30000;
			}
			if (maxRetries <= 0)
			{
				maxRetries = 10;
			}
			if (sleepInterval * maxRetries > 480000)
			{
				sleepInterval = 48000;
				maxRetries = 10;
			}
			int num = 0;
			do
			{
				if (num >= maxRetries)
				{
					return;
				}
				Thread.Sleep(sleepInterval);
				num++;
			}
			while (!methodToRetry());
		}

		public static void WaitUntilOperationCompleted(this SpoOperation operation, Action completionAction)
		{
			SpoOperationMonitor spoOperationMonitor = new SpoOperationMonitor(operation);
			if (completionAction != null)
			{
				spoOperationMonitor.Completed += new EventHandler((object sender, EventArgs args) => completionAction());
			}
			spoOperationMonitor.WaitUntilOperationCompleted();
		}

		public static void WaitUntilOperationCompleted(this SpoOperation operation)
		{
			(new SpoOperationMonitor(operation)).WaitUntilOperationCompleted();
		}
	}
}