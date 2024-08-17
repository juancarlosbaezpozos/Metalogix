using System;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    internal abstract class BaseLock : IDisposable
    {
        protected ReaderWriterLockSlim _Locks;

        protected BaseLock(ReaderWriterLockSlim locks)
        {
            this._Locks = locks;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected abstract void Dispose(bool value);
    }
}