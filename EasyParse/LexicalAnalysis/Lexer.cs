using System.Collections.Generic;
using System.Linq;

namespace EasyParse.LexicalAnalysis
{
    public class Lexer
    {
        public Lexer AddPattern(string regex, string lexeme) => this;
        public Lexer IgnorePattern(string regex) => this;

        public IEnumerable<Lexeme> Tokenize(string input) => Enumerable.Empty<Lexeme>();
    }
}
