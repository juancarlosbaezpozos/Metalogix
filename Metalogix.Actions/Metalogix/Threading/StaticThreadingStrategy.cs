using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using System;
using System.Threading;

namespace Metalogix.Threading
{
    public class StaticThreadingStrategy : IThreadingStrategy
    {
        private static StaticThreadingStrategy s_instance;

        private bool m_bThreadingEnabled = true;

        public static StaticThreadingStrategy Instance
        {
            get
            {
                if (StaticThreadingStrategy.s_instance == null)
                {
                    StaticThreadingStrategy.s_instance = new StaticThreadingStrategy();
                }

                return StaticThreadingStrategy.s_instance;
            }
        }

        public int MaxThreadsCount
        {
            get { return ActionConfigurationVariables.ThreadsPerActionLimit; }
            set { ActionConfigurationVariables.ThreadsPerActionLimit = value; }
        }

        public bool ThreadingEnabled
        {
            get
            {
                if (!this.m_bThreadingEnabled)
                {
                    return false;
                }

                return this.MaxThreadsCount > 0;
            }
            set { this.m_bThreadingEnabled = value; }
        }

        private StaticThreadingStrategy()
        {
            ConfigurationVariables.ConfigurationVariablesChanged +=
                new ConfigurationVariables.ConfigurationVariablesChangedHander(this.On_EnvironmentVariables_Changed);
        }

        public bool AllowThreadAllocation(ThreadManager manager)
        {
            if (this.ThreadingEnabled && manager.ActiveThreads < this.MaxThreadsCount || manager.ActiveThreads == 0)
            {
                return true;
            }

            return false;
        }

        private void On_EnvironmentVariables_Changed(object sender,
            ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs_0)
        {
            string variableName = configVarsChangedArgs_0.VariableName;
            if ((string.IsNullOrEmpty(variableName) || variableName == Resources.PerActionResourceUseKey) &&
                this.ThreadingStrategyStateChanged != null)
            {
                this.ThreadingStrategyStateChanged();
            }
        }

        public event Metalogix.Threading.ThreadingStrategyStateChanged ThreadingStrategyStateChanged;
    }
}