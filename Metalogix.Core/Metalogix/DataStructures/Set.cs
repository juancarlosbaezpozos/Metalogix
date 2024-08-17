using System;
using System.Collections;
using System.Text;

namespace Metalogix.DataStructures
{
    public class Set : ICollection, IEnumerable
    {
        private Hashtable m_hashTableSet = new Hashtable();

        public int Count
        {
            get { return this.m_hashTableSet.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.m_hashTableSet.IsReadOnly; }
        }

        public bool IsSynchronized
        {
            get { return this.m_hashTableSet.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return this.m_hashTableSet.SyncRoot; }
        }

        public Set()
        {
        }

        public Set(Set setToCopy)
        {
            foreach (object obj in setToCopy)
            {
                this.Add(obj);
            }
        }

        public Set(Array objects)
        {
            foreach (object @object in objects)
            {
                this.Add(@object);
            }
        }

        public Set(ArrayList objects)
        {
            foreach (object @object in objects)
            {
                this.Add(@object);
            }
        }

        public bool Add(object item)
        {
            if (this.Contains(item))
            {
                return false;
            }

            this.m_hashTableSet.Add(item, null);
            return true;
        }

        public void Add(Array items)
        {
            foreach (object item in items)
            {
                if (this.Contains(item))
                {
                    continue;
                }

                this.m_hashTableSet.Add(item, null);
            }
        }

        public void Add(ArrayList items)
        {
            this.Add(items.ToArray());
        }

        public void Clear()
        {
            this.m_hashTableSet.Clear();
        }

        public bool Contains(object item)
        {
            return this.m_hashTableSet.ContainsKey(item);
        }

        public bool Contains(Array items)
        {
            return this.IsSupersetOf(new Set(items));
        }

        public bool Contains(ArrayList items)
        {
            return this.Contains(items.ToArray());
        }

        public void CopyTo(Array array, int index)
        {
            this.m_hashTableSet.Keys.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_hashTableSet.Keys.GetEnumerator();
        }

        public static Set Intersection(Set s1, Set s2)
        {
            Set sets = new Set();
            foreach (object obj in s1)
            {
                if (!s2.Contains(obj))
                {
                    continue;
                }

                sets.Add(obj);
            }

            return sets;
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }

        public bool IsEqual(Set otherSet)
        {
            if (!this.IsSubsetOf(otherSet))
            {
                return false;
            }

            return this.IsSupersetOf(otherSet);
        }

        public bool IsProperSubsetOf(Set possibleSuperset)
        {
            if (!this.IsSubsetOf(possibleSuperset))
            {
                return false;
            }

            return possibleSuperset.Count > this.Count;
        }

        public bool IsProperSupersetOf(Set possibleSubset)
        {
            return possibleSubset.IsProperSubsetOf(this);
        }

        public bool IsSubsetOf(Set possibleSuperset)
        {
            bool flag;
            IEnumerator enumerator = this.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (possibleSuperset.Contains(enumerator.Current))
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }

                return true;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return flag;
        }

        public bool IsSupersetOf(Set possibleSubset)
        {
            return possibleSubset.IsSubsetOf(this);
        }

        public static Set operator +(Set s1, Set s2)
        {
            return Set.Union(s1, s2);
        }

        public static Set operator -(Set s1, Set s2)
        {
            Set sets = new Set(s1);
            foreach (object obj in s2)
            {
                s1.Remove(obj);
            }

            return sets;
        }

        public bool Remove(object item)
        {
            if (!this.Contains(item))
            {
                return false;
            }

            this.m_hashTableSet.Remove(item);
            return true;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            ICollection keys = this.m_hashTableSet.Keys;
            bool flag = true;
            foreach (string key in keys)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(key);
            }

            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        public static Set Union(Set s1, Set s2)
        {
            Set sets = new Set(s1);
            foreach (object obj in s2)
            {
                sets.Add(obj);
            }

            return sets;
        }
    }
}