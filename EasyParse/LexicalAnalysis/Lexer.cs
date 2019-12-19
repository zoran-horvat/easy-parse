using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public IEnumerable<Lexeme> Tokenize(string input) => Enumerable.Empty<Lexeme>();
    }
}
