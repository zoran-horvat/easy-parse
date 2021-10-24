using System;
using System.Collections.Generic;

namespace EasyParse.Fluent.Rules
{
    static class CollectionExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> objects)
        {
            Queue<T> queue = new();

            foreach (T obj in objects)
                queue.Enqueue(obj);
            return queue;
        }

        public static void Add<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (T item in items)
                set.Add(item);
        }

        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (T item in items)
                queue.Enqueue(item);
        }

        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> sequence, Func<T> defaultFactory)
        {
            bool isEmpty = true;
            foreach (T obj in sequence)
            {
                yield return obj;
                isEmpty = false;
            }

            if (isEmpty) yield return defaultFactory();
        }
    }
}
