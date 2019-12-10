using System.Collections.Generic;

namespace ParserCompiler
{
    public static class GrammarExtensions
    {
        public static IEnumerable<Rule> ToRules(this IEnumerable<string> rawRules) =>
            new GrammarParser().GetRules(rawRules);
    }
}