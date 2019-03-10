using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable RECS0083

namespace PokeAPI
{
    public class Cache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict;
        private readonly Func<TKey, Task<Maybe<TValue>>> _get;

        public bool IsActive { get; set; }

        public Cache(Func<TKey, Task<Maybe<TValue>>> getNew, bool active = true,
            IDictionary<TKey, TValue> defValues = null)
        {
            _get = getNew;

            IsActive = active;

            var defValues2 = defValues ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>();

            _dict = new ConcurrentDictionary<TKey, TValue>(defValues2);
        }

        public async Task<TValue> GetAsync(TKey key)
        {
            if (_dict.TryGetValue(key, out var cacheItem))
                return cacheItem;

            var item = await _get(key);
            if (item.HasValue)
                return IsActive ? _dict.GetOrAdd(key, item.Value) : item.Value;

            throw new KeyNotFoundException();
        }

        public TValue Get(TKey key) => GetAsync(key).Result;

        public Maybe<TValue> TryGet(TKey key) => _dict.TryGetValue(key, out var v) ? new Maybe<TValue>(v) : Maybe<TValue>.Nothing;

        public TValue TryGetDef(TKey key, TValue def = default(TValue))
        {
            var mv = TryGet(key);

            return mv.HasValue ? mv.Value : def;
        }

        public int Count => _dict.Count;
        public bool IsReadOnly => true;
        public ICollection<TKey> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;

        public void Clear()
        {
            _dict.Clear();
        }

        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            ((IDictionary<TKey, TValue>) _dict).Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>) _dict).CopyTo(array, arrayIndex);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
    }

    public class Cache<T> : Cache<Unit, T>
    {
        public Cache(Func<Unit, Task<Maybe<T>>> getNew)
            : base(getNew)
        {
        }

        public Cache(Func<Task<Maybe<T>>> getNew)
            : base(_ => getNew())
        {
        }

        public Task<T> GetAsync() => GetAsync(0);
        public T Get() => Get(0);
        public Maybe<T> TryGet() => TryGet(0);
        public T TryGetDef() => TryGetDef(0);

        public T TryGetDef(T def) => base.TryGetDef(0, def);
    }
}