using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public static class GrammarExtensions
    {
        public static IEnumerable<Symbol> AllSymbols(this IEnumerable<RuleDefinition> rules) =>
            rules.SelectMany(rule => rule.Body.Concat(new[] {rule.Head})).Distinct();

        public static IDictionary<NonTerminal, int> SortOrder(this IEnumerable<RuleDefinition> rules) =>
            rules.Select((rule, index) => (rule, index))
                .GroupBy(tuple => tuple.rule.Head, tuple => tuple.index)
                .ToDictionary(group => group.Key, group => group.Min());
    }
}