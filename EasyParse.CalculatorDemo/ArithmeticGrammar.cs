using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        public IRule Value() => Rule()
            .Regex("number", @"\d+").End()
            .Literal("(").Symbol(Additive).Literal(")").End();

        public IRule Multiplicative() => Rule()
            .Symbol(Value).End()
            .Symbol(Multiplicative).Literal("*").Symbol(Value).End();

        public IRule Additive() => Rule()
            .Symbol(Multiplicative).End()
            .Symbol(Additive).Literal("+").Symbol(Multiplicative).End();

        public IRule Expression() => Rule().Symbol(Additive).End();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
