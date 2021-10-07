﻿using System.Text.RegularExpressions;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Parsing.Rules
{
    public class RegexSymbol : TerminalSymbol
    {
        public RegexSymbol(string name, Regex expression) : base(name)
        {
            this.Expression = expression;
        }

        public Regex Expression { get; }

        public override string ToString() => 
            $"regex({this.Expression})";

        public override Lexeme ToLexemeModel() =>
            new LexemePattern(base.Name, this.Expression.ToString());

        public Lexeme ToIgnoreLexemeModel() =>
            new IgnoreLexeme(this.Expression.ToString());
    }
}