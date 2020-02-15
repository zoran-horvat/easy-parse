using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Text;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace EasyParse.LexicalAnalysis
{
    class Match
    {
        public int Position => this.RegexMatch.Index;
        public int Length => this.RegexMatch.Length;

        private Regex Pattern { get; }
        private RegexMatch RegexMatch { get; }
        private Plaintext Input { get; }
        private Func<string, Location, Token> TokenFactory { get; }

        private Match(Regex pattern, Func<string, Location, Token> tokenFactory, RegexMatch match, Plaintext input)
        {
            this.Pattern = pattern;
            this.TokenFactory = tokenFactory;
            this.RegexMatch = match;
            this.Input = input;
        }

        public static IEnumerable<Match> FirstMatch(Regex pattern, Func<string, Location, Token> tokenFactory, Plaintext input) =>
            Next(pattern, tokenFactory, input, 0);

        public IEnumerable<Match> Next(int position) =>
            Next(this.Pattern, this.TokenFactory, this.Input, position);

        public Token Token =>
            this.TokenFactory(this.RegexMatch.Value, this.Input.LocationFor(this.Position));

        private static IEnumerable<Match> Next(Regex pattern, Func<string, Location, Token> tokenFactory, Plaintext input, int position) =>
            pattern.Match(input.Content, position) is RegexMatch regexMatch && regexMatch.Success 
                ? new []{new Match(pattern, tokenFactory, regexMatch, input)}
                : Enumerable.Empty<Match>();
    }
}
