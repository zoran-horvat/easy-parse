using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public static class SetExtensions
    {
        public static Set<T> AsSet<T>(this IEnumerable<T> values) where T : class =>
            new Set<T>().Union(values);

        public static Set<Terminal> Find(this Set<FollowSet> followSets, NonTerminal key) =>
            followSets.First(set => set.Key.Equals(key)).Values;

        public static Set<Terminal> Find(this Set<FirstSet> firstSets, NonTerminal key) =>
            firstSets.First(set => set.Key.Equals(key)).Values;

        public static Set<T> Union<T>(this IEnumerable<Set<T>> sets) where T : class =>
            sets.Aggregate((union, next) => union.Union(next));
    }
}