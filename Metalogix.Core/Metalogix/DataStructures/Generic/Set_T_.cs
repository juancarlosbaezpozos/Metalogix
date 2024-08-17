using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.DataStructures.Generic
{
    public class Set<T> : SerializableList<T>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return true; }
        }

        public override T this[T key]
        {
            get { return default(T); }
        }

        public Set()
        {
        }

        public Set(Set<T> copySet) : base(copySet)
        {
        }

        public Set(T[] items) : base(items)
        {
        }

        public Set(List<T> items)
        {
            if (items != null)
            {
                base.AddRangeToCollection(items.ToArray());
            }
        }

        public static Set<T> Intersection(Set<T> s1, Set<T> s2)
        {
            Set<T> set = new Set<T>();
            foreach (T t in s1)
            {
                if (!s2.Contains(t))
                {
                    continue;
                }

                set.AddToCollection(t);
            }

            return set;
        }

        public bool IntersectsWith(Set<T> possiblyIntersectingSet)
        {
            bool flag;
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!possiblyIntersectingSet.Contains(enumerator.Current))
                    {
                        continue;
                    }

                    flag = true;
                    return flag;
                }

                return false;
            }

            return flag;
        }

        public bool IsEqual(Set<T> otherSet)
        {
            if (!this.IsSubsetOf(otherSet))
            {
                return false;
            }

            return this.IsSupersetOf(otherSet);
        }

        public bool IsProperSubsetOf(Set<T> possibleSuperset)
        {
            if (!this.IsSubsetOf(possibleSuperset))
            {
                return false;
            }

            return possibleSuperset.Count > base.Count;
        }

        public bool IsProperSupersetOf(Set<T> possibleSubset)
        {
            return possibleSubset.IsProperSubsetOf(this);
        }

        public bool IsSubsetOf(Set<T> possibleSuperset)
        {
            bool flag;
            using (IEnumerator<T> enumerator = base.GetEnumerator())
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

            return flag;
        }

        public bool IsSupersetOf(Set<T> possibleSubset)
        {
            return possibleSubset.IsSubsetOf(this);
        }

        public static Set<T> operator +(Set<T> s1, Set<T> s2)
        {
            return Set<T>.Union(s1, s2);
        }

        public static Set<T> operator -(Set<T> s1, Set<T> s2)
        {
            Set<T> set = new Set<T>(s1);
            foreach (T t in s2)
            {
                set.RemoveFromCollection(t);
            }

            return set;
        }

        public static Set<T> Union(Set<T> s1, Set<T> s2)
        {
            Set<T> set = new Set<T>(s1);
            foreach (T t in s2)
            {
                if (set.Contains(t))
                {
                    continue;
                }

                set.AddToCollection(t);
            }

            return set;
        }
    }
}