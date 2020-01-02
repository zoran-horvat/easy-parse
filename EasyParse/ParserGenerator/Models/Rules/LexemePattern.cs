using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class LexemePattern : Lexeme
    {
        public string Name { get; }
        public Regex Pattern { get; }

        public LexemePattern(string name, string pattern)
        {
            this.Name = name;
            this.Pattern = new Regex(pattern);
        }

        public override string ToString() =>
            $"{this.Name} matching \"{this.Pattern}\"";
    }
}