using System;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    internal class ReadLock : BaseLock
    {
        public ReadLock(ReaderWriterLockSlim locks) : base(locks)
        {
            Locks.GetReadLock(this._Locks);
        }

        protected override void Dispose(bool value)
        {
            Locks.ReleaseReadLock(this._Locks);
            GC.SuppressFinalize(this);
        }
    }
}