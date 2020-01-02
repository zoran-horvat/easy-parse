using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public abstract class Lexeme
    {
        public Regex Pattern { get; }

        protected Lexeme(string pattern)
        {
            this.Pattern = new Regex(pattern);
        }
    }
}