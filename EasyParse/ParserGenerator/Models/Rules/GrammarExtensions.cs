using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public static class GrammarExtensions
    {
        public static Grammar ToGrammar(this IEnumerable<string> rawRules) =>
            new GrammarParser().Parse(rawRules);

        public static IEnumerable<Symbol> AllSymbols(this IEnumerable<Rule> rules) =>
            rules.SelectMany(rule => rule.Body.Concat(new[] {rule.Head})).Distinct();

        public static IDictionary<NonTerminal, int> SortOrder(this IEnumerable<Rule> rules) =>
            rules.Select((rule, index) => (rule, index))
                .GroupBy(tuple => tuple.rule.Head, tuple => tuple.index)
                .ToDictionary(group => group.Key, group => group.Min());
    }
}