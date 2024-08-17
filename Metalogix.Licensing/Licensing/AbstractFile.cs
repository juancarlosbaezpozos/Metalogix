using Metalogix.Licensing.Storage;
using System;

namespace Metalogix.Licensing
{
    public abstract class AbstractFile : IDisposable
    {
        private readonly IDataStorage _storage;

        public bool Exists
        {
            get { return this.StorageHandler.Exists; }
        }

        public IDataStorage StorageHandler
        {
            get { return this._storage; }
        }

        protected AbstractFile(IDataStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }

            this._storage = storage;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this._storage != null)
            {
                this._storage.Dispose();
            }
        }

        ~AbstractFile()
        {
            this.Dispose(false);
        }

        public abstract void Load();

        public abstract void Save();
    }
}