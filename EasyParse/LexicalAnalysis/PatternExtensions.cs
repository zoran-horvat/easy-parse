using System.Collections.Generic;
using System.Linq;

namespace EasyParse.LexicalAnalysis
{
    static class PatternExtensions
    {
        public static IEnumerable<Match> FirstMatches(this string input, IEnumerable<Pattern> patterns) =>
            patterns.SelectMany(pattern => Match.FirstMatch(pattern.Expression, pattern.CreateToken, input));
    }
}