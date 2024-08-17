using System;
using System.Collections.Generic;
using System.Threading;

namespace Metalogix.Telemetry
{
    public sealed class Queue<T>
    {
        private readonly System.Collections.Generic.Queue<T> _queue = new System.Collections.Generic.Queue<T>();

        private volatile bool _stopped;

        public bool IsStopped
        {
            get { return this._stopped; }
        }

        public Queue()
        {
        }

        public T Dequeue()
        {
            T t;
            if (this._stopped)
            {
                return default(T);
            }

            lock (this._queue)
            {
                if (!this._stopped)
                {
                    while (this._queue.Count == 0)
                    {
                        Monitor.Wait(this._queue);
                        if (!this._stopped)
                        {
                            continue;
                        }

                        t = default(T);
                        return t;
                    }

                    t = this._queue.Dequeue();
                }
                else
                {
                    t = default(T);
                }
            }

            return t;
        }

        public bool Enqueue(T item)
        {
            bool flag;
            if (this._stopped)
            {
                return false;
            }

            lock (this._queue)
            {
                if (!this._stopped)
                {
                    this._queue.Enqueue(item);
                    Monitor.Pulse(this._queue);
                    return true;
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }

        public void Start()
        {
            if (!this._stopped)
            {
                return;
            }

            lock (this._queue)
            {
                if (this._stopped)
                {
                    this._stopped = false;
                }
            }
        }

        public void Stop()
        {
            if (this._stopped)
            {
                return;
            }

            lock (this._queue)
            {
                if (!this._stopped)
                {
                    this._stopped = true;
                    Monitor.PulseAll(this._queue);
                }
            }
        }
    }
}