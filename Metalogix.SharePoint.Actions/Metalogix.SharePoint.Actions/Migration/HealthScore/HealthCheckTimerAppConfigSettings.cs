using Metalogix;
using Metalogix.Core;
using System;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public sealed class HealthCheckTimerAppConfigSettings : IHealthCheckTimerSettings
	{
		private const string ServerHealthCheckEnabledKey = "ServerHealthLoggingEnabled";

		private const bool DefaultEnableServerHealthCheck = false;

		private const string ServerHealthCheckIntervalKey = "ServerHealthLoggingInterval";

		private const int DefaultCheckInterval = 300000;

		public bool ServerHealthCheckEnabled
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("ServerHealthLoggingEnabled");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("ServerHealthLoggingEnabled", value);
			}
		}

		public int ServerHealthCheckInterval
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("ServerHealthLoggingInterval");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("ServerHealthLoggingInterval", value);
			}
		}

		static HealthCheckTimerAppConfigSettings()
		{
			HealthCheckTimerAppConfigSettings.InitializeServerHealthCheckEnabledSetting();
			HealthCheckTimerAppConfigSettings.InitializeServerHealthCheckIntervalSetting();
		}

		public HealthCheckTimerAppConfigSettings()
		{
		}

		private static void InitializeServerHealthCheckEnabledSetting()
		{
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.UserSpecific, "ServerHealthLoggingEnabled", false);
		}

		private static void InitializeServerHealthCheckIntervalSetting()
		{
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.UserSpecific, "ServerHealthLoggingInterval", 300000);
		}
	}
}