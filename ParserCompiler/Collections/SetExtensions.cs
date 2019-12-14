using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public static class SetExtensions
    {
        public static Set<T> AsSet<T>(this IEnumerable<T> values) where T : class =>
            new Set<T>().Union(values);

        public static Set<Terminal> Find(this Set<FollowSet> followSet, NonTerminal key) =>
            followSet.First(set => set.Key.Equals(key)).Values;
    }
}