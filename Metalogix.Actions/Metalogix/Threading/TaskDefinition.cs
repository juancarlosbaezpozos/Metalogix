using System;
using System.Reflection;
using System.Threading;

namespace Metalogix.Threading
{
    public class TaskDefinition
    {
        protected ThreadPriority? m_requestedPriority = null;

        protected ThreadedOperationDelegate m_execution;

        protected object[] m_oParams;

        protected bool m_bReuseable;

        private bool m_bExecuted;

        private object m_oLockExecuted = new object();

        protected object m_oLockWaitHandle = new object();

        protected EventWaitHandle m_waitHandle = new ManualResetEvent(false);

        public string Name
        {
            get { return this.m_execution.Method.Name; }
        }

        public ThreadPriority? RequestedPriority
        {
            get { return this.m_requestedPriority; }
            set { this.m_requestedPriority = value; }
        }

        public TaskDefinition(ThreadedOperationDelegate execution, object[] parameters)
        {
            this.Instantiate(execution, parameters, false);
        }

        public TaskDefinition(ThreadedOperationDelegate execution, object[] parameters, bool bReuseable)
        {
            this.Instantiate(execution, parameters, bReuseable);
        }

        public virtual void Execute()
        {
            if (!this.MarkAsExecuted() || this.m_bReuseable)
            {
                if (this.m_waitHandle == null)
                {
                    this.m_waitHandle = new ManualResetEvent(false);
                }

                try
                {
                    this.m_execution(this.m_oParams);
                }
                finally
                {
                    this.m_waitHandle.Set();
                    this.FireFinished();
                    lock (this.m_oLockWaitHandle)
                    {
                        this.m_waitHandle.Close();
                        this.m_waitHandle = null;
                    }
                }
            }
            else
            {
                this.FireFinished();
            }
        }

        protected void FireFinished()
        {
            if (this.Finished != null)
            {
                this.Finished(this);
            }
        }

        private void Instantiate(ThreadedOperationDelegate execution, object[] parameters, bool bReuseable)
        {
            this.m_execution = execution;
            this.m_oParams = parameters;
            this.m_bReuseable = bReuseable;
        }

        protected bool MarkAsExecuted()
        {
            bool flag;
            lock (this.m_oLockExecuted)
            {
                bool mBExecuted = this.m_bExecuted;
                this.m_bExecuted = true;
                flag = mBExecuted;
            }

            return flag;
        }

        public void WaitOne()
        {
            lock (this.m_oLockWaitHandle)
            {
                if (this.m_waitHandle != null)
                {
                    this.m_waitHandle.WaitOne();
                }
            }
        }

        public event TaskFinished Finished;
    }
}