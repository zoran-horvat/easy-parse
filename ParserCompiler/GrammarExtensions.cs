using System.Collections.Generic;

namespace ParserCompiler
{
    public static class GrammarExtensions
    {
        public static Grammar ToGrammar(this IEnumerable<string> rawRules) =>
            new GrammarParser().Parse(rawRules);
    }
}