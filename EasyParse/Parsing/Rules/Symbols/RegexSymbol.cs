using System;
using System.Text.RegularExpressions;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Parsing.Rules.Symbols
{
    public class RegexSymbol : TerminalSymbol
    {
        public RegexSymbol(string name, Regex expression, Type transformedType, Func<string, object> transform) : base(name)
        {
            this.Expression = expression;
            this.Type = transformedType;
            this.Transform = transform;
        }

        public static RegexSymbol Create<T>(string name, Regex expression, Func<string, T> transform) =>
            new RegexSymbol(name, expression, typeof(T), s => (object)transform(s));
            
        public Regex Expression { get; }
        public override Type Type {get; }
        public Func<string, object> Transform { get; }

        public Lexeme ToIgnoreLexemeModel() =>
            new IgnoreLexeme(this.Expression.ToString());

        public Lexeme ToLexemeModel() =>
            new LexemePattern(Name, this.Expression.ToString());

        public override string ToString() =>
            $"regex({this.Expression})";
    }
}
