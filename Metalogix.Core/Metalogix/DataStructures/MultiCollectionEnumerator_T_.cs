using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.DataStructures
{
    public class MultiCollectionEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private IEnumerator[] m_enumerators;

        private int m_iEnumeratorIndex;

        public object Current
        {
            get { return this.m_enumerators[this.m_iEnumeratorIndex].Current; }
        }

        T System.Collections.Generic.IEnumerator<T>.Current
        {
            get { return (T)((IEnumerator)this).Current; }
        }

        public MultiCollectionEnumerator(IEnumerator[] enumerators)
        {
            this.m_enumerators = enumerators;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IEnumerator[] mEnumerators = this.m_enumerators;
            for (int i = 0; i < (int)mEnumerators.Length; i++)
            {
                IDisposable disposable = mEnumerators[i] as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public bool MoveNext()
        {
            bool flag = this.m_enumerators[this.m_iEnumeratorIndex].MoveNext();
            if (!flag)
            {
                this.m_iEnumeratorIndex++;
                if ((int)this.m_enumerators.Length > this.m_iEnumeratorIndex)
                {
                    this.m_enumerators[this.m_iEnumeratorIndex].Reset();
                    flag = this.m_enumerators[this.m_iEnumeratorIndex].MoveNext();
                }
            }

            return flag;
        }

        public void Reset()
        {
            IEnumerator[] mEnumerators = this.m_enumerators;
            for (int i = 0; i < (int)mEnumerators.Length; i++)
            {
                mEnumerators[i].Reset();
            }

            this.m_iEnumeratorIndex = 0;
        }
    }
}