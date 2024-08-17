using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Metalogix.Licensing
{
    internal sealed class GlobalLock : IDisposable
    {
        private readonly Mutex _mutex;

        private readonly bool _newMutexCreated;

        private bool _hasHandle;

        public GlobalLock(string instanceID) : this(instanceID, -1)
        {
        }

        public GlobalLock(string instanceID, int timeOut)
        {
            string str = string.Format("Global\\{0}", instanceID);
            MutexAccessRule mutexAccessRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Modify | MutexRights.Synchronize,
                AccessControlType.Allow);
            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(mutexAccessRule);
            this._mutex = new Mutex(false, str, out this._newMutexCreated, mutexSecurity);
            this.WaitOne(timeOut);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._hasHandle && this._mutex != null)
                {
                    this._mutex.ReleaseMutex();
                }

                this._mutex.Close();
            }
        }

        private void WaitOne(int timeOut)
        {
            try
            {
                if (timeOut > 0)
                {
                    this._hasHandle = this._mutex.WaitOne(timeOut, false);
                }
                else
                {
                    this._hasHandle = this._mutex.WaitOne(-1, false);
                }

                if (!this._hasHandle)
                {
                    throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
                }
            }
            catch (AbandonedMutexException abandonedMutexException)
            {
                this._hasHandle = true;
            }
        }
    }
}