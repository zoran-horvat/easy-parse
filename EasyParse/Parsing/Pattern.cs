using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public static class Pattern
    {
        public static RegexSymbol Int =>
            RegexSymbol.Create<int>("number", new Regex(@"\d+"), int.Parse);

        public static RegexSymbol WhiteSpace =>
            RegexSymbol.Create<string>("whitespace", new Regex(@"\s+"), x => x);

        public static RegexSymbol EndOfLine =>
            RegexSymbol.Create<string>("eoln", new Regex(@"(\r\n|\r|\n)"), x => x);
            
    }
}
