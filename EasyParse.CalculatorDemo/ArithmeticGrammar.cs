using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        public IRule Value() => Rule()
            .Regex("number", @"\d+", int.Parse).ToIdentity<int>()
            .Literal("(").Symbol(Additive).Literal(")").To((string _, int additive, string _) => additive);

        public IRule Multiplicative() => Rule()
            .Symbol(Value).ToIdentity<int>()
            .Symbol(Multiplicative).Literal("*").Symbol(Value).To((int a, string _, int b) => a * b);

        public IRule Additive() => Rule()
            .Symbol(Multiplicative).ToIdentity<int>()
            .Symbol(Additive).Literal("+").Symbol(Multiplicative).To((int a, string _, int b) => a + b);

        public IRule Expression() => Rule().Symbol(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
