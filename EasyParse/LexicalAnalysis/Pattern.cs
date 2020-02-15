using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.LexicalAnalysis.Tokens;

namespace EasyParse.LexicalAnalysis
{
    class Pattern
    {
        public Regex Expression { get; }
        private IEnumerable<string> LexemeLabel { get; }

        public Pattern(string expression) : this(expression, new string[0])
        {
        }

        public Pattern(string expression, string lexemeLabel) : this(expression, new[] {lexemeLabel})
        {
        }

        private Pattern(string expression, string[] lexemeLabel)
        {
            this.Expression = new Regex(expression);
            this.LexemeLabel = lexemeLabel;
        }

        public Token CreateToken(string value, Location location) =>
            this.LexemeLabel.Select<string, Token>(label => new Lexeme(label, location, value))
                .DefaultIfEmpty(new Ignored(value, location))
                .First();

        public override string ToString() =>
            this.LexemeLabel
                .Select(label => $"{label} = \"{this.Expression}\"")
                .DefaultIfEmpty($"Ignore \"{this.Expression}\"")
                .First();
    }
}
