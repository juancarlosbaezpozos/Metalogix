using System;
using System.Threading;

namespace Metalogix.Parallelization
{
    public class RWLock : IDisposable
    {
        private const int AcquireTimeout = 60000;

        private Guid _instanceID = Guid.NewGuid();

        private LockCookie _lockCookie;

        private readonly ReaderWriterLock _lock;

        private RWLock.RWLockType _type;

        private bool _isUpgraded;

        public RWLock()
        {
            this._lock = new ReaderWriterLock();
            this._type = RWLock.RWLockType.None;
        }

        private RWLock(RWLock parent, RWLock.RWLockType type)
        {
            this._lock = parent._lock;
            this._type = type;
            switch (type)
            {
                case RWLock.RWLockType.Read:
                {
                    this._lock.AcquireReaderLock(60000);
                    return;
                }
                case RWLock.RWLockType.Write:
                {
                    this._lock.AcquireWriterLock(60000);
                    return;
                }
                default:
                {
                    throw new ArgumentException("type");
                }
            }
        }

        public void DowngradeToReaderLock()
        {
            if (this._type == RWLock.RWLockType.Read)
            {
                throw new Exception("Lock type is already reader lock.");
            }

            if (!this._isUpgraded)
            {
                throw new Exception("Lock was not upgraded to writer lock.");
            }

            this._lock.DowngradeFromWriterLock(ref this._lockCookie);
            this._type = RWLock.RWLockType.Read;
            this._isUpgraded = false;
        }

        public RWLock GetReaderLock()
        {
            return new RWLock(this, RWLock.RWLockType.Read);
        }

        public RWLock GetWriterLock()
        {
            return new RWLock(this, RWLock.RWLockType.Write);
        }

        public void Release()
        {
            if (this._isUpgraded)
            {
                this.DowngradeToReaderLock();
            }

            switch (this._type)
            {
                case RWLock.RWLockType.Read:
                {
                    this._lock.ReleaseReaderLock();
                    break;
                }
                case RWLock.RWLockType.Write:
                {
                    this._lock.ReleaseWriterLock();
                    break;
                }
            }

            this._type = RWLock.RWLockType.None;
        }

        void System.IDisposable.Dispose()
        {
            this.Release();
        }

        public void UpgrateToWriterLock()
        {
            if (this._type == RWLock.RWLockType.Write)
            {
                throw new Exception("Lock type is already writer lock.");
            }

            this._lockCookie = this._lock.UpgradeToWriterLock(60000);
            this._type = RWLock.RWLockType.Write;
            this._isUpgraded = true;
        }

        private enum RWLockType
        {
            None,
            Read,
            Write
        }
    }
}