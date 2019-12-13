using System.Collections.Generic;

namespace ParserCompiler.Models.Rules
{
    public static class GrammarExtensions
    {
        public static Grammar ToGrammar(this IEnumerable<string> rawRules) =>
            new GrammarParser().Parse(rawRules);
    }
}