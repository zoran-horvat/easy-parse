using System.Collections.Generic;

namespace ParserCompiler.Collections
{
    public static class SetExtensions
    {
        public static Set<T> AsSet<T>(this IEnumerable<T> values) =>
            new Set<T>().Union(values);
    }
}