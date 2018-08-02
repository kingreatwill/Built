using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Built
{
    public static class ExtensionMethods
    {
        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
    }
}