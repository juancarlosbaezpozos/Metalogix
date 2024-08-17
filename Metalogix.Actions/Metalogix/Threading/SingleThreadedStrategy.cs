using System;
using System.Threading;

namespace Metalogix.Threading
{
    public class SingleThreadedStrategy : IThreadingStrategy
    {
        private static SingleThreadedStrategy s_instance;

        private int m_iMaxThreadCount = 1;

        private bool m_bThreadingEnabled;

        public static SingleThreadedStrategy Instance
        {
            get
            {
                if (SingleThreadedStrategy.s_instance == null)
                {
                    SingleThreadedStrategy.s_instance = new SingleThreadedStrategy();
                }

                return SingleThreadedStrategy.s_instance;
            }
        }

        public int MaxThreadsCount
        {
            get { return this.m_iMaxThreadCount; }
            set { }
        }

        public bool ThreadingEnabled
        {
            get { return this.m_bThreadingEnabled; }
            set { }
        }

        public SingleThreadedStrategy()
        {
        }

        public bool AllowThreadAllocation(ThreadManager manager)
        {
            return manager.ActiveThreads == 0;
        }

        public event Metalogix.Threading.ThreadingStrategyStateChanged ThreadingStrategyStateChanged;

        public event Metalogix.Threading.ThreadingStrategyStateChanged ThreadStateChanged;
    }
}