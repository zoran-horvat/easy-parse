using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            int position = 0;

            while (matches.Any() && position < input.Length)
            {
                Token output = TokenAt(position, input, matches);
                if (!(output is Ignored)) yield return output;
                matches = this.Advance(matches, position + output.Length).ToList();
                position += output.Length;
            }

            if (position < input.Length)
                yield return new InvalidInput(input.LocationFor(position), input.Content.Substring(position));
            else
                yield return new EndOfInput(input.LocationFor(position));
        }

        private Token TokenAt(int position, Plaintext input, IEnumerable<Match> matches) =>
            matches
                .Where(match => match.Position == position)
                .Select(match => (position: match.Position, length: match.Length, tokenFactory: (Func<Token>) (() => match.Token)))
                .DefaultIfEmpty((position: position, length: input.Length - position, tokenFactory: () => this.Invalid(input, position)))
                .Aggregate((longest, cur) => cur.length > longest.length ? cur : longest)
                .tokenFactory();

        private Token Invalid(Plaintext input, int position) =>
            new InvalidInput(input.LocationFor(position), input.Content.Substring(position));

        private IEnumerable<Match> Advance(IEnumerable<Match> matches, int position) =>
            matches.SelectMany(match => 
                match.Position < position ? match.Next(position)
                : new [] {match});
    }
}
