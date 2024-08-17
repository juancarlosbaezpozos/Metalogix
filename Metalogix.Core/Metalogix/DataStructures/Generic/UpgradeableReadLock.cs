using System;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    internal class UpgradeableReadLock : BaseLock
    {
        public UpgradeableReadLock(ReaderWriterLockSlim locks) : base(locks)
        {
            Locks.GetUpgradeableReadLock(this._Locks);
        }

        protected override void Dispose(bool value)
        {
            Locks.ReleaseUpgradeableReadLock(this._Locks);
            GC.SuppressFinalize(this);
        }
    }
}