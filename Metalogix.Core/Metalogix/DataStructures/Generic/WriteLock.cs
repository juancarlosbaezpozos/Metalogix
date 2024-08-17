using System;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    internal class WriteLock : BaseLock
    {
        public WriteLock(ReaderWriterLockSlim locks) : base(locks)
        {
            Locks.GetWriteLock(this._Locks);
        }

        protected override void Dispose(bool value)
        {
            Locks.ReleaseWriteLock(this._Locks);
            GC.SuppressFinalize(this);
        }
    }
}