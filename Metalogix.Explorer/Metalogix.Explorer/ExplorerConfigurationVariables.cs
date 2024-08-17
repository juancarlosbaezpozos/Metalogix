using Metalogix;
using Metalogix.Core;
using Metalogix.Explorer.Properties;
using System;

namespace Metalogix.Explorer
{
    public class ExplorerConfigurationVariables : ConfigurationVariables
    {
        private static ConfigurationVariables.ConfigurationVariable<bool> s_upgradeActiveConnections;

        public static bool UpgradeActiveConnections
        {
            get { return ExplorerConfigurationVariables.s_upgradeActiveConnections.GetValue<bool>(); }
            set { ExplorerConfigurationVariables.s_upgradeActiveConnections.SetValue(value); }
        }

        static ExplorerConfigurationVariables()
        {
            ExplorerConfigurationVariables.s_upgradeActiveConnections =
                new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                    Resources.UpgradeActiveConnectionsKey, true);
        }

        public ExplorerConfigurationVariables()
        {
        }
    }
}