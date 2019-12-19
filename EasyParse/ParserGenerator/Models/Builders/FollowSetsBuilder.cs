using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Builders
{
    static class FollowSetsBuilder
    {
        public static Set<FollowSet> BuildFor(IEnumerable<Rule> rules, Set<FirstSet> firstSets) =>
            PurgeNonTerminals(BuildFor(rules.ToList(), firstSets));

        private static Set<FollowSet> PurgeNonTerminals(Set<IntermediateFollowSet> followSets) =>
            followSets.Select(followSet => followSet.PurgeNonTerminals()).AsSet();

        private static Set<IntermediateFollowSet> BuildFor(List<Rule> rules, Set<FirstSet> firstSets) =>
            Closure(rules, InitialFollowSets(rules), firstSets);

        private static Set<IntermediateFollowSet> InitialFollowSets(List<Rule> rules) =>
            rules
                .Select(rule => rule.Head)
                .Select(nonTerminal => new IntermediateFollowSet(nonTerminal, ImmediateFollowers(rules, nonTerminal)))
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

        private static Set<IntermediateFollowSet> Closure(List<Rule> rules, Set<IntermediateFollowSet> followSets, Set<FirstSet> firstSets)
        {
            Set<IntermediateFollowSet> result = followSets;
            while (Advance(rules, result, firstSets) is Set<IntermediateFollowSet> next && !(next.Equals(result)))
            {
                result = next;
            }
            return result;
        }

        private static Set<IntermediateFollowSet> Advance(List<Rule> rules, Set<IntermediateFollowSet> followSets, Set<FirstSet> firstSets) =>
            followSets.Select(set => Advance(set, rules, followSets, firstSets)).AsSet();

        private static IntermediateFollowSet Advance(IntermediateFollowSet set, List<Rule> rules, Set<IntermediateFollowSet> followSets, Set<FirstSet> firstSets) =>
            set.Union(SymbolsToAdd(set, firstSets)).Union(SymbolsToAdd(set, rules, followSets));

        private static IntermediateFollowSet SymbolsToAdd(IntermediateFollowSet set, Set<FirstSet> firstSets) =>
            new IntermediateFollowSet(set.Label,
                firstSets
                    .Where(firstSet => set.OfType<NonTerminal>().Contains(firstSet.Label))
                    .SelectMany(firstSet => firstSet));

        private static IntermediateFollowSet SymbolsToAdd(IntermediateFollowSet set, List<Rule> rules, Set<IntermediateFollowSet> followSets) =>
            new IntermediateFollowSet(set.Label,
                rules
                    .Where(rule => rule.Body.LastOrDefault() is NonTerminal last && last.Equals(set.Label))
                    .Select(rule => rule.Head)
                    .Distinct()
                    .SelectMany(head => followSets.First(followSet => followSet.Label.Equals(head))));
    }
}
