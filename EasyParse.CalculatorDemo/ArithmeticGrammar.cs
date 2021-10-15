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
            .Match("(", Symbol(Additive), ")").To((string _, int additive, string _) => additive));

        private IRule Multiplicative() => Rule()
            .Match(Unit).ToIdentity<int>()
            .Match(Symbol(Multiply)).ToIdentity<int>()
            .Match(Symbol(Divide)).ToIdentity<int>();

        public IRule Multiply() => Rule()
            .Match(Symbol(Multiplicative), "*", Unit).To((int a, string _, int b) => a * b);

        public IRule Divide() => Rule()
            .Match(Symbol(Multiplicative), "/", Unit).To((int a, string _, int b) => a / b);

        public IRule Additive() => Rule()
            .Match(Symbol(Multiplicative)).ToIdentity<int>()
            .Match(Symbol(Add)).ToIdentity<int>()
            .Match(Symbol(Subtract)).ToIdentity<int>();

        public IRule Add() => Rule()
            .Match(Symbol(Additive), "+", Symbol(Multiplicative)).To((int a, string _, int b) => a + b);

        public IRule Subtract() => Rule()
            .Match(Symbol(Additive), "-", Symbol(Multiplicative)).To((int a, string _, int b) => a - b);

        public IRule Expression() => 
            Rule().Match(Symbol(Additive)).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
