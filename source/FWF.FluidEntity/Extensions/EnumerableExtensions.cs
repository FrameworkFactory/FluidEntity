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
