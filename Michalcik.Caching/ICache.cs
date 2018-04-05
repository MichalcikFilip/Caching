using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Michalcik.Caching
{
    public interface ICache<K, T> : IEnumerable<KeyValuePair<string, T>>, IEnumerable
    {
        ObjectCache ObjectCache { get; }
        CacheItemPolicy DefaultPolicy { get; }

        void Add(K key, T item);
        void Add(K key, T item, TimeSpan expiration);
        void Add(K key, T item, CacheItemPolicy policy);
        T Get(K key);
        T GetOrAdd(K key, Func<T> itemBuilder);
        T GetOrAdd(K key, Func<T> itemBuilder, TimeSpan expiration);
        T GetOrAdd(K key, Func<T> itemBuilder, CacheItemPolicy policy);
        void Remove(K key);
    }
}
