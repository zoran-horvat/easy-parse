using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Symbols;

namespace ParserCompiler
{
    public class Grammar
    {
        private IEnumerable<Rule> Rules { get; }
     
        public Grammar(IEnumerable<Rule> rules)
        {
            this.Rules = rules.ToList();
        }

        public Set<FirstSet> FirstSets =>
            this.PurgeNonTerminals(this.Closure(this.InitialFirstSets));

        private Set<FirstSet> PurgeNonTerminals(Set<FirstSet> firstSets) =>
            firstSets.Select(set => set.PurgeNonTerminals()).AsSet();

        private Set<FirstSet> InitialFirstSets =>
            this.Rules
                .Select(rule => (head: rule.Head, body: rule.Body.Take(1)))
                .SelectMany(tuple => tuple.body.Select(terminal => (head: tuple.head, terminal: terminal)))
                .GroupBy(tuple => tuple.head, tuple => tuple.terminal)
                .Select(group => new FirstSet(group.Key, group))
                .AsSet();

        private Set<FirstSet> Closure(Set<FirstSet> firstSets)
        {
            Set<FirstSet> result = firstSets;
            while (this.Advance(result) is Set<FirstSet> next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        private Set<FirstSet> Advance(Set<FirstSet> firstSets) =>
            firstSets.Select(set => this.Advance(set, firstSets)).AsSet();

        private FirstSet Advance(FirstSet set, Set<FirstSet> firstSets) =>
            set.OfType<NonTerminal>()
                .Select(nonTerminal => firstSets.First(expansion => expansion.Key.Equals(nonTerminal)))
                .Aggregate(set, (acc, extension) => acc.Union(extension));

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Rules.Select(rule => rule.ToString()));
    }
}