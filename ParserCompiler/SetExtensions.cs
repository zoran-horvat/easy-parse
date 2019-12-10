using System.Collections.Generic;

namespace ParserCompiler
{
    public static class SetExtensions
    {
        public static Set<T> AsSet<T>(this IEnumerable<T> values) =>
            new Set<T>(values);
    }
}