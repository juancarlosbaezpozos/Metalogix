using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public class ChunkedFile : IDisposable
    {
        private System.IO.Stream _stream;

        private bool _isFileStream;

        public string FileName { get; private set; }

        public bool IsOutOfRetentionTime
        {
            get
            {
                DateTime lastAccess = this.LastAccess;
                DateTime now = DateTime.Now;
                return lastAccess < now.AddMinutes((double)(-this.RetentionTimeMinutes));
            }
        }

        public DateTime LastAccess { get; set; }

        public bool RemoveAfterTransfer { get; set; }

        public int RetentionTimeMinutes { get; set; }

        public System.IO.Stream Stream
        {
            get { return this._stream; }
            set
            {
                this._stream = value;
                FileStream fileStream = value as FileStream;
                this._isFileStream = fileStream != null;
                this.FileName = (fileStream != null ? fileStream.Name : "MemoryStream");
            }
        }

        public ChunkedFile()
        {
        }

        public void CloseStream()
        {
            try
            {
                this._stream.Close();
                this._stream.Dispose();
                if (this._isFileStream && !string.IsNullOrEmpty(this.FileName))
                {
                    File.Delete(this.FileName);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Format(
                    "ChunkedFile >> Dispose: Failed to delete temp file \"{0}\", error:\r\n{1}", this.FileName,
                    exception));
                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CloseStream();
            }
        }

        ~ChunkedFile()
        {
            this.Dispose(false);
        }

        public override string ToString()
        {
            object[] fileName = new object[]
            {
                this.FileName, this.RetentionTimeMinutes, this.LastAccess, this.RemoveAfterTransfer,
                this.IsOutOfRetentionTime
            };
            return string.Format("Name={0}, RetTime={1}, LastAccess={2},RemAfterClose={3},IsOutOfRetTime={4}",
                fileName);
        }
    }
}