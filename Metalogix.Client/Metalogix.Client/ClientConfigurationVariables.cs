using Metalogix;
using Metalogix.Client.Properties;
using Metalogix.Core;
using Metalogix.Deployment;
using System;

namespace Metalogix.Client
{
    public class ClientConfigurationVariables : ConfigurationVariables
    {
        private static ConfigurationVariables.ConfigurationVariable<AutomaticUpdaterSettings.AutoUpdateSettingType>
            s_autoUpdateSettings;

        private static ConfigurationVariables.ConfigurationVariable<string> s_autoUpdateSkipVersion;

        private static AutomaticUpdaterSettings.AutoUpdateSettingType? s_autoUpdateSetting;

        private static string s_sAutoUpdateSkipVersion;

        public static AutomaticUpdaterSettings.AutoUpdateSettingType AutoUpdateSettings
        {
            get
            {
                if (!ClientConfigurationVariables.s_autoUpdateSetting.HasValue)
                {
                    ClientConfigurationVariables.s_autoUpdateSetting =
                        new AutomaticUpdaterSettings.AutoUpdateSettingType?(ClientConfigurationVariables
                            .s_autoUpdateSettings.GetValue<AutomaticUpdaterSettings.AutoUpdateSettingType>());
                }

                return ClientConfigurationVariables.s_autoUpdateSetting.Value;
            }
            set
            {
                ClientConfigurationVariables.s_autoUpdateSetting =
                    new AutomaticUpdaterSettings.AutoUpdateSettingType?(value);
                ClientConfigurationVariables.s_autoUpdateSettings.SetValue(value);
            }
        }

        public static Version AutoUpdateSkipVersion
        {
            get
            {
                ClientConfigurationVariables.s_sAutoUpdateSkipVersion =
                    ClientConfigurationVariables.s_autoUpdateSkipVersion.GetValue<string>();
                if (string.IsNullOrEmpty(ClientConfigurationVariables.s_sAutoUpdateSkipVersion))
                {
                    return null;
                }

                return new Version(ClientConfigurationVariables.s_sAutoUpdateSkipVersion);
            }
            set
            {
                if (value != null)
                {
                    ClientConfigurationVariables.s_sAutoUpdateSkipVersion = value.ToString();
                }
                else
                {
                    ClientConfigurationVariables.s_sAutoUpdateSkipVersion = string.Empty;
                }

                ClientConfigurationVariables.s_autoUpdateSkipVersion.SetValue(ClientConfigurationVariables
                    .s_sAutoUpdateSkipVersion);
            }
        }

        static ClientConfigurationVariables()
        {
            ClientConfigurationVariables.s_autoUpdateSettings =
                new ConfigurationVariables.ConfigurationVariable<AutomaticUpdaterSettings.AutoUpdateSettingType>(
                    ResourceScope.UserSpecific, Resources.AutoUpdateSettingsKey,
                    AutomaticUpdaterSettings.AutoUpdateSettingType.AutoUpdate);
            ClientConfigurationVariables.s_autoUpdateSkipVersion =
                new ConfigurationVariables.ConfigurationVariable<string>(ResourceScope.UserSpecific,
                    Resources.AutoUpdateSkipVersionKey, string.Empty);
            ClientConfigurationVariables.s_autoUpdateSetting = null;
            ClientConfigurationVariables.s_sAutoUpdateSkipVersion = string.Empty;
        }

        public ClientConfigurationVariables()
        {
        }
    }
}