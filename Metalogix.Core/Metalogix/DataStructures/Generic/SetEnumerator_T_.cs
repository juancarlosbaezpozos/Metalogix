using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.DataStructures.Generic
{
    public class SetEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private IEnumerator m_enumerator;

        T System.Collections.Generic.IEnumerator<T>.Current
        {
            get { return (T)((IEnumerator)this).Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.m_enumerator.Current; }
        }

        public SetEnumerator(IEnumerator underlyingEnumerator)
        {
            this.m_enumerator = underlyingEnumerator;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public bool MoveNext()
        {
            return this.m_enumerator.MoveNext();
        }

        public void Reset()
        {
            this.m_enumerator.Reset();
        }
    }
}