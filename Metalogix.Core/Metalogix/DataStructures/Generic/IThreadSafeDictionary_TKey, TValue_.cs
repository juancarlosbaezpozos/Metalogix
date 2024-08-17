using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.DataStructures.Generic
{
    public interface IThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        void AddSafe(TKey key, TValue newValue);

        void MergeSafe(TKey key, TValue newValue);

        void RemoveSafe(TKey key);
    }
}