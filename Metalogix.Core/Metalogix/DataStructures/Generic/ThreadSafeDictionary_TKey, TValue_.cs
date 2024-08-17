using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Metalogix.DataStructures.Generic
{
    [Serializable]
    public class ThreadSafeDictionary<TKey, TValue> : IThreadSafeDictionary<TKey, TValue>, IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private IDictionary<TKey, TValue> _dict;
        [NonSerialized] private ReaderWriterLockSlim dictionaryLock;

        public ThreadSafeDictionary()
        {
            this.dictionaryLock = Locks.GetLockInstance(LockRecursionPolicy.NoRecursion);
            this._dict = new Dictionary<TKey, TValue>();
        }

        public ThreadSafeDictionary(int capacity)
        {
            this.dictionaryLock = Locks.GetLockInstance(LockRecursionPolicy.NoRecursion);
            this._dict = new Dictionary<TKey, TValue>(capacity);
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            using (new WriteLock(this.dictionaryLock))
            {
                this._dict.Add(item);
            }
        }

        public virtual void Add(TKey key, TValue value)
        {
            using (new WriteLock(this.dictionaryLock))
            {
                this._dict.Add(key, value);
            }
        }

        public virtual void Clear()
        {
            using (new WriteLock(this.dictionaryLock))
            {
                this._dict.Clear();
            }
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            using (new ReadLock(this.dictionaryLock))
            {
                return this._dict.Contains(item);
            }
        }

        public virtual bool ContainsKey(TKey key)
        {
            using (new ReadLock(this.dictionaryLock))
            {
                return this._dict.ContainsKey(key);
            }
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            using (new ReadLock(this.dictionaryLock))
            {
                this._dict.CopyTo(array, arrayIndex);
            }
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            using (new ReadLock(this.dictionaryLock))
            {
                return new List<KeyValuePair<TKey, TValue>>(this._dict).GetEnumerator();
            }
        }

        public void AddSafe(TKey key, TValue newValue)
        {
            using (UpgradeableReadLock upgradeableReadLock = new UpgradeableReadLock(this.dictionaryLock))
            {
                if (!this._dict.ContainsKey(key))
                {
                    using (WriteLock writeLock = new WriteLock(this.dictionaryLock))
                    {
                        this._dict.Add(key, newValue);
                    }
                }
            }
        }

        public void MergeSafe(TKey key, TValue newValue)
        {
            using (new WriteLock(this.dictionaryLock))
            {
                if (this._dict.ContainsKey(key))
                {
                    this._dict.Remove(key);
                }

                this._dict.Add(key, newValue);
            }
        }

        public virtual bool Remove(TKey key)
        {
            using (new WriteLock(this.dictionaryLock))
            {
                return this._dict.Remove(key);
            }
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            using (new WriteLock(this.dictionaryLock))
            {
                return this._dict.Remove(item);
            }
        }

        public void RemoveSafe(TKey key)
        {
            using (new UpgradeableReadLock(this.dictionaryLock))
            {
                if (this._dict.ContainsKey(key))
                {
                    using (new WriteLock(this.dictionaryLock))
                    {
                        this._dict.Remove(key);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException(
                "Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            using (new ReadLock(this.dictionaryLock))
            {
                return this._dict.TryGetValue(key, out value);
            }
        }

        public virtual int Count
        {
            get
            {
                using (new ReadLock(this.dictionaryLock))
                {
                    return this._dict.Count;
                }
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                using (new ReadLock(this.dictionaryLock))
                {
                    return this._dict.IsReadOnly;
                }
            }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                using (new ReadLock(this.dictionaryLock))
                {
                    return this._dict[key];
                }
            }
            set
            {
                using (new WriteLock(this.dictionaryLock))
                {
                    this._dict[key] = value;
                }
            }
        }

        public virtual ICollection<TKey> Keys
        {
            get
            {
                using (new ReadLock(this.dictionaryLock))
                {
                    return new List<TKey>(this._dict.Keys);
                }
            }
        }

        public virtual ICollection<TValue> Values
        {
            get
            {
                using (new ReadLock(this.dictionaryLock))
                {
                    return new List<TValue>(this._dict.Values);
                }
            }
        }
    }
}