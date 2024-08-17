using Metalogix;
using Metalogix.Core;
using System;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public sealed class HealthScoreBlockerSettings
	{
		private const string ServerHealthScoreRestartKey = "ServerHealthScoreRestartValue";

		private const string ServerHealthScorePauseKey = "ServerHealthScorePauseValue";

		public const int DefaultServerHealthBlockerRestartValue = 3;

		public const int DefaultServerHealthBlockerPauseValue = 9;

		public const int MinimumServerHealthPauseValue = 0;

		public const int MaximumServerHealthPauseValue = 9;

		public const int MinimumServerHealthRestartValue = 0;

		public const int MaximumServerHealthRestartValue = 5;

		public const int HealthScoreCheckInterval = 60000;

		private int ServerHealthPauseValue
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("ServerHealthScorePauseValue");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("ServerHealthScorePauseValue", value);
			}
		}

		private int ServerHealthRestartValue
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("ServerHealthScoreRestartValue");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("ServerHealthScoreRestartValue", value);
			}
		}

		static HealthScoreBlockerSettings()
		{
			HealthScoreBlockerSettings.InitializeServerHealthScoreRestartSetting();
			HealthScoreBlockerSettings.InitializeServerHealthScorePauseSetting();
		}

		public HealthScoreBlockerSettings()
		{
		}

		public int GetPauseValueUpToMaxValue()
		{
			int serverHealthPauseValue = this.ServerHealthPauseValue;
			if (serverHealthPauseValue < 0)
			{
				return 0;
			}
			if (serverHealthPauseValue > 9)
			{
				return 9;
			}
			return serverHealthPauseValue;
		}

		public int GetRestartValueUpToMaxValue()
		{
			int serverHealthRestartValue = this.ServerHealthRestartValue;
			if (serverHealthRestartValue < 0)
			{
				return 0;
			}
			if (serverHealthRestartValue > 5)
			{
				return 5;
			}
			return serverHealthRestartValue;
		}

		private static void InitializeServerHealthScorePauseSetting()
		{
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.User, "ServerHealthScorePauseValue", 9);
		}

		private static void InitializeServerHealthScoreRestartSetting()
		{
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.User, "ServerHealthScoreRestartValue", 3);
		}
	}
}