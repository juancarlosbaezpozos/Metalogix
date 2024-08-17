using System;
using System.Threading;

namespace Metalogix.Threading
{
    public class TaskDefinition<T> : TaskDefinition
    {
        private T m_oWorkInstance;

        private ThreadedOperationDelegate<T> m_genericExecution;

        public TaskDefinition(T instance, ThreadedOperationDelegate<T> execution, object[] parameters, bool bReuseable)
            : base(null, parameters, bReuseable)
        {
            this.m_oWorkInstance = instance;
            this.m_genericExecution = execution;
        }

        public TaskDefinition(T instance, ThreadedOperationDelegate<T> execution, object[] parameters) : base(null,
            parameters)
        {
            this.m_oWorkInstance = instance;
            this.m_genericExecution = execution;
        }

        public override void Execute()
        {
            if (!base.MarkAsExecuted() || this.m_bReuseable)
            {
                if (this.m_waitHandle == null)
                {
                    this.m_waitHandle = new ManualResetEvent(false);
                }

                try
                {
                    this.m_genericExecution(this.m_oWorkInstance, this.m_oParams);
                }
                finally
                {
                    this.m_waitHandle.Set();
                    base.FireFinished();
                    lock (this.m_oLockWaitHandle)
                    {
                        this.m_waitHandle.Close();
                        this.m_waitHandle = null;
                    }
                }
            }
            else
            {
                base.FireFinished();
            }
        }
    }
}