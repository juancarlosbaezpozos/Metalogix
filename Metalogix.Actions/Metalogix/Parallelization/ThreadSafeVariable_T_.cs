using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Parallelization
{
    public class ThreadSafeVariable<T>
    {
        private readonly RWLock _lock;

        public T Value { get; set; }

        public ThreadSafeVariable(T value)
        {
            this.Value = value;
            this._lock = new RWLock();
        }

        public RWLock StartRead()
        {
            return this._lock.GetReaderLock();
        }

        public RWLock StartWrite()
        {
            return this._lock.GetWriterLock();
        }
    }
}