using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        private Symbol Unit => Symbol(() => Rule()
            .Match(RegexSymbol.Create<int>("number", new Regex(@"\d+"), int.Parse)).ToIdentity<int>()
            .Match("-", Unit).To((string _, int value) => -value)
            .Match("(", Additive, ")").To((string _, int additive, string _) => additive));

        private Symbol Multiplicative => Symbol(() => Rule()
            .Match(Unit).ToIdentity<int>()
            .Match(Multiply).ToIdentity<int>()
            .Match(Divide).ToIdentity<int>());

        private Symbol Multiply => Symbol(() => Rule()
            .Match(Multiplicative, "*", Unit).To((int a, string _, int b) => a * b));

        private Symbol Divide => Symbol(() => Rule()
            .Match(Multiplicative, "/", Unit).To((int a, string _, int b) => a / b));

        public Symbol Additive => Symbol(() => Rule()
            .Match(Multiplicative).ToIdentity<int>()
            .Match(Add).ToIdentity<int>()
            .Match(Subtract).ToIdentity<int>());

        public Symbol Add => Symbol(() => Rule()
            .Match(Additive, "+", Multiplicative).To((int a, string _, int b) => a + b));

        public Symbol Subtract => Symbol(() => Rule()
            .Match(Additive, "-", Multiplicative).To((int a, string _, int b) => a - b));

        public IRule Expression() => 
            Rule().Match(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
