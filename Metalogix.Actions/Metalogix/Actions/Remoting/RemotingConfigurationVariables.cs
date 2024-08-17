using Metalogix;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using System;

namespace Metalogix.Actions.Remoting
{
    public class RemotingConfigurationVariables : ConfigurationVariables
    {
        private static ConfigurationVariables.ConfigurationVariable<int> _remotePowerShellTimerInterval;

        private static ConfigurationVariables.ConfigurationVariable<string> _remotePowerShellScriptFilesLocation;

        public static string RemotePowerShellScriptFilesLocation
        {
            get { return RemotingConfigurationVariables._remotePowerShellScriptFilesLocation.GetValue<string>(); }
            set { RemotingConfigurationVariables._remotePowerShellScriptFilesLocation.SetValue(value); }
        }

        public static int RemotePowerShellTimerInterval
        {
            get { return RemotingConfigurationVariables._remotePowerShellTimerInterval.GetValue<int>(); }
            set { RemotingConfigurationVariables._remotePowerShellTimerInterval.SetValue(value); }
        }

        static RemotingConfigurationVariables()
        {
            RemotingConfigurationVariables._remotePowerShellTimerInterval =
                new ConfigurationVariables.ConfigurationVariable<int>(ResourceScope.EnvironmentSpecific,
                    Resources.RemotePowerShellTimerIntervalKey, 15000);
            RemotingConfigurationVariables._remotePowerShellScriptFilesLocation =
                new ConfigurationVariables.ConfigurationVariable<string>(ResourceScope.EnvironmentSpecific,
                    Resources.RemotePowerShellScriptFilesLocationKey, "C:\\MetalogixScripts\\");
        }

        public RemotingConfigurationVariables()
        {
        }
    }
}