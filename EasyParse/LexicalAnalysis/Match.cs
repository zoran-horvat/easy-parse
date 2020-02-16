using System.Collections.Generic;
using System.Linq;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Text;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace EasyParse.LexicalAnalysis
{
    class Match
    {
        public string Value { get; }
        public Location Location { get; }
        public Location LocationAfter { get; }

        private Pattern Pattern { get; }
        private Plaintext Input { get; }

        public Match(Pattern pattern, RegexMatch match, Plaintext input, Location location, Location locationAfter)
        {
            this.Pattern = pattern;
            this.Value = match.Value;
            this.Input = input;
            this.Location = location;
            this.LocationAfter = locationAfter;
        }

        public static IEnumerable<Match> FirstMatch(Pattern pattern, Plaintext input) =>
            input.TryMatch(pattern.Expression, input.Beginning)
                .Select(tuple => new Match(pattern, tuple.match, input, tuple.at, tuple.locationAfter));

        public IEnumerable<Match> Next(Location at) =>
            this.Input.TryMatch(this.Pattern.Expression, at)
                .Select(tuple => new Match(this.Pattern, tuple.match, this.Input, tuple.at, tuple.locationAfter));

        public Token Token =>
            this.Pattern.CreateToken(this.Value, this.Location, this.LocationAfter);

        public override string ToString() =>
            $"Matching [{this.Pattern.Expression}] at {this.Location}";
    }
}
