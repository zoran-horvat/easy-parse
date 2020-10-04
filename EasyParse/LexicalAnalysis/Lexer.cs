using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Text;

namespace EasyParse.LexicalAnalysis
{
    public class Lexer
    {
        private ImmutableList<Pattern> Patterns { get; }

        public Lexer() : this(ImmutableList<Pattern>.Empty)
        {
        }

        private Lexer(ImmutableList<Pattern> patterns)
        {
            this.Patterns = patterns;
        }

        public Lexer AddPattern(string regex, string lexeme) =>
            new Lexer(this.Patterns.Add(new Pattern(regex, lexeme)));

        public Lexer IgnorePattern(string regex) =>
            new Lexer(this.Patterns.Add(new Pattern(regex)));

        public IEnumerable<Token> Tokenize(Plaintext input)
        {
            List<Match> matches = input.FirstMatches(this.Patterns).ToList();
            Location location = input.Beginning;

            while (matches.Any() && location is InnerLocation inner)
            {
                Token output = TokenAt(location, input, matches);
                if (!(output is Ignored)) yield return output;
                matches = this.Advance(matches, output.LocationAfter).ToList();
                location = output.LocationAfter;
            }

            if (location is InnerLocation remaining)
                yield return new InvalidInput(remaining, EndOfText.Value, input.Substring(remaining));

            yield return new EndOfInput(location);
        }

        private Token TokenAt(Location location, Plaintext input, IEnumerable<Match> matches) =>
            matches
                .Where(match => match.Location.CompareTo(location) == 0)
                .Select(match => (nextLocation: match.LocationAfter, factory: (Func<Token>)(() => match.Token)))
                .DefaultIfEmpty((nextLocation: EndOfText.Value, factory: () => this.Invalid(input, location)))
                .Aggregate((longest, cur) => cur.nextLocation.CompareTo(longest.nextLocation) > 0 ? cur : longest)
                .factory();

        private Token Invalid(Plaintext input, Location location) =>
            new InvalidInput(location, EndOfText.Value, 
                location is InnerLocation inner ? input.Content.Substring(inner.Offset) : string.Empty);

        private IEnumerable<Match> Advance(IEnumerable<Match> matches, Location to) =>
            matches.SelectMany(match => 
                match.Location.CompareTo(to) < 0 ? match.Next(to)
                : new [] {match});
    }
}
