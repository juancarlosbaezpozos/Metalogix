using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Core
{
    public static class GlobalConfigurationVariableSettings
    {
        public static bool ClearApplicationVariablesOnMainAssemblyChanged { get; set; }

        static GlobalConfigurationVariableSettings()
        {
            GlobalConfigurationVariableSettings.ClearApplicationVariablesOnMainAssemblyChanged = true;
        }
    }
}