using System;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    internal static class Locks
    {
        public static ReaderWriterLockSlim GetLockInstance(LockRecursionPolicy recursionPolicy)
        {
            return new ReaderWriterLockSlim(recursionPolicy);
        }

        public static void GetReadLock(ReaderWriterLockSlim locks)
        {
            bool flag = false;
            while (!flag)
            {
                flag = locks.TryEnterReadLock(1);
            }
        }

        public static void GetUpgradeableReadLock(ReaderWriterLockSlim locks)
        {
            bool flag = false;
            while (!flag)
            {
                flag = locks.TryEnterUpgradeableReadLock(1);
            }
        }

        public static void GetWriteLock(ReaderWriterLockSlim locks)
        {
            bool flag = false;
            while (!flag)
            {
                flag = locks.TryEnterWriteLock(1);
            }
        }

        public static void ReleaseReadLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsReadLockHeld)
            {
                locks.ExitReadLock();
            }
        }

        public static void ReleaseUpgradeableReadLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsUpgradeableReadLockHeld)
            {
                locks.ExitUpgradeableReadLock();
            }
        }

        public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsWriteLockHeld)
            {
                locks.ExitWriteLock();
            }
        }
    }
}