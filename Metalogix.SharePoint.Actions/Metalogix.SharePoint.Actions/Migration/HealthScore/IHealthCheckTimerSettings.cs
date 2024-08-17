using System;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public interface IHealthCheckTimerSettings
	{
		bool ServerHealthCheckEnabled
		{
			get;
			set;
		}

		int ServerHealthCheckInterval
		{
			get;
			set;
		}
	}
}