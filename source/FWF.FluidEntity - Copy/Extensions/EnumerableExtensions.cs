using System;
using System.Collections.Generic;

namespace FWF.FluidEntity
{
    public static class EnumerableExtensions
    {

        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return null;
            }

            using (var e = collection.GetEnumerator())
            {
                while (e.MoveNext())
                {
                }
            }

            return collection;
        }

        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T obj in items)
            {
                action(obj);
            }
        }

        public static IEnumerable<T> Consume<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return null;
            }

            return new HashSet<T>(collection).Materialize();
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            using (var e = itemsToAdd.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    collection.Add(e.Current);
                }
            }
        }

        public static void SafeAddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> itemsToAdd)
        {
            using (var e = itemsToAdd.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (!dictionary.ContainsKey(e.Current.Key))
                    {
                        dictionary.Add(e.Current);
                    }
                }
            }
        }

        public static void SafeAddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> itemsToAdd)
        {
            using (var e = itemsToAdd.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (!dictionary.ContainsKey(e.Current.Key))
                    {
                        dictionary.Add(e.Current);
                    }
                }
            }
        }


        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            List<T> nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }

    }
}
