using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    static class QueueExtensions
    {
        public static void Enqueue<T, TElement>(this Queue<T> queue, IEnumerable<TElement> objects) where TElement : T
        {
            foreach (TElement obj in objects)
                queue.Enqueue(obj);
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> objects)
        {
            Queue<T> queue = new();

            foreach (T obj in objects)
                queue.Enqueue(obj);
            return queue;
        }
    }
}
