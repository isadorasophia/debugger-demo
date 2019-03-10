using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;

#pragma warning disable RECS0083

namespace PokeAPI
{
    public struct CacheGetter<TKey, TValue> : IDictionary<TKey, TValue>
    {
        Cache<TKey, TValue> c;

        public ICollection<TKey  > Keys   => c.Keys  ;
        public ICollection<TValue> Values => c.Values;

        public int Count => c.Count;
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets or sets whether the cache is active or not.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return c.IsActive;
            }
            set
            {
                c.IsActive = value;
            }
        }

        public TValue this[TKey key] => c.TryGetDef(key);

        public CacheGetter(Cache<TKey, TValue> cache)
        {
            c = cache;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            c.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => c.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }
        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotImplementedException();
        }
        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
    }
}
