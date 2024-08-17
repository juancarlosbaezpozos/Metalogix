using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Metalogix.Threading
{
    public class WorkerThread : IDisposable
    {
        public static int Count;

        private volatile bool _doWork = true;

        private readonly object _lockHandle = new object();

        private readonly object _queueLock = new object();

        private readonly object _doWorkLock = new object();

        private EventWaitHandle _waitHandle;

        private readonly Thread _workerThread;

        private readonly Queue<TaskDefinition> _taskQueue;

        private bool _fireQueueFinishedEvent = true;

        internal bool FireQueueFinishedEventEvents
        {
            get { return this._fireQueueFinishedEvent; }
            set { this._fireQueueFinishedEvent = value; }
        }

        internal ThreadPriority Priority
        {
            get { return this._workerThread.Priority; }
            set
            {
                if (this._workerThread.Priority != value)
                {
                    this._workerThread.Priority = value;
                }
            }
        }

        public bool UsesCurrentThread
        {
            get { return Thread.CurrentThread == this._workerThread; }
        }

        public WorkerThread()
        {
            this._waitHandle = new ManualResetEvent(false);
            this._taskQueue = new Queue<TaskDefinition>();
            Thread thread = new Thread(new ThreadStart(this.Run))
            {
                Priority = ThreadPriority.BelowNormal
            };
            this._workerThread = thread;
            WorkerThread.Count++;
        }

        public WorkerThread(ThreadPriority priority)
        {
            this._waitHandle = new ManualResetEvent(false);
            this._taskQueue = new Queue<TaskDefinition>();
            Thread thread = new Thread(new ThreadStart(this.Run))
            {
                Priority = priority
            };
            this._workerThread = thread;
            WorkerThread.Count++;
        }

        public void Dispose()
        {
            if (Thread.CurrentThread == this._workerThread)
            {
                this.EndWorkerThread(null);
            }
            else
            {
                this.Enqueue(new TaskDefinition(new ThreadedOperationDelegate(this.EndWorkerThread), null));
                this._workerThread.Join();
            }

            WorkerThread.Count--;
        }

        public void EndWorkerThread(object oParams)
        {
            lock (this._doWorkLock)
            {
                this._doWork = false;
            }
        }

        public void Enqueue(TaskDefinition newTask)
        {
            lock (this._queueLock)
            {
                this._taskQueue.Enqueue(newTask);
            }

            if (this._workerThread.ThreadState == ThreadState.Unstarted)
            {
                this._workerThread.Start();
            }

            lock (this._lockHandle)
            {
                if (this._waitHandle != null)
                {
                    this._waitHandle.Set();
                }
            }
        }

        public void Enqueue(IList<TaskDefinition> newTasks)
        {
            lock (this._queueLock)
            {
                foreach (TaskDefinition newTask in newTasks)
                {
                    this._taskQueue.Enqueue(newTask);
                }
            }

            if (this._workerThread.ThreadState == ThreadState.Unstarted)
            {
                this._workerThread.Start();
            }

            lock (this._lockHandle)
            {
                if (this._waitHandle != null)
                {
                    this._waitHandle.Set();
                }
            }
        }

        private void FireQueueFinished()
        {
            if (this.FireQueueFinishedEventEvents && this.QueueFinished != null)
            {
                this.QueueFinished(this);
            }
        }

        private void FireTaskFailed(TaskDefinition task, Exception exception_0)
        {
            if (this.TaskFailed != null)
            {
                this.TaskFailed(this, task.Name, exception_0);
            }
        }

        private void ProcessQueue()
        {
            while (this._taskQueue.Count > 0)
            {
                TaskDefinition taskDefinition = null;
                lock (this._queueLock)
                {
                    if (this._taskQueue.Count > 0)
                    {
                        taskDefinition = this._taskQueue.Dequeue();
                    }
                }

                if (taskDefinition == null)
                {
                    continue;
                }

                try
                {
                    taskDefinition.Execute();
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    this.FireTaskFailed(taskDefinition, targetInvocationException.InnerException);
                }
                catch (Exception exception)
                {
                    this.FireTaskFailed(taskDefinition, exception);
                }
            }
        }

        private void Run()
        {
            bool flag;
            lock (this._doWorkLock)
            {
                flag = this._doWork;
            }

            while (flag)
            {
                this._waitHandle.WaitOne();
                try
                {
                    this.ProcessQueue();
                }
                finally
                {
                    this.FireQueueFinished();
                    lock (this._queueLock)
                    {
                        if (this._taskQueue.Count == 0)
                        {
                            lock (this._lockHandle)
                            {
                                this._waitHandle.Reset();
                            }
                        }
                    }

                    lock (this._doWorkLock)
                    {
                        flag = this._doWork;
                    }
                }
            }

            lock (this._lockHandle)
            {
                if (this._waitHandle != null)
                {
                    this._waitHandle.Close();
                    this._waitHandle = null;
                }
            }
        }

        public event Metalogix.Threading.QueueFinished QueueFinished;

        public event Metalogix.Threading.TaskFailed TaskFailed;
    }
}