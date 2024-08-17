using Metalogix;
using Metalogix.Core;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Globalization;

namespace Metalogix.UI.WinForms
{
	public class UIConfigurationVariables : ConfigurationVariables
	{
		private static ConfigurationVariables.ConfigurationVariable<bool> s_enableStickySettings;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showProductVersionInAppTitle;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showMarketingSplashScreen;

		private static ConfigurationVariables.ConfigurationVariable<int> s_marketingSplashCountdown;

		private static ConfigurationVariables.ConfigurationVariable<string> s_lastMarketingSplashProductShown;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showAdvanced;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showPowerShellWarning;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showWordPressConnection;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showAtomFeedConnection;

		private static ConfigurationVariables.ConfigurationVariable<bool> s_showBloggerConnection;

		private static ConfigurationVariables.ConfigurationVariable<int> _daysToSuppressLicenseAlertKey;

		private static ConfigurationVariables.ConfigurationVariable<string> _dateToShowLicenseAlertKey;

		private static ConfigurationVariables.ConfigurationVariable<double> _daysToShowLicenseExpiryWarning;

		private static ConfigurationVariables.ConfigurationVariable<bool> _showNintexAppWarning;

		private static bool? s_bShowMarketingSplashScreen;

		private static int? s_iMarketingSplashCountdown;

		private static string s_strLastMarketingSplashProductShown;

		private static bool? s_bShowAdvanced;

		private static bool? _bShowNintexAppWarning;

		public static string DateToShowLicenseAlert
		{
			get
			{
				return UIConfigurationVariables._dateToShowLicenseAlertKey.GetValue<string>();
			}
			set
			{
				UIConfigurationVariables._dateToShowLicenseAlertKey.SetValue(value);
			}
		}

		public static double DaysToShowLicenseExpiryWarning
		{
			get
			{
				return UIConfigurationVariables._daysToShowLicenseExpiryWarning.GetValue<double>();
			}
			set
			{
				UIConfigurationVariables._daysToShowLicenseExpiryWarning.SetValue(value);
			}
		}

		public static int DaysToSuppressLicenseAlert
		{
			get
			{
				return UIConfigurationVariables._daysToSuppressLicenseAlertKey.GetValue<int>();
			}
			set
			{
				UIConfigurationVariables._daysToSuppressLicenseAlertKey.SetValue(value);
			}
		}

		public static bool EnableStickySettings
		{
			get
			{
				return UIConfigurationVariables.s_enableStickySettings.GetValue<bool>();
			}
			set
			{
				UIConfigurationVariables.s_enableStickySettings.SetValue(value);
			}
		}

		public static string LastMarketingSplashProductShown
		{
			get
			{
				string sStrLastMarketingSplashProductShown = UIConfigurationVariables.s_strLastMarketingSplashProductShown;
				if (sStrLastMarketingSplashProductShown == null)
				{
					sStrLastMarketingSplashProductShown = UIConfigurationVariables.s_lastMarketingSplashProductShown.GetValue<string>();
					UIConfigurationVariables.s_strLastMarketingSplashProductShown = sStrLastMarketingSplashProductShown;
				}
				return sStrLastMarketingSplashProductShown;
			}
			set
			{
				UIConfigurationVariables.s_strLastMarketingSplashProductShown = value;
				UIConfigurationVariables.s_lastMarketingSplashProductShown.SetValue(value);
			}
		}

		public static int MarketingSplashCountdown
		{
			get
			{
				if (!UIConfigurationVariables.s_iMarketingSplashCountdown.HasValue)
				{
					UIConfigurationVariables.s_iMarketingSplashCountdown = new int?(UIConfigurationVariables.s_marketingSplashCountdown.GetValue<int>());
				}
				return UIConfigurationVariables.s_iMarketingSplashCountdown.Value;
			}
			set
			{
				UIConfigurationVariables.s_iMarketingSplashCountdown = new int?(value);
				UIConfigurationVariables.s_marketingSplashCountdown.SetValue(value);
			}
		}

		public static bool ShowAdvanced
		{
			get
			{
				if (!UIConfigurationVariables.s_bShowAdvanced.HasValue)
				{
					UIConfigurationVariables.s_bShowAdvanced = new bool?((UIConfigurationVariables.s_showAdvanced.GetValue<bool>() ? true : !UIApplication.INSTANCE.ShowAdvancedModeToggleButton));
				}
				return UIConfigurationVariables.s_bShowAdvanced.Value;
			}
			set
			{
				UIConfigurationVariables.s_bShowAdvanced = new bool?(value);
				UIConfigurationVariables.s_showAdvanced.SetValue(value);
			}
		}

		public static bool ShowAtomFeedConnection
		{
			get
			{
				return UIConfigurationVariables.s_showAtomFeedConnection.GetValue<bool>();
			}
			set
			{
				UIConfigurationVariables.s_showAtomFeedConnection.SetValue(value);
			}
		}

		public static bool ShowBloggerConnection
		{
			get
			{
				return UIConfigurationVariables.s_showBloggerConnection.GetValue<bool>();
			}
			set
			{
				UIConfigurationVariables.s_showBloggerConnection.SetValue(value);
			}
		}

		public static bool ShowMarketingSplashScreen
		{
			get
			{
				if (!UIConfigurationVariables.s_bShowMarketingSplashScreen.HasValue)
				{
					UIConfigurationVariables.s_bShowMarketingSplashScreen = new bool?(UIConfigurationVariables.s_showMarketingSplashScreen.GetValue<bool>());
				}
				return UIConfigurationVariables.s_bShowMarketingSplashScreen.Value;
			}
			set
			{
				UIConfigurationVariables.s_bShowMarketingSplashScreen = new bool?(value);
				UIConfigurationVariables.s_showMarketingSplashScreen.SetValue(value);
			}
		}

		public static bool ShowNintexAppWarning
		{
			get
			{
				if (!UIConfigurationVariables._bShowNintexAppWarning.HasValue)
				{
					UIConfigurationVariables._bShowNintexAppWarning = new bool?(UIConfigurationVariables._showNintexAppWarning.GetValue<bool>());
				}
				return UIConfigurationVariables._bShowNintexAppWarning.Value;
			}
			set
			{
				UIConfigurationVariables._bShowNintexAppWarning = new bool?(value);
				UIConfigurationVariables._showNintexAppWarning.SetValue(value);
			}
		}

		public static IConfigurationVariable ShowPowerShellWarning
		{
			get
			{
				return UIConfigurationVariables.s_showPowerShellWarning;
			}
		}

		public static bool ShowProductVersionInAppTitle
		{
			get
			{
				return UIConfigurationVariables.s_showProductVersionInAppTitle.GetValue<bool>();
			}
		}

		public static bool ShowWordPressConnection
		{
			get
			{
				return UIConfigurationVariables.s_showWordPressConnection.GetValue<bool>();
			}
			set
			{
				UIConfigurationVariables.s_showWordPressConnection.SetValue(value);
			}
		}

		static UIConfigurationVariables()
		{
			UIConfigurationVariables.s_enableStickySettings = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, Resources.EnableStickySettingsKey, true);
			UIConfigurationVariables.s_showProductVersionInAppTitle = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.UserSpecific, Resources.ShowProductVersionInAppTitleKey, true);
			UIConfigurationVariables.s_showMarketingSplashScreen = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.UserSpecific, Resources.ShowMarketingSplashScreenKey, true);
			UIConfigurationVariables.s_marketingSplashCountdown = new ConfigurationVariables.ConfigurationVariable<int>(ResourceScope.UserSpecific, Resources.MarketingSplashCountdownKey, 5);
			UIConfigurationVariables.s_lastMarketingSplashProductShown = new ConfigurationVariables.ConfigurationVariable<string>(ResourceScope.UserSpecific, Resources.LastMarketingSplashProductShownKey, "StoragePoint");
			UIConfigurationVariables.s_showAdvanced = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, Resources.ShowAdvancedKey, false);
			UIConfigurationVariables.s_showPowerShellWarning = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.UserSpecific, "ShowPowerShellWarning", true);
			UIConfigurationVariables.s_showWordPressConnection = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, Resources.ShowWordPressConnectionKey, false);
			UIConfigurationVariables.s_showAtomFeedConnection = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, Resources.ShowAtomFeedConnectionKey, false);
			UIConfigurationVariables.s_showBloggerConnection = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, Resources.ShowBloggerConnectionKey, false);
			UIConfigurationVariables._daysToSuppressLicenseAlertKey = new ConfigurationVariables.ConfigurationVariable<int>(ResourceScope.EnvironmentSpecific, Resources.DaysToSuppressLicenseAlertKey, 30);
			string dateToShowLicenseAlertKey = Resources.DateToShowLicenseAlertKey;
			DateTime minValue = DateTime.MinValue;
			UIConfigurationVariables._dateToShowLicenseAlertKey = new ConfigurationVariables.ConfigurationVariable<string>(ResourceScope.EnvironmentSpecific, dateToShowLicenseAlertKey, minValue.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));
			UIConfigurationVariables._daysToShowLicenseExpiryWarning = new ConfigurationVariables.ConfigurationVariable<double>(ResourceScope.EnvironmentSpecific, Resources.DaysToShowLicenseExpiryWarningKey, 90);
			UIConfigurationVariables._showNintexAppWarning = new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.UserSpecific, "ShowNintexAppWarning", true);
			UIConfigurationVariables.s_bShowMarketingSplashScreen = null;
			UIConfigurationVariables.s_iMarketingSplashCountdown = null;
			UIConfigurationVariables.s_strLastMarketingSplashProductShown = null;
			UIConfigurationVariables.s_bShowAdvanced = null;
			UIConfigurationVariables._bShowNintexAppWarning = null;
		}

		public UIConfigurationVariables()
		{
		}
	}
}