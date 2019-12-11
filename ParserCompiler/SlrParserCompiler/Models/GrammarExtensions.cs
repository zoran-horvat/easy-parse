using System.Collections.Generic;

namespace ParserCompiler.SlrParserCompiler.Models
{
    public static class GrammarExtensions
    {
        public static Grammar ToGrammar(this IEnumerable<string> rawRules) =>
            new GrammarParser().Parse(rawRules);
    }
}