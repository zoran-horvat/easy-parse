using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        public IRule Unit() => Rule()
            .Regex("number", @"\d+", int.Parse).ToIdentity<int>()
            .Literal("-").Symbol(Unit).To((string _, int value) => -value)
            .Literal("(").Symbol(Additive).Literal(")").To((string _, int additive, string _) => additive);

        public IRule Multiplicative() => Rule()
            .Symbol(Unit).ToIdentity<int>()
            .Symbol(Multiply).ToIdentity<int>()
            .Symbol(Divide).ToIdentity<int>();

        public IRule Multiply() => Rule()
            .Symbol(Multiplicative).Literal("*").Symbol(Unit).To((int a, string _, int b) => a * b);

        public IRule Divide() => Rule()
            .Symbol(Multiplicative).Literal("/").Symbol(Unit).To((int a, string _, int b) => a / b);

        public IRule Additive() => Rule()
            .Symbol(Multiplicative).ToIdentity<int>()
            .Symbol(Add).ToIdentity<int>()
            .Symbol(Subtract).ToIdentity<int>();

        public IRule Add() => Rule()
            .Symbol(Additive).Literal("+").Symbol(Multiplicative).To((int a, string _, int b) => a + b);

        public IRule Subtract() => Rule()
            .Symbol(Additive).Literal("-").Symbol(Multiplicative).To((int a, string _, int b) => a - b);

        public IRule Expression() => Rule().Symbol(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
