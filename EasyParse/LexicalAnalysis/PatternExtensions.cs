using System.Collections.Generic;
using System.Linq;
using EasyParse.Text;

namespace EasyParse.LexicalAnalysis
{
    static class PatternExtensions
    {
        public static IEnumerable<Match> FirstMatches(this Plaintext input, IEnumerable<Pattern> patterns) =>
            patterns.SelectMany(pattern => Match.FirstMatch(pattern.Expression, pattern.CreateToken, input));
    }
}