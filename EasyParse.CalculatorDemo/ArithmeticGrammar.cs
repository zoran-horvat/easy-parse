using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar<int>
    {
        public IRule<int> Value() => Rule<int>()
            .Regex("number", @"\d+").End()
            .Literal("(").Symbol(Additive).Literal(")").End();

        public IRule<int> Multiplicative() => Rule<int>()
            .Symbol(Value).End()
            .Symbol(Multiplicative).Literal("*").Symbol(Value).End();

        public IRule<int> Additive() => Rule<int>()
            .Symbol(Multiplicative).End()
            .Symbol(Additive).Literal("+").Symbol(Multiplicative).End();

        public IRule<int> Expression() => Rule<int>().Symbol(Additive).End();

        protected override IRule<int> Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
