using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Metabase
{
    public abstract class MetabaseObject : IDisposable
    {
        private readonly MetabaseObject m_parent;

        [Browsable(false)] public MetabaseConnection Connection { get; private set; }

        internal DataRow Data { get; private set; }

        public MetabaseObject(MetabaseObject parent, DataRow data) : this(parent.Connection, parent, data)
        {
        }

        internal MetabaseObject(MetabaseConnection connection, MetabaseObject parent, DataRow data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("dataRow");
            }

            this.m_parent = parent;
            this.Connection = connection;
            this.Data = data;
        }

        public void Dispose()
        {
            this.Dispose(false);
            if (this.OnDisposed != null)
            {
                this.OnDisposed(this, null);
                this.OnDisposed = null;
            }
        }

        public abstract void Dispose(bool forceGC);

        public bool Exists()
        {
            if (this.Data == null)
            {
                return true;
            }

            return this.Data.RowState != DataRowState.Detached;
        }

        public T GetParent<T>()
            where T : MetabaseObject
        {
            return (T)this.m_parent;
        }

        public event EventHandler OnDisposed;
    }
}