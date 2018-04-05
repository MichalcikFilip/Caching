using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Michalcik.Caching
{
    public class Cache<K, T> : ICache<K, T>, IEnumerable<KeyValuePair<string, T>>, IEnumerable
    {
        public ObjectCache ObjectCache { get; }
        public CacheItemPolicy DefaultPolicy { get; set; }

        public Cache()
            : this(new MemoryCache("memory"), null)
        { }

        public Cache(ObjectCache objectCache)
            : this(objectCache, null)
        { }

        public Cache(CacheItemPolicy defaultPolicy)
            : this(new MemoryCache("memory"), defaultPolicy)
        { }

        public Cache(ObjectCache objectCache, CacheItemPolicy defaultPolicy)
        {
            ObjectCache = objectCache ?? throw new ArgumentNullException(nameof(objectCache));
            DefaultPolicy = defaultPolicy;
        }

        public void Add(K key, T item)
        {
            Add(key, item, DefaultPolicy);
        }

        public void Add(K key, T item, TimeSpan expiration)
        {
            Add(key, item, new CacheItemPolicy() { SlidingExpiration = expiration });
        }

        public void Add(K key, T item, CacheItemPolicy policy)
        {
            Add(TranslateKey(key), item, policy);
        }

        private void Add(string key, T item, CacheItemPolicy policy)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            ObjectCache.Set(key, item, policy);
        }

        public T Get(K key)
        {
            return Get(TranslateKey(key));
        }

        private T Get(string key)
        {
            return TranslateItem(ObjectCache[key]);
        }

        public T GetOrAdd(K key, Func<T> itemBuilder)
        {
            return GetOrAdd(key, itemBuilder, DefaultPolicy);
        }

        public T GetOrAdd(K key, Func<T> itemBuilder, TimeSpan expiration)
        {
            return GetOrAdd(key, itemBuilder, new CacheItemPolicy() { SlidingExpiration = expiration });
        }

        public T GetOrAdd(K key, Func<T> itemBuilder, CacheItemPolicy policy)
        {
            if (itemBuilder == null)
                throw new ArgumentNullException(nameof(itemBuilder));

            string sKey = TranslateKey(key);
            T item = Get(sKey);

            if (item != null)
                return item;

            item = itemBuilder();
            Add(sKey, item, policy);

            return item;
        }

        public void Remove(K key)
        {
            ObjectCache.Remove(TranslateKey(key));
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return ObjectCache.ToDictionary(x => x.Key, x => TranslateItem(x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)ObjectCache).GetEnumerator();
        }

        private string TranslateKey(K key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            string sKey = key.ToString();

            if (string.IsNullOrWhiteSpace(sKey))
                throw new ArgumentOutOfRangeException(nameof(key), "key.ToString() cannot be empty or whitespace");

            return sKey;
        }

        private T TranslateItem(object item)
        {
            if (item is T)
                return (T)item;

            return default(T);
        }
    }
}
