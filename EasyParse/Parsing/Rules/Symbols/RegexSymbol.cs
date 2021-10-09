using System;
using System.Text.RegularExpressions;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Parsing.Rules.Symbols
{
    public class RegexSymbol : TerminalSymbol
    {
        public RegexSymbol(string name, Regex expression) : base(name)
        {
            this.Expression = expression;
        }

        public Regex Expression { get; }
        public override Type Type => typeof(string);

        public Lexeme ToIgnoreLexemeModel() =>
            new IgnoreLexeme(this.Expression.ToString());

        public Lexeme ToLexemeModel() =>
            new LexemePattern(Name, this.Expression.ToString());

        public override string ToString() =>
            $"regex({this.Expression})";
    }
}
