using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class IgnoreLexeme
    {
        public Regex Pattern { get; }

        public IgnoreLexeme(string pattern)
        {
            this.Pattern = new Regex(pattern);
        }
    }
}
