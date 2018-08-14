using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Built
{
    public static class ExtensionMethods
    {
        public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }

        public static bool IsNullable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            return list as IReadOnlyList<T> ?? new ReadOnlyWrapper<T>(list);
        }

        private sealed class ReadOnlyWrapper<T> : IReadOnlyList<T>
        {
            private readonly IList<T> _list;

            public ReadOnlyWrapper(IList<T> list) => _list = list;

            public int Count => _list.Count;

            public T this[int index] => _list[index];

            public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}