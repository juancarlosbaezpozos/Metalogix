using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Metalogix.Threading
{
    public class ThreadManager : IDisposable
    {
        public const string DISPOSE_TASK_KEY = "Dispose";

        private EventWaitHandle m_threadReleasedWaitHandle = new ManualResetEvent(false);

        private EventWaitHandle m_taskQueuedWaitHandle = new ManualResetEvent(false);

        private EventWaitHandle m_threadsActiveWaitHandle = new ManualResetEvent(true);

        private List<WorkerThread> m_activeThreads = new List<WorkerThread>();

        private Queue<WorkerThread> m_inactiveThreads = new Queue<WorkerThread>();

        private Dictionary<string, Queue<TaskDefinition>> m_bufferedTasks =
            new Dictionary<string, Queue<TaskDefinition>>();

        private Queue<TaskDefinition> m_queuedTasks = new Queue<TaskDefinition>();

        private List<TaskDefinition> m_activeQueueTasks = new List<TaskDefinition>();

        private List<string> m_openedGates = new List<string>();

        private List<string> m_openedRegexGates = new List<string>();

        private volatile bool m_bProcessQueue;

        private Thread m_queueThread;

        private object m_oLockMarshallingQueue = new object();

        private Dictionary<TaskDefinition, string> m_marshalledTaskKeys = new Dictionary<TaskDefinition, string>();

        private Dictionary<string, Queue<TaskDefinition>> m_marshalledTaskQueue =
            new Dictionary<string, Queue<TaskDefinition>>();

        private int? m_iBlockedThreadCount = new int?(0);

        private IThreadingStrategy m_threadStrategy;

        private QueueFinished m_workerThreadFinished;

        private TaskFailed m_workerThreadFailed;

        private TaskFinished m_queuedTaskFinished;

        private TaskFinished m_marshalledTaskFinished;

        public int ActiveThreads
        {
            get
            {
                int count;
                lock (this.m_activeThreads)
                {
                    count = this.m_activeThreads.Count;
                }

                return count;
            }
        }

        public bool ThreadingEnabled
        {
            get { return this.m_threadStrategy.ThreadingEnabled; }
        }

        public ThreadManager(IThreadingStrategy strategy)
        {
            this.m_threadStrategy = strategy;
            this.m_workerThreadFinished = new QueueFinished(this.On_WorkerThread_Finished);
            this.m_workerThreadFailed = new TaskFailed(this.On_WorkerThread_Failed);
            this.m_queuedTaskFinished = new TaskFinished(this.On_QueuedTask_Finished);
            this.m_marshalledTaskFinished = new TaskFinished(this.On_MarshalledTask_Finished);
            this.m_threadStrategy.ThreadingStrategyStateChanged +=
                new ThreadingStrategyStateChanged(this.On_ThreadStrategyState_Changed);
            this.m_taskQueuedWaitHandle.Reset();
            if (this.m_threadStrategy.ThreadingEnabled)
            {
                this.m_queueThread = new Thread(new ThreadStart(this.RunQueueProcessor))
                {
                    Priority = ThreadPriority.Lowest
                };
                this.m_bProcessQueue = true;
                this.m_queueThread.Start();
            }
        }

        private void AttachEvents(WorkerThread thread)
        {
            thread.QueueFinished += this.m_workerThreadFinished;
            thread.TaskFailed += this.m_workerThreadFailed;
        }

        public void DelayedSetBufferedTasks(TaskDefinition task, string sScopeKey, bool bReset, bool bWaitForTasks)
        {
            if (task == null)
            {
                this.SetBufferedTasks(sScopeKey, bReset, bWaitForTasks);
                return;
            }

            TaskDefinition[] taskDefinitionArray = new TaskDefinition[] { task };
            this.DelayedSetBufferedTasks(taskDefinitionArray, sScopeKey, bReset, bWaitForTasks);
        }

        public void DelayedSetBufferedTasks(ICollection<TaskDefinition> delayingTasks, string sScopeKey, bool bReset,
            bool bWaitForTasks)
        {
            if (delayingTasks == null || delayingTasks.Count == 0)
            {
                this.SetBufferedTasks(sScopeKey, bReset, bWaitForTasks);
                return;
            }

            object[] objArray = new object[] { delayingTasks, sScopeKey, bReset, bWaitForTasks };
            this.QueueTask(objArray, new ThreadedOperationDelegate(this.WaitAndSet));
        }

        private void DettachEvents(WorkerThread thread)
        {
            thread.QueueFinished -= this.m_workerThreadFinished;
            thread.TaskFailed -= this.m_workerThreadFailed;
        }

        public void Dispose()
        {
            bool flag = true;
            while (flag)
            {
                lock (this.m_activeThreads)
                {
                    flag = (this.m_activeThreads.Count > 0 ? true : this.m_iBlockedThreadCount.Value > 0);
                }

                if (!flag)
                {
                    continue;
                }

                this.m_threadsActiveWaitHandle.WaitOne(5000, false);
                Thread.Sleep(0);
            }

            this.m_bProcessQueue = false;
            if (!this.m_threadStrategy.ThreadingEnabled)
            {
                this.ProcessQueue();
            }
            else
            {
                this.RequestProcessQueue();
                this.m_queueThread.Join();
                this.m_queueThread = null;
            }

            flag = true;
            while (flag)
            {
                lock (this.m_activeThreads)
                {
                    flag = (this.m_activeThreads.Count > 0 ? true : this.m_iBlockedThreadCount.Value > 0);
                }

                if (!flag)
                {
                    continue;
                }

                this.m_threadsActiveWaitHandle.WaitOne(5000, false);
                Thread.Sleep(0);
            }

            this.m_threadStrategy.ThreadingStrategyStateChanged -=
                new ThreadingStrategyStateChanged(this.On_ThreadStrategyState_Changed);
            lock (this.m_inactiveThreads)
            {
                while (this.m_inactiveThreads.Count > 0)
                {
                    this.m_inactiveThreads.Dequeue().Dispose();
                }
            }

            lock (this.m_bufferedTasks)
            {
                this.m_bufferedTasks.Clear();
            }

            this.m_taskQueuedWaitHandle.Close();
            this.m_taskQueuedWaitHandle = null;
            this.m_threadReleasedWaitHandle.Close();
            this.m_threadReleasedWaitHandle = null;
            this.m_bufferedTasks = null;
            this.m_threadStrategy = null;
            this.m_activeThreads = null;
            this.m_inactiveThreads = null;
        }

        private void FireAsynchronousEventFailed(WorkerThread thread, string taskName, Exception exception_0)
        {
            if (this.AsynchronousTaskFailed != null)
            {
                this.AsynchronousTaskFailed(thread, taskName, exception_0);
            }
        }

        public void ForEach<T>(IEnumerable list, bool bWaitForTasks, object[] oParams,
            ThreadedOperationDelegate<T> operation)
        {
            this.ForEach<T>(list, bWaitForTasks, null, oParams, operation);
        }

        private void ForEach<T>(IEnumerable list, bool bWaitForTasks, ThreadPriority? threadPriority, object[] oParams,
            ThreadedOperationDelegate<T> operation)
        {
            Queue<TaskDefinition> taskDefinitions = new Queue<TaskDefinition>();
            foreach (T t in list)
            {
                TaskDefinition taskDefinition = new TaskDefinition<T>(t, operation, oParams)
                {
                    RequestedPriority = threadPriority
                };
                taskDefinitions.Enqueue(taskDefinition);
            }

            List<TaskDefinition> taskDefinitions1 = new List<TaskDefinition>(taskDefinitions.ToArray());
            while (taskDefinitions.Count > 0)
            {
                WorkerThread value = this.WaitForAvailableThread();
                TaskDefinition taskDefinition1 = taskDefinitions.Dequeue();
                ThreadPriority priority = value.Priority;
                if (taskDefinition1.RequestedPriority.HasValue)
                {
                    value.Priority = taskDefinition1.RequestedPriority.Value;
                }

                if (!value.UsesCurrentThread)
                {
                    value.Enqueue(taskDefinition1);
                }
                else
                {
                    taskDefinitions1.Remove(taskDefinition1);
                    try
                    {
                        taskDefinition1.Execute();
                    }
                    catch (Exception exception)
                    {
                        this.FireAsynchronousEventFailed(null, taskDefinition1.Name, exception);
                    }

                    value.Priority = priority;
                }
            }

            if (bWaitForTasks)
            {
                this.WaitForTasks(taskDefinitions1);
            }
        }

        private WorkerThread GetAvailableThread()
        {
            return this.GetAvailableThread(true);
        }

        private WorkerThread GetAvailableThread(bool bReturnCurrentIfWorker)
        {
            WorkerThread workerThread;
            if (this.m_threadStrategy == null)
            {
                return null;
            }

            WorkerThread workerThread1 = null;
            lock (this.m_activeThreads)
            {
                lock (this.m_inactiveThreads)
                {
                    if (!this.m_threadStrategy.AllowThreadAllocation(this))
                    {
                        while (this.m_inactiveThreads.Count > 0)
                        {
                            this.m_inactiveThreads.Dequeue().Dispose();
                        }

                        if (bReturnCurrentIfWorker)
                        {
                            foreach (WorkerThread mActiveThread in this.m_activeThreads)
                            {
                                if (!mActiveThread.UsesCurrentThread)
                                {
                                    continue;
                                }

                                workerThread1 = mActiveThread;
                                workerThread = workerThread1;
                                return workerThread;
                            }
                        }

                        this.m_threadReleasedWaitHandle.Reset();
                    }
                    else
                    {
                        if (this.m_inactiveThreads.Count > 0)
                        {
                            workerThread1 = this.m_inactiveThreads.Dequeue();
                        }

                        if (workerThread1 == null)
                        {
                            workerThread1 = new WorkerThread();
                        }

                        this.AttachEvents(workerThread1);
                        this.m_activeThreads.Add(workerThread1);
                        this.m_threadsActiveWaitHandle.Reset();
                    }
                }

                return workerThread1;
            }

            return workerThread;
        }

        private void On_MarshalledTask_Finished(TaskDefinition task)
        {
            task.Finished -= this.m_marshalledTaskFinished;
            lock (this.m_oLockMarshallingQueue)
            {
                if (this.m_marshalledTaskKeys.ContainsKey(task))
                {
                    string item = this.m_marshalledTaskKeys[task];
                    if (this.m_marshalledTaskQueue.ContainsKey(item))
                    {
                        Queue<TaskDefinition> taskDefinitions = this.m_marshalledTaskQueue[item];
                        if (taskDefinitions.Peek() != task)
                        {
                            throw new Exception("Task completed prior to tasks marshalled ahead of it on the queue");
                        }

                        taskDefinitions.Dequeue();
                        if (taskDefinitions.Count != 0)
                        {
                            this.QueueTask(taskDefinitions.Peek());
                        }
                        else
                        {
                            this.m_marshalledTaskQueue.Remove(item);
                        }
                    }

                    this.m_marshalledTaskKeys.Remove(task);
                }
            }
        }

        private void On_QueuedTask_Finished(TaskDefinition task)
        {
            task.Finished -= this.m_queuedTaskFinished;
            lock (this.m_activeQueueTasks)
            {
                if (this.m_activeQueueTasks.Contains(task))
                {
                    this.m_activeQueueTasks.Remove(task);
                }
            }
        }

        private void On_ThreadStrategyState_Changed()
        {
            this.SetThreadReleased();
        }

        private void On_WorkerThread_Failed(WorkerThread thread, string taskName, Exception exception_0)
        {
            this.FireAsynchronousEventFailed(thread, taskName, exception_0);
        }

        private void On_WorkerThread_Finished(WorkerThread thread)
        {
            this.RequeueThread(thread);
        }

        private void ProcessConsumerQueue<T>(object[] oParams)
        {
            Queue<T> ts = oParams[0] as Queue<T>;
            ThreadedOperationDelegate<T> threadedOperationDelegate = oParams[1] as ThreadedOperationDelegate<T>;
            object[] objArray = oParams[2] as object[];
            EventWaitHandle eventWaitHandle = oParams[3] as EventWaitHandle;
            EventWaitHandle eventWaitHandle1 = oParams[4] as EventWaitHandle;
            this.ProcessConsumerQueue<T>(ts, threadedOperationDelegate, objArray, eventWaitHandle, eventWaitHandle1);
        }

        private void ProcessConsumerQueue<T>(Queue<T> queue, ThreadedOperationDelegate<T> consumer,
            object[] consumerParams, EventWaitHandle elementProcessedHandle, EventWaitHandle elementQueuedHandle)
        {
            bool flag = true;
            while (flag)
            {
                T t = default(T);
                while (queue.Count == 0)
                {
                    elementQueuedHandle.Reset();
                    elementQueuedHandle.WaitOne();
                }

                lock (queue)
                {
                    t = queue.Dequeue();
                }

                if (t.Equals(default(T)))
                {
                    flag = false;
                }
                else
                {
                    try
                    {
                        consumer(queue.Dequeue(), consumerParams);
                    }
                    catch (Exception exception)
                    {
                        this.FireAsynchronousEventFailed(null, consumer.Method.Name, exception);
                    }

                    elementProcessedHandle.Set();
                }
            }

            elementProcessedHandle.Close();
        }

        private void ProcessMethodInfo(object[] oParams)
        {
            if ((int)oParams.Length == 2)
            {
                MethodInfo methodInfo = oParams[0] as MethodInfo;
                object[] objArray = oParams[1] as object[];
                if (methodInfo != null)
                {
                    methodInfo.Invoke(this, objArray);
                }
            }
        }

        private void ProcessQueue()
        {
            lock (this.m_queuedTasks)
            {
                Thread currentThread = Thread.CurrentThread;
                ThreadPriority priority = currentThread.Priority;
                while (this.m_queuedTasks.Count > 0)
                {
                    TaskDefinition taskDefinition = this.m_queuedTasks.Dequeue();
                    if (taskDefinition.RequestedPriority.HasValue)
                    {
                        currentThread.Priority = taskDefinition.RequestedPriority.Value;
                    }

                    try
                    {
                        taskDefinition.Execute();
                    }
                    catch (Exception exception)
                    {
                        this.FireAsynchronousEventFailed(null, taskDefinition.Name, exception);
                    }

                    currentThread.Priority = priority;
                }
            }
        }

        private void ProcessQueueAsync()
        {
            bool count = true;
            bool flag = false;
            while (true)
            {
                if (!this.m_bProcessQueue)
                {
                    if (!count && !flag)
                    {
                        break;
                    }
                }

                lock (this.m_queuedTasks)
                {
                    if (this.m_queuedTasks.Count <= 0)
                    {
                        this.m_taskQueuedWaitHandle.Reset();
                    }
                    else
                    {
                        WorkerThread availableThread = this.GetAvailableThread(false);
                        if (availableThread != null)
                        {
                            TaskDefinition taskDefinition = this.m_queuedTasks.Dequeue();
                            lock (this.m_activeQueueTasks)
                            {
                                this.m_activeQueueTasks.Add(taskDefinition);
                            }

                            if (taskDefinition.RequestedPriority.HasValue)
                            {
                                availableThread.Priority = taskDefinition.RequestedPriority.Value;
                            }

                            taskDefinition.Finished += this.m_queuedTaskFinished;
                            availableThread.Enqueue(taskDefinition);
                        }
                    }
                }

                this.m_taskQueuedWaitHandle.WaitOne(5000, false);
                Thread.Sleep(0);
                lock (this.m_queuedTasks)
                {
                    count = this.m_queuedTasks.Count > 0;
                }

                lock (this.m_activeQueueTasks)
                {
                    flag = this.m_activeQueueTasks.Count > 0;
                }
            }
        }

        public TaskDefinition QueueBufferedTask(string sScopeKey, object[] oParams, ThreadedOperationDelegate operation)
        {
            return this.QueueBufferedTask(sScopeKey, null, oParams, operation);
        }

        public TaskDefinition QueueBufferedTask(string sScopeKey, ThreadPriority? priority, object[] oParams,
            ThreadedOperationDelegate operation)
        {
            TaskDefinition taskDefinition = new TaskDefinition(operation, oParams)
            {
                RequestedPriority = priority
            };
            this.QueueBufferedTask(sScopeKey, taskDefinition);
            return taskDefinition;
        }

        public TaskDefinition QueueBufferedTask(string sScopeKey, object[] oParams, MethodInfo operation)
        {
            return this.QueueBufferedTask(sScopeKey, null, oParams, operation);
        }

        public TaskDefinition QueueBufferedTask(string sScopeKey, ThreadPriority? priority, object[] oParams,
            MethodInfo operation)
        {
            object[] objArray = new object[] { operation, oParams };
            return this.QueueBufferedTask(sScopeKey, priority, objArray,
                new ThreadedOperationDelegate(this.ProcessMethodInfo));
        }

        public void QueueBufferedTask(string sScopeKey, TaskDefinition task)
        {
            this.QueueBufferedTask(new string[] { sScopeKey }, task);
        }

        public void QueueBufferedTask(IList<string> scopeKeys, TaskDefinition task)
        {
            if (scopeKeys == null || scopeKeys.Count == 0)
            {
                this.QueueTask(task);
                return;
            }

            string item = scopeKeys[0];
            for (int i = 1; i < scopeKeys.Count; i++)
            {
                string str = scopeKeys[i];
                task = new TaskDefinition((object[] @params) => this.QueueBufferedTask(str, task), null);
            }

            lock (this.m_bufferedTasks)
            {
                lock (this.m_openedGates)
                {
                    if (!this.m_openedGates.Contains(item))
                    {
                        foreach (string mOpenedRegexGate in this.m_openedRegexGates)
                        {
                            if (!Regex.IsMatch(item, mOpenedRegexGate))
                            {
                                continue;
                            }

                            this.QueueTask(task);
                            return;
                        }

                        Queue<TaskDefinition> taskDefinitions = null;
                        if (!this.m_bufferedTasks.ContainsKey(item))
                        {
                            taskDefinitions = new Queue<TaskDefinition>();
                            this.m_bufferedTasks.Add(item, taskDefinitions);
                        }
                        else
                        {
                            taskDefinitions = this.m_bufferedTasks[item];
                        }

                        taskDefinitions.Enqueue(task);
                    }
                    else
                    {
                        this.QueueTask(task);
                        return;
                    }
                }
            }
        }

        public TaskDefinition QueueMarshalledTask(string sMarshallingKey, object[] oParams,
            ThreadedOperationDelegate operation)
        {
            return this.QueueMarshalledTask(sMarshallingKey, null, oParams, operation);
        }

        public TaskDefinition QueueMarshalledTask(string sMarshallingKey, ThreadPriority? priority, object[] oParams,
            ThreadedOperationDelegate operation)
        {
            TaskDefinition taskDefinition = new TaskDefinition(operation, oParams)
            {
                RequestedPriority = priority
            };
            this.QueueMarshalledTask(sMarshallingKey, taskDefinition);
            return taskDefinition;
        }

        public TaskDefinition QueueMarshalledTask(string sMarshallingKey, object[] oParams, MethodInfo operation)
        {
            return this.QueueMarshalledTask(sMarshallingKey, null, oParams, operation);
        }

        public TaskDefinition QueueMarshalledTask(string sMarshallingKey, ThreadPriority? priority, object[] oParams,
            MethodInfo operation)
        {
            return this.QueueMarshalledTask(sMarshallingKey, priority, oParams,
                new ThreadedOperationDelegate(this.ProcessMethodInfo));
        }

        public void QueueMarshalledTask(string sMarshallingKey, TaskDefinition task)
        {
            if (!this.ThreadingEnabled)
            {
                task.Execute();
            }

            lock (this.m_oLockMarshallingQueue)
            {
                task.Finished += this.m_marshalledTaskFinished;
                Queue<TaskDefinition> taskDefinitions = null;
                if (!this.m_marshalledTaskQueue.ContainsKey(sMarshallingKey))
                {
                    taskDefinitions = new Queue<TaskDefinition>();
                    this.m_marshalledTaskQueue.Add(sMarshallingKey, taskDefinitions);
                }
                else
                {
                    taskDefinitions = this.m_marshalledTaskQueue[sMarshallingKey];
                }

                taskDefinitions.Enqueue(task);
                this.m_marshalledTaskKeys.Add(task, sMarshallingKey);
                if (taskDefinitions.Count == 1)
                {
                    this.QueueTask(task);
                }
            }
        }

        public TaskDefinition QueueTask(object[] oParams, MethodInfo operation)
        {
            return this.QueueTask(null, oParams, operation);
        }

        public TaskDefinition QueueTask(ThreadPriority? priority, object[] oParams, MethodInfo operation)
        {
            object[] objArray = new object[] { operation, oParams };
            return this.QueueTask(priority, objArray, new ThreadedOperationDelegate(this.ProcessMethodInfo));
        }

        public TaskDefinition QueueTask(object[] oParams, ThreadedOperationDelegate operation)
        {
            return this.QueueTask(null, oParams, operation);
        }

        public TaskDefinition QueueTask(ThreadPriority? priority, object[] oParams, ThreadedOperationDelegate operation)
        {
            TaskDefinition taskDefinition = new TaskDefinition(operation, oParams)
            {
                RequestedPriority = priority
            };
            this.QueueTask(taskDefinition);
            return taskDefinition;
        }

        public void QueueTask(TaskDefinition task)
        {
            lock (this.m_queuedTasks)
            {
                this.m_queuedTasks.Enqueue(task);
            }

            this.RequestProcessQueue();
        }

        public TaskDefinition RequestAsynchronousOperation(object[] oParams, ThreadedOperationDelegate operation)
        {
            return this.RequestAsynchronousOperation(null, oParams, operation);
        }

        public TaskDefinition RequestAsynchronousOperation(ThreadPriority? priority, object[] oParams,
            ThreadedOperationDelegate operation)
        {
            TaskDefinition taskDefinition = new TaskDefinition(operation, oParams);
            if (priority.HasValue)
            {
                taskDefinition.RequestedPriority = priority;
            }

            this.RequestAsynchronousOperation(taskDefinition);
            return taskDefinition;
        }

        public TaskDefinition RequestAsynchronousOperation(object[] oParams, MethodInfo operation)
        {
            return this.RequestAsynchronousOperation(null, oParams, operation);
        }

        public TaskDefinition RequestAsynchronousOperation(ThreadPriority? priority, object[] oParams,
            MethodInfo operation)
        {
            object[] objArray = new object[] { operation, oParams };
            return this.RequestAsynchronousOperation(priority, objArray,
                new ThreadedOperationDelegate(this.ProcessMethodInfo));
        }

        public void RequestAsynchronousOperation(TaskDefinition task)
        {
            WorkerThread availableThread = this.GetAvailableThread();
            ThreadPriority priority = availableThread.Priority;
            if (task.RequestedPriority.HasValue)
            {
                availableThread.Priority = task.RequestedPriority.Value;
            }

            if (availableThread != null && !availableThread.UsesCurrentThread)
            {
                availableThread.Enqueue(task);
                return;
            }

            try
            {
                task.Execute();
            }
            catch (Exception exception)
            {
                this.FireAsynchronousEventFailed(null, task.Name, exception);
            }

            availableThread.Priority = priority;
        }

        private void RequestProcessQueue()
        {
            this.m_taskQueuedWaitHandle.Set();
        }

        private void RequeueThread(WorkerThread thread)
        {
            this.DettachEvents(thread);
            thread.Priority = ThreadPriority.BelowNormal;
            if (this.m_activeThreads != null)
            {
                lock (this.m_activeThreads)
                {
                    lock (this.m_inactiveThreads)
                    {
                        if (this.m_activeThreads.Contains(thread))
                        {
                            this.m_activeThreads.Remove(thread);
                        }

                        if (this.m_activeThreads.Count > 0 || this.m_iBlockedThreadCount.Value > 0)
                        {
                            this.m_threadsActiveWaitHandle.Set();
                        }

                        if (!this.m_inactiveThreads.Contains(thread))
                        {
                            this.m_inactiveThreads.Enqueue(thread);
                        }
                    }
                }

                this.SetThreadReleased();
            }
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer)
        {
            this.RunProducerConsumerQueue<T>(producer, null, consumer, null, null);
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer, object[] producerParams,
            ThreadedOperationDelegate<T> consumer, object[] consumerParams)
        {
            this.RunProducerConsumerQueue<T>(producer, producerParams, consumer, consumerParams, null);
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer, int iQueueCap)
        {
            QueuingDelegate<T> queuer = delegate(T item, ref int iValue)
            {
                bool result = iValue > iQueueCap;
                iValue++;
                return result;
            };
            this.RunProducerConsumerQueue<T>(producer, null, consumer, null, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer, object[] producerParams,
            ThreadedOperationDelegate<T> consumer, object[] consumerParams, int iQueueCap)
        {
            QueuingDelegate<T> queuer = delegate(T item, ref int iValue)
            {
                bool result = iValue > iQueueCap;
                iValue++;
                return result;
            };
            this.RunProducerConsumerQueue<T>(producer, producerParams, consumer, consumerParams, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer, QueuingDelegate<T> queuer)
        {
            this.RunProducerConsumerQueue<T>(producer, null, consumer, null, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadedOperationProducer<T> producer, object[] producerParams,
            ThreadedOperationDelegate<T> consumer, object[] consumerParams, QueuingDelegate<T> queuer)
        {
            this.RunProducerConsumerQueue<T>(null, producer, producerParams, consumer, consumerParams, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer)
        {
            this.RunProducerConsumerQueue<T>(priority, producer, null, consumer, null, null);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            object[] producerParams, ThreadedOperationDelegate<T> consumer, object[] consumerParams)
        {
            this.RunProducerConsumerQueue<T>(priority, producer, producerParams, consumer, consumerParams, null);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer, int iQueueCap)
        {
            QueuingDelegate<T> queuer = delegate(T item, ref int iValue)
            {
                bool result = iValue > iQueueCap;
                iValue++;
                return result;
            };
            this.RunProducerConsumerQueue<T>(priority, producer, null, consumer, null, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            object[] producerParams, ThreadedOperationDelegate<T> consumer, object[] consumerParams, int iQueueCap)
        {
            QueuingDelegate<T> queuer = delegate(T item, ref int iValue)
            {
                bool result = iValue > iQueueCap;
                iValue++;
                return result;
            };
            this.RunProducerConsumerQueue<T>(priority, producer, producerParams, consumer, consumerParams, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            ThreadedOperationDelegate<T> consumer, QueuingDelegate<T> queuer)
        {
            this.RunProducerConsumerQueue<T>(priority, producer, null, consumer, null, queuer);
        }

        public void RunProducerConsumerQueue<T>(ThreadPriority? priority, ThreadedOperationProducer<T> producer,
            object[] producerParams, ThreadedOperationDelegate<T> consumer, object[] consumerParams,
            QueuingDelegate<T> queuer)
        {
            EventWaitHandle manualResetEvent = new ManualResetEvent(false);
            EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
            int num = 0;
            ThreadPriority threadPriority = Thread.CurrentThread.Priority;
            if (priority.HasValue)
            {
                Thread.CurrentThread.Priority = priority.Value;
            }

            WorkerThread availableThread = null;
            Queue<T> ts = new Queue<T>();
            T t = default(T);
            T t1 = producer(producerParams);
            while (!t1.Equals(t))
            {
                ts.Enqueue(t1);
                manualResetEvent.Set();
                if (availableThread == null)
                {
                    availableThread = this.GetAvailableThread(false);
                }

                if (availableThread != null)
                {
                    if (priority.HasValue)
                    {
                        availableThread.Priority = priority.Value;
                    }

                    object[] objArray = new object[]
                        { ts, consumer, consumerParams, eventWaitHandle, manualResetEvent };
                    TaskDefinition taskDefinition =
                        new TaskDefinition(new ThreadedOperationDelegate(this.ProcessConsumerQueue<T>), objArray);
                    availableThread.Enqueue(taskDefinition);
                }
                else
                {
                    try
                    {
                        consumer(ts.Dequeue(), consumerParams);
                    }
                    catch (Exception exception)
                    {
                        this.FireAsynchronousEventFailed(null, consumer.Method.Name, exception);
                    }
                }

                if (queuer != null)
                {
                    if (!queuer(t1, ref num))
                    {
                        eventWaitHandle.Reset();
                        eventWaitHandle.WaitOne();
                        continue;
                    }
                }

                t1 = producer(producerParams);
            }

            if (availableThread != null)
            {
                ts.Enqueue(default(T));
                manualResetEvent.Set();
            }

            manualResetEvent.Close();
            Thread.CurrentThread.Priority = threadPriority;
        }

        private void RunQueueProcessor()
        {
            this.ProcessQueueAsync();
            List<TaskDefinition> taskDefinitions = null;
            lock (this.m_activeQueueTasks)
            {
                taskDefinitions = new List<TaskDefinition>(this.m_activeQueueTasks);
            }

            this.WaitForTasks(taskDefinitions);
        }

        public void SetBufferedTasks(string sScopeKey, bool bReset, bool bWaitForTasks)
        {
            this.SetBufferedTasks(new string[] { sScopeKey }, bReset, bWaitForTasks);
        }

        public void SetBufferedTasks(IEnumerable<string> scopeKeys, bool bReset, bool bWaitForTasks)
        {
            List<TaskDefinition> taskDefinitions = null;
            lock (this.m_bufferedTasks)
            {
                lock (this.m_openedGates)
                {
                    taskDefinitions = new List<TaskDefinition>();
                    foreach (string scopeKey in scopeKeys)
                    {
                        if (!bReset && !this.m_openedGates.Contains(scopeKey))
                        {
                            this.m_openedGates.Add(scopeKey);
                        }

                        if (!this.m_bufferedTasks.ContainsKey(scopeKey))
                        {
                            continue;
                        }

                        Queue<TaskDefinition> item = this.m_bufferedTasks[scopeKey];
                        this.m_bufferedTasks.Remove(scopeKey);
                        lock (this.m_queuedTasks)
                        {
                            if (bWaitForTasks)
                            {
                                taskDefinitions.AddRange(item);
                            }

                            while (item.Count > 0)
                            {
                                this.m_queuedTasks.Enqueue(item.Dequeue());
                            }
                        }
                    }
                }
            }

            this.RequestProcessQueue();
            if (taskDefinitions.Count > 0)
            {
                this.WaitForTasks(taskDefinitions);
            }
        }

        public void SetBufferedTasksWithWildCard(string regexScopeKey, bool reset, bool waitForTasks)
        {
            if (string.IsNullOrEmpty(regexScopeKey))
            {
                return;
            }

            List<string> strs = new List<string>(this.m_bufferedTasks.Count);
            lock (this.m_bufferedTasks)
            {
                lock (this.m_openedGates)
                {
                    this.m_openedRegexGates.Add(regexScopeKey);
                    foreach (string key in this.m_bufferedTasks.Keys)
                    {
                        if (!Regex.IsMatch(key, regexScopeKey))
                        {
                            continue;
                        }

                        strs.Add(key);
                    }
                }
            }

            this.SetBufferedTasks(strs, reset, waitForTasks);
        }

        private void SetThreadReleased()
        {
            this.m_threadReleasedWaitHandle.Set();
            this.m_taskQueuedWaitHandle.Set();
        }

        private void WaitAndSet(object[] oParams)
        {
            IEnumerable<TaskDefinition> taskDefinitions = oParams[0] as IEnumerable<TaskDefinition>;
            string str = (string)oParams[1];
            bool flag = (bool)oParams[2];
            this.WaitAndSet(taskDefinitions, str, flag, (bool)oParams[3]);
        }

        private void WaitAndSet(IEnumerable<TaskDefinition> delayingTasks, string sScopeKey, bool bReset,
            bool bWaitForTasks)
        {
            this.WaitForTasks(delayingTasks);
            this.SetBufferedTasks(sScopeKey, bReset, bWaitForTasks);
        }

        private WorkerThread WaitForAvailableThread()
        {
            WorkerThread i;
            for (i = this.GetAvailableThread(); i == null; i = this.GetAvailableThread())
            {
                this.m_threadReleasedWaitHandle.WaitOne();
            }

            return i;
        }

        public void WaitForTask(TaskDefinition task)
        {
            if (task != null)
            {
                this.WaitForTasks(new TaskDefinition[] { task });
            }
        }

        public void WaitForTasks(IEnumerable<TaskDefinition> tasks)
        {
            WorkerThread workerThread = null;
            if (Thread.CurrentThread != this.m_queueThread)
            {
                lock (this.m_activeThreads)
                {
                    List<WorkerThread>.Enumerator enumerator = this.m_activeThreads.GetEnumerator();
                    try
                    {
                        while (true)
                        {
                            if (enumerator.MoveNext())
                            {
                                WorkerThread current = enumerator.Current;
                                if (current.UsesCurrentThread)
                                {
                                    workerThread = current;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }

                    if (workerThread != null)
                    {
                        this.m_activeThreads.Remove(workerThread);
                        this.m_iBlockedThreadCount = new int?(this.m_iBlockedThreadCount.Value + 1);
                        WorkerThread.Count--;
                        this.SetThreadReleased();
                    }
                }
            }

            foreach (TaskDefinition task in tasks)
            {
                if (task == null)
                {
                    continue;
                }

                if (this.ThreadingEnabled)
                {
                    task.WaitOne();
                }
                else
                {
                    task.Execute();
                }
            }

            while (workerThread != null)
            {
                lock (this.m_activeThreads)
                {
                    if (this.m_threadStrategy.AllowThreadAllocation(this))
                    {
                        this.m_activeThreads.Add(workerThread);
                        this.m_iBlockedThreadCount = new int?(this.m_iBlockedThreadCount.Value - 1);
                        WorkerThread.Count++;
                        this.m_threadsActiveWaitHandle.Reset();
                        workerThread = null;
                    }
                }

                if (workerThread == null)
                {
                    continue;
                }

                this.m_threadReleasedWaitHandle.WaitOne(5000, false);
            }
        }

        public event TaskFailed AsynchronousTaskFailed;
    }
}