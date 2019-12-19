using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Builders
{
    static class FirstSetsBuilder
    {
        public static Set<FirstSet> BuildFor(IEnumerable<Rule> rules) =>
            PurgeNonTerminals(Closure(InitialFirstSets(rules)));

        private static Set<FirstSet> PurgeNonTerminals(Set<IntermediateFirstSet> firstSets) =>
            firstSets.Select(set => set.PurgeNonTerminals()).AsSet();

        private static Set<IntermediateFirstSet> InitialFirstSets(IEnumerable<Rule> rules) =>
            rules
                .Select(rule => (head: rule.Head, body: rule.Body.Take(1)))
                .SelectMany(tuple => tuple.body.Select(terminal => (head: tuple.head, terminal: terminal)))
                .GroupBy(tuple => tuple.head, tuple => tuple.terminal)
                .Select(group => new IntermediateFirstSet(group.Key, group))
                .AsSet();

        private static Set<IntermediateFirstSet> Closure(Set<IntermediateFirstSet> firstSets)
        {
            Set<IntermediateFirstSet> result = firstSets;
            while (Advance(result) is Set<IntermediateFirstSet> next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        private static Set<IntermediateFirstSet> Advance(Set<IntermediateFirstSet> firstSets) =>
            firstSets.Select(set => Advance(set, firstSets)).AsSet();

        private static IntermediateFirstSet Advance(IntermediateFirstSet set, Set<IntermediateFirstSet> firstSets) =>
            set.OfType<NonTerminal>()
                .Select(nonTerminal => firstSets.First(expansion => expansion.Label.Equals(nonTerminal)))
                .Aggregate(set, (acc, extension) => acc.Union(extension));    }
}
