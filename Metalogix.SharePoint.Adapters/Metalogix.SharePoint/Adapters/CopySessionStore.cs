using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Metalogix.SharePoint.Adapters
{
    internal sealed class CopySessionStore : IDisposable
    {
        private const int _CleanCheckEveryMinute = 1;

        private readonly Dictionary<Guid, ChunkedFile> _sessions = new Dictionary<Guid, ChunkedFile>();

        private Timer _cleanUpThread;

        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        public ChunkedFile this[Guid sessionId]
        {
            get
            {
                ChunkedFile now;
                this._lock.AcquireReaderLock(-1);
                try
                {
                    if (!this._sessions.TryGetValue(sessionId, out now))
                    {
                        throw new ArgumentException("The specified session either does not exist or it was closed.");
                    }

                    now.LastAccess = DateTime.Now;
                }
                finally
                {
                    this._lock.ReleaseReaderLock();
                }

                return now;
            }
        }

        public CopySessionStore()
        {
        }

        public Guid Add(ChunkedFile file)
        {
            Guid guid = Guid.NewGuid();
            file.LastAccess = DateTime.Now;
            this._lock.AcquireWriterLock(-1);
            try
            {
                this._sessions.Add(guid, file);
                if (this._cleanUpThread == null)
                {
                    this._cleanUpThread = new Timer(new TimerCallback(this.DoCleanUp), null, new TimeSpan(0, 1, 0),
                        new TimeSpan(0, 1, 0));
                    Trace.WriteLine("CopySessionStore >> Add: cleanup thread started.");
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }

            return guid;
        }

        public ChunkedFile Close(Guid sessionId)
        {
            return this.Close(sessionId, false);
        }

        public ChunkedFile Close(Guid sessionId, bool forceRemove)
        {
            ChunkedFile item = this[sessionId];
            this._lock.AcquireWriterLock(-1);
            try
            {
                try
                {
                    item.Stream.Flush();
                    if (item.RemoveAfterTransfer || forceRemove)
                    {
                        this._sessions.Remove(sessionId);
                        CopySessionStore.DeletePhysicalFile(item);
                    }

                    if (this._sessions.Count == 0 && this._cleanUpThread != null)
                    {
                        this._cleanUpThread.Dispose();
                        this._cleanUpThread = null;
                        Trace.WriteLine("CopySessionStore >> Cleanup: thread closed, nothing to check.");
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    Trace.WriteLine(string.Format(
                        "CopySessionStore >> Failed to close to the file \"{0}\", error:\r\n{1}", item.FileName,
                        exception));
                    throw;
                }
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }

            return item;
        }

        private static void DeletePhysicalFile(ChunkedFile file)
        {
            try
            {
                file.CloseStream();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Format(
                    "CopySessionStore >> DeletePhysicalFile: Failed to delete temp file \"{0}\", error:\r\n{1}",
                    file.FileName, exception));
                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && this._cleanUpThread != null)
            {
                this._cleanUpThread.Dispose();
            }
        }

        private void DoCleanUp(object state)
        {
            Trace.WriteLine("CopySessionStore >> Starting cleanup");
            this._lock.AcquireReaderLock(-1);
            try
            {
                List<Guid> guids = new List<Guid>();
                foreach (KeyValuePair<Guid, ChunkedFile> _session in this._sessions)
                {
                    if (!_session.Value.IsOutOfRetentionTime)
                    {
                        continue;
                    }

                    guids.Add(_session.Key);
                }

                LockCookie writerLock = this._lock.UpgradeToWriterLock(-1);
                try
                {
                    foreach (Guid guid in guids)
                    {
                        ChunkedFile item = this[guid];
                        this._sessions.Remove(guid);
                        CopySessionStore.DeletePhysicalFile(item);
                        Trace.WriteLine(string.Format("CopySessionStore >> Cleanup: remove file '{0}', Id={1}",
                            item.FileName, guid));
                    }
                }
                finally
                {
                    this._lock.DowngradeFromWriterLock(ref writerLock);
                }

                if (this._sessions.Count == 0 && this._cleanUpThread != null)
                {
                    this._cleanUpThread.Dispose();
                    this._cleanUpThread = null;
                    Trace.WriteLine("CopySessionStore >> Cleanup: thread closed, nothing to check.");
                }
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
        }

        ~CopySessionStore()
        {
            this.Dispose(false);
        }

        public Stream GetStream(Guid sessionId)
        {
            return this[sessionId].Stream;
        }
    }
}