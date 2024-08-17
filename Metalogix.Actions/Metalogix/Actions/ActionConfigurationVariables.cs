using Metalogix;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using Metalogix.Jobs;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    public class ActionConfigurationVariables : ConfigurationVariables
    {
        private static ConfigurationVariables.ConfigurationVariable<float> s_percentageCPUUsageThreshold;

        private static ConfigurationVariables.ConfigurationVariable<string> s_jobTemplateResolver;

        private static AdapterCallWrapper m_defaultJobHistoryCallWrapper;

        private static float? s_fPercentageCPUUsageThreshold;

        private static string s_sJobTemplateResolver;

        public static AdapterCallWrapper DefaultJobHistoryCallWrapper
        {
            get
            {
                AdapterCallWrapper mDefaultJobHistoryCallWrapper =
                    ActionConfigurationVariables.m_defaultJobHistoryCallWrapper;
                if (mDefaultJobHistoryCallWrapper == null)
                {
                    mDefaultJobHistoryCallWrapper = (AdapterCallWrapperAction wrapperAction) => wrapperAction();
                    ActionConfigurationVariables.m_defaultJobHistoryCallWrapper = mDefaultJobHistoryCallWrapper;
                }

                return mDefaultJobHistoryCallWrapper;
            }
            set { ActionConfigurationVariables.m_defaultJobHistoryCallWrapper = value; }
        }

        public static bool EnableCustomTransformers
        {
            get { return ConfigurationVariables.GetConfigurationValue<bool>(Resources.EnableCustomTransformersKey); }
            set { ConfigurationVariables.SetConfigurationValue<bool>(Resources.EnableCustomTransformersKey, value); }
        }

        public static string JobTemplateResolver
        {
            get
            {
                if (ActionConfigurationVariables.s_sJobTemplateResolver == null)
                {
                    ActionConfigurationVariables.s_sJobTemplateResolver =
                        ActionConfigurationVariables.s_jobTemplateResolver.GetValue<string>();
                }

                return ActionConfigurationVariables.s_sJobTemplateResolver;
            }
            set
            {
                ActionConfigurationVariables.s_sJobTemplateResolver = value;
                ActionConfigurationVariables.s_jobTemplateResolver.SetValue(value);
            }
        }

        public static float PercentageCPUUsageThreshold
        {
            get
            {
                if (!ActionConfigurationVariables.s_fPercentageCPUUsageThreshold.HasValue)
                {
                    ActionConfigurationVariables.s_fPercentageCPUUsageThreshold =
                        new float?(ActionConfigurationVariables.s_percentageCPUUsageThreshold.GetValue<float>());
                }

                return (float)ActionConfigurationVariables.s_fPercentageCPUUsageThreshold.Value;
            }
            set
            {
                ActionConfigurationVariables.s_fPercentageCPUUsageThreshold = new float?(value);
                ActionConfigurationVariables.s_percentageCPUUsageThreshold.SetValue(value);
            }
        }

        public static int ThreadsPerActionLimit
        {
            get { return ConfigurationVariables.GetConfigurationValue<int>(Resources.PerActionResourceUseKey); }
            set { ConfigurationVariables.SetConfigurationValue<int>(Resources.PerActionResourceUseKey, value); }
        }

        static ActionConfigurationVariables()
        {
            ActionConfigurationVariables.s_percentageCPUUsageThreshold =
                new ConfigurationVariables.ConfigurationVariable<float>(ResourceScope.ApplicationAndUserSpecific,
                    Resources.PercentageCPUUsageKey, 50f);
            ActionConfigurationVariables.s_jobTemplateResolver =
                new ConfigurationVariables.ConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                    Resources.JobTemplateResolver, "");
            ActionConfigurationVariables.s_fPercentageCPUUsageThreshold = null;
            ActionConfigurationVariables.s_sJobTemplateResolver = null;
            ApplicationData.MainAssemblyChanged += new EventHandler(ActionConfigurationVariables.MainAssemblyChanged);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                Resources.EnableCustomTransformersKey, true);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                Resources.PerActionResourceUseKey, Environment.ProcessorCount * 2);
        }

        public ActionConfigurationVariables()
        {
        }

        internal static new void ClearApplicationVariables()
        {
            ActionConfigurationVariables.s_fPercentageCPUUsageThreshold = null;
            ConfigurationVariables.FireConfigurationVariablesChanged(null);
        }

        public static void EnsureNonUIEnvironmentVariables()
        {
            int threadsPerActionLimit = ActionConfigurationVariables.ThreadsPerActionLimit;
        }

        private static void MainAssemblyChanged(object sender, EventArgs e)
        {
            ActionConfigurationVariables.ClearApplicationVariables();
        }
    }
}