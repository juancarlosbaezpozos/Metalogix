using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using System;
using System.Diagnostics;
using System.Threading;

namespace Metalogix.Threading
{
    public class DynamicThreadingStrategy : IThreadingStrategy
    {
        private static DynamicThreadingStrategy s_instance;

        private static Timer s_timer;

        private static object o_totalCounterLock;

        private static PerformanceCounter s_totalCounter;

        private static object o_processCounterLock;

        private static PerformanceCounter s_processCounter;

        private static Process currentProcess;

        private static TimeSpan lastTotalProcessSpan;

        private static TimeSpan lastUserProcessSpan;

        private static DateTime lastChecked;

        private static volatile float s_percentageUsedTotal;

        private static volatile float s_percentageUsedUser;

        public static DynamicThreadingStrategy Instance
        {
            get
            {
                if (DynamicThreadingStrategy.s_instance == null)
                {
                    DynamicThreadingStrategy.s_instance = new DynamicThreadingStrategy();
                    DynamicThreadingStrategy.s_timer =
                        new Timer(new TimerCallback(DynamicThreadingStrategy.CalculatePercentCPUUsed), null, 0, 10000);
                }

                return DynamicThreadingStrategy.s_instance;
            }
        }

        protected float PercentageCPUUsedTotal
        {
            get { return DynamicThreadingStrategy.s_percentageUsedTotal; }
        }

        protected float PercentageCPUUsedUser
        {
            get { return DynamicThreadingStrategy.s_percentageUsedUser; }
        }

        private static PerformanceCounter ProcessCPUCounter
        {
            get
            {
                lock (DynamicThreadingStrategy.o_processCounterLock)
                {
                    if (DynamicThreadingStrategy.s_processCounter == null)
                    {
                        DynamicThreadingStrategy.s_processCounter = new PerformanceCounter()
                        {
                            CategoryName = "Processor",
                            CounterName = "% Processor Time"
                        };
                    }
                }

                return DynamicThreadingStrategy.s_processCounter;
            }
        }

        public bool ThreadingEnabled
        {
            get { return true; }
            set { }
        }

        private static PerformanceCounter TotalCPUCounter
        {
            get
            {
                lock (DynamicThreadingStrategy.o_totalCounterLock)
                {
                    if (DynamicThreadingStrategy.s_totalCounter == null)
                    {
                        DynamicThreadingStrategy.s_totalCounter = new PerformanceCounter()
                        {
                            CategoryName = "Processor",
                            CounterName = "% Processor Time",
                            InstanceName = "_Total"
                        };
                    }
                }

                return DynamicThreadingStrategy.s_totalCounter;
            }
        }

        static DynamicThreadingStrategy()
        {
            DynamicThreadingStrategy.s_instance = null;
            DynamicThreadingStrategy.s_timer = null;
            DynamicThreadingStrategy.o_totalCounterLock = new object();
            DynamicThreadingStrategy.s_totalCounter = null;
            DynamicThreadingStrategy.o_processCounterLock = new object();
            DynamicThreadingStrategy.s_processCounter = null;
            DynamicThreadingStrategy.currentProcess = Process.GetCurrentProcess();
            DynamicThreadingStrategy.lastTotalProcessSpan = DynamicThreadingStrategy.currentProcess.TotalProcessorTime;
            DynamicThreadingStrategy.lastUserProcessSpan = DynamicThreadingStrategy.currentProcess.UserProcessorTime;
            DynamicThreadingStrategy.lastChecked = DateTime.Now;
            DynamicThreadingStrategy.s_percentageUsedTotal = 0f;
            DynamicThreadingStrategy.s_percentageUsedUser = 0f;
        }

        private DynamicThreadingStrategy()
        {
            ConfigurationVariables.ConfigurationVariablesChanged +=
                new ConfigurationVariables.ConfigurationVariablesChangedHander(this.On_EnvironmentVariables_Changed);
            DynamicThreadingStrategy.ThreadingStrategyChanged +=
                new Metalogix.Threading.ThreadingStrategyStateChanged(this
                    .DynamicThreadingStrategy_ThreadingStrategyChanged);
        }

        public bool AllowThreadAllocation(ThreadManager threadManager)
        {
            if (threadManager.ActiveThreads == 0)
            {
                return true;
            }

            if (threadManager.ActiveThreads < ActionConfigurationVariables.ThreadsPerActionLimit &&
                this.PercentageCPUUsedTotal <= ActionConfigurationVariables.PercentageCPUUsageThreshold)
            {
                return true;
            }

            return false;
        }

        private static void CalculatePercentCPUUsed(object oValue)
        {
            TimeSpan now = DateTime.Now - DynamicThreadingStrategy.lastChecked;
            TimeSpan totalProcessorTime = DynamicThreadingStrategy.currentProcess.TotalProcessorTime;
            TimeSpan timeSpan = totalProcessorTime - DynamicThreadingStrategy.lastTotalProcessSpan;
            TimeSpan userProcessorTime = DynamicThreadingStrategy.currentProcess.UserProcessorTime;
            TimeSpan timeSpan1 = userProcessorTime - DynamicThreadingStrategy.lastUserProcessSpan;
            DynamicThreadingStrategy.lastChecked = DateTime.Now;
            DynamicThreadingStrategy.lastTotalProcessSpan = totalProcessorTime;
            DynamicThreadingStrategy.lastUserProcessSpan = userProcessorTime;
            float ticks = (float)(now.Ticks * (long)Environment.ProcessorCount);
            float single = (float)timeSpan.Ticks * 100f / ticks;
            float ticks1 = (float)timeSpan1.Ticks * 100f / ticks;
            if (DynamicThreadingStrategy.s_percentageUsedTotal != 0f)
            {
                DynamicThreadingStrategy.s_percentageUsedTotal =
                    (DynamicThreadingStrategy.s_percentageUsedTotal + single) / 2f;
                DynamicThreadingStrategy.s_percentageUsedUser =
                    (DynamicThreadingStrategy.s_percentageUsedUser + ticks1) / 2f;
            }
            else
            {
                DynamicThreadingStrategy.s_percentageUsedTotal = single;
                DynamicThreadingStrategy.s_percentageUsedUser = ticks1;
            }

            if (DynamicThreadingStrategy.ThreadingStrategyChanged != null)
            {
                DynamicThreadingStrategy.ThreadingStrategyChanged();
            }
        }

        private void DynamicThreadingStrategy_ThreadingStrategyChanged()
        {
            if (this.ThreadingStrategyStateChanged != null)
            {
                this.ThreadingStrategyStateChanged();
            }
        }

        private void On_EnvironmentVariables_Changed(object sender,
            ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs_0)
        {
            string variableName = configVarsChangedArgs_0.VariableName;
            if (string.IsNullOrEmpty(variableName) || variableName == Resources.PerActionResourceUseKey ||
                variableName == Resources.PercentageCPUUsageKey && this.ThreadingStrategyStateChanged != null)
            {
                this.ThreadingStrategyStateChanged();
            }
        }

        private static event Metalogix.Threading.ThreadingStrategyStateChanged ThreadingStrategyChanged;

        public event Metalogix.Threading.ThreadingStrategyStateChanged ThreadingStrategyStateChanged;
    }
}