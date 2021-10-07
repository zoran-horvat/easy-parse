using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    static class QueueExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> objects)
        {
            Queue<T> queue = new();

            foreach (T obj in objects)
                queue.Enqueue(obj);
            return queue;
        }
    }
}
