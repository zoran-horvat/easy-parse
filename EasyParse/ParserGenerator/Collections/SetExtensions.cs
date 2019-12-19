using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Collections
{
    public static class SetExtensions
    {
        public static Set<T> AsSet<T>(this IEnumerable<T> values) where T : class =>
            new Set<T>().Union(values);

        public static Set<Terminal> Find(this Set<FollowSet> followSets, NonTerminal key) =>
            followSets.First(set => set.Label.Equals(key)).Values;

        public static Set<Terminal> Find(this Set<FirstSet> firstSets, NonTerminal key) =>
            firstSets.First(set => set.Label.Equals(key)).Values;

        public static Set<T> Union<T>(this IEnumerable<Set<T>> sets) where T : class =>
            sets.Aggregate((union, next) => union.Union(next));
    }
}