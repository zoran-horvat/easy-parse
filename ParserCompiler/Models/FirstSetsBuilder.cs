using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    static class FirstSetsBuilder
    {
        public static Set<FirstSet> BuildFor(IEnumerable<Rule> rules) =>
            PurgeNonTerminals(Closure(InitialFirstSets(rules)));

        private static Set<FirstSet> PurgeNonTerminals(Set<FirstSet> firstSets) =>
            firstSets.Select(set => set.PurgeNonTerminals()).AsSet();

        private static Set<FirstSet> InitialFirstSets(IEnumerable<Rule> rules) =>
            rules
                .Select(rule => (head: rule.Head, body: rule.Body.Take(1)))
                .SelectMany(tuple => tuple.body.Select(terminal => (head: tuple.head, terminal: terminal)))
                .GroupBy(tuple => tuple.head, tuple => tuple.terminal)
                .Select(group => new FirstSet(group.Key, group))
                .AsSet();

        private static Set<FirstSet> Closure(Set<FirstSet> firstSets)
        {
            Set<FirstSet> result = firstSets;
            while (Advance(result) is Set<FirstSet> next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        private static Set<FirstSet> Advance(Set<FirstSet> firstSets) =>
            firstSets.Select(set => Advance(set, firstSets)).AsSet();

        private static FirstSet Advance(FirstSet set, Set<FirstSet> firstSets) =>
            set.OfType<NonTerminal>()
                .Select(nonTerminal => firstSets.First(expansion => expansion.Key.Equals(nonTerminal)))
                .Aggregate(set, (acc, extension) => acc.Union(extension));    }
}
