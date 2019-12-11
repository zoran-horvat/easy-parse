using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    class FirstSetsBuilder
    {
        public static Set<NonTerminalToSymbols> From(IEnumerable<Rule> rules) =>
            PurgeNonTerminals(Closure(InitialFirstSets(rules)));

        private static Set<NonTerminalToSymbols> PurgeNonTerminals(Set<NonTerminalToSymbols> firstSets) =>
            firstSets.Select(set => set.PurgeNonTerminals()).AsSet();

        private static Set<NonTerminalToSymbols> InitialFirstSets(IEnumerable<Rule> rules) =>
            rules
                .Select(rule => (head: rule.Head, body: rule.Body.Take(1)))
                .SelectMany(tuple => tuple.body.Select(terminal => (head: tuple.head, terminal: terminal)))
                .GroupBy(tuple => tuple.head, tuple => tuple.terminal)
                .Select(group => new NonTerminalToSymbols(group.Key, group))
                .AsSet();

        private static Set<NonTerminalToSymbols> Closure(Set<NonTerminalToSymbols> firstSets)
        {
            Set<NonTerminalToSymbols> result = firstSets;
            while (Advance(result) is Set<NonTerminalToSymbols> next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        private static Set<NonTerminalToSymbols> Advance(Set<NonTerminalToSymbols> firstSets) =>
            firstSets.Select(set => Advance(set, firstSets)).AsSet();

        private static NonTerminalToSymbols Advance(NonTerminalToSymbols set, Set<NonTerminalToSymbols> firstSets) =>
            set.OfType<NonTerminal>()
                .Select(nonTerminal => firstSets.First(expansion => expansion.Key.Equals(nonTerminal)))
                .Aggregate(set, (acc, extension) => acc.Union(extension));    }
}
