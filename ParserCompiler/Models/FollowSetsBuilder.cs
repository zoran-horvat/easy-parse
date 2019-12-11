using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    static class FollowSetsBuilder
    {
        public static Set<FollowSet> BuildFor(IEnumerable<Rule> rules, Set<FirstSet> firstSets) =>
            PurgeNonTerminals(BuildFor(rules.ToList(), firstSets));

        private static Set<FollowSet> PurgeNonTerminals(Set<FollowSet> followSets) =>
            followSets.Select(followSet => followSet.PurgeNonTerminals()).AsSet();

        private static Set<FollowSet> BuildFor(List<Rule> rules, Set<FirstSet> firstSets) =>
            Closure(rules, InitialFollowSets(rules), firstSets);

        private static Set<FollowSet> InitialFollowSets(List<Rule> rules) =>
            rules
                .Select(rule => rule.Head)
                .Select(nonTerminal => new FollowSet(nonTerminal, ImmediateFollowers(rules, nonTerminal)))
                .AsSet();

        private static IEnumerable<Symbol> ImmediateFollowers(IEnumerable<Rule> rules, NonTerminal of) =>
            ImmediateFollowers(rules)
                .Where(tuple => tuple.preceding.Equals(of))
                .Select(tuple => tuple.following)
                .Concat(EndOfInputFollow(of));

        private static IEnumerable<Symbol> EndOfInputFollow(NonTerminal of) =>
            of.Equals(new NonTerminal("S'")) ? new[] {new EndOfInput(),} : Enumerable.Empty<Symbol>();

        private static IEnumerable<(NonTerminal preceding, Symbol following)> ImmediateFollowers(IEnumerable<Rule> rules) =>
            rules.SelectMany(ImmediateFollowers);

        private static IEnumerable<(NonTerminal preceding, Symbol following)> ImmediateFollowers(Rule rule) =>
            rule.Body
                .Zip(rule.Body.Skip(1), (preceding, following) => (preceding, following))
                .Where(tuple => tuple.preceding is NonTerminal)
                .Select(tuple => ((NonTerminal) tuple.preceding, tuple.following));

        private static Set<FollowSet> Closure(List<Rule> rules, Set<FollowSet> followSets, Set<FirstSet> firstSets)
        {
            Set<FollowSet> result = followSets;
            while (Advance(rules, result, firstSets) is Set<FollowSet> next && !(next.Equals(result)))
            {
                result = next;
            }
            return result;
        }

        private static Set<FollowSet> Advance(List<Rule> rules, Set<FollowSet> followSets, Set<FirstSet> firstSets) =>
            followSets.Select(set => Advance(set, rules, followSets, firstSets)).AsSet();

        private static FollowSet Advance(FollowSet set, List<Rule> rules, Set<FollowSet> followSets, Set<FirstSet> firstSets) =>
            set.Union(SymbolsToAdd(set, firstSets)).Union(SymbolsToAdd(set, rules, followSets));

        private static FollowSet SymbolsToAdd(FollowSet set, Set<FirstSet> firstSets) =>
            new FollowSet(set.Key,
                firstSets
                    .Where(firstSet => set.OfType<NonTerminal>().Contains(firstSet.Key))
                    .SelectMany(firstSet => firstSet));

        private static FollowSet SymbolsToAdd(FollowSet set, List<Rule> rules, Set<FollowSet> followSets) =>
            new FollowSet(set.Key,
                rules
                    .Where(rule => rule.Body.LastOrDefault() is NonTerminal last && last.Equals(set.Key))
                    .Select(rule => rule.Head)
                    .Distinct()
                    .SelectMany(head => followSets.First(followSet => followSet.Key.Equals(head))));
    }
}
