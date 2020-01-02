using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class IgnoreLexeme : Lexeme
    {
        public Regex Pattern { get; }

        public IgnoreLexeme(string pattern)
        {
            this.Pattern = new Regex(pattern);
        }

        public override string ToString() => 
            $"Ignore \"{this.Pattern}\"";
    }
}
