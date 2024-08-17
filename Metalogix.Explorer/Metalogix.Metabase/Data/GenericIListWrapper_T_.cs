using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Metabase.Data
{
    public class GenericIListWrapper<T> : IList, ICollection, IEnumerable
    {
        private IList<T> m_baseList;

        public int Count
        {
            get { return this.m_baseList.Count; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return this.m_baseList.IsReadOnly; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object this[int index]
        {
            get { return this.m_baseList[index]; }
            set { this.m_baseList[index] = (T)value; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public GenericIListWrapper(IList<T> baseList)
        {
            this.m_baseList = baseList;
        }

        public int Add(object value)
        {
            this.m_baseList.Add((T)value);
            return this.m_baseList.IndexOf((T)value);
        }

        public void Clear()
        {
            this.m_baseList.Clear();
        }

        public bool Contains(object value)
        {
            return this.m_baseList.Contains((T)value);
        }

        public void CopyTo(Array array, int index)
        {
            T[] tArray = new T[array.Length];
            this.m_baseList.CopyTo(tArray, index);
            for (int i = 0; i < (int)tArray.Length; i++)
            {
                array.SetValue(tArray[i], i);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new GenericIListWrapper<T>.WrapperEnumerator<T>(this);
        }

        public int IndexOf(object value)
        {
            return this.m_baseList.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            this.m_baseList.Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            this.m_baseList.Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            this.m_baseList.RemoveAt(index);
        }

        private class WrapperEnumerator<U> : IEnumerator
        {
            private U[] m_items;

            private int m_index;

            public object Current
            {
                get { return this.m_items[this.m_index]; }
            }

            public WrapperEnumerator(GenericIListWrapper<U> wrapper)
            {
                this.m_items = new U[wrapper.Count];
                for (int i = 0; i < wrapper.Count; i++)
                {
                    this.m_items[i] = (U)wrapper[i];
                }
            }

            public bool MoveNext()
            {
                GenericIListWrapper<T>.WrapperEnumerator<U> wrapperEnumerator = this;
                int mIndex = wrapperEnumerator.m_index + 1;
                int num = mIndex;
                wrapperEnumerator.m_index = mIndex;
                return num < (int)this.m_items.Length;
            }

            public void Reset()
            {
                this.m_index = -1;
            }
        }
    }
}