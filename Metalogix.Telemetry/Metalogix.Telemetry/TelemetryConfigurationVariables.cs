using Metalogix;
using Metalogix.Core;
using Metalogix.Telemetry.Properties;
using System;

namespace Metalogix.Telemetry
{
    public class TelemetryConfigurationVariables : ConfigurationVariables
    {
        private static ConfigurationVariables.ConfigurationVariable<bool> _telemetryOptIn;

        public static bool TelemetryOptIn
        {
            get { return TelemetryConfigurationVariables._telemetryOptIn.GetValue<bool>(); }
            set { TelemetryConfigurationVariables._telemetryOptIn.SetValue(value); }
        }

        static TelemetryConfigurationVariables()
        {
            TelemetryConfigurationVariables._telemetryOptIn =
                new ConfigurationVariables.ConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                    Resources.TelemetryOptInKey, true);
        }

        public TelemetryConfigurationVariables()
        {
        }
    }
}