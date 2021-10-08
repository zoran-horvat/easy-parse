using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        public IRule Value() => Rule()
            .Regex("number", @"\d+").Map<string, int>(int.Parse)
            .Literal("(").Symbol(Additive).Literal(")").Map((string _, int additive, string _) => additive);

        public IRule Multiplicative() => Rule()
            .Symbol(Value).MapIdentity<int>()
            .Symbol(Multiplicative).Literal("*").Symbol(Value).Map((int a, string _, int b) => a * b);

        public IRule Additive() => Rule()
            .Symbol(Multiplicative).MapIdentity<int>()
            .Symbol(Additive).Literal("+").Symbol(Multiplicative).Map((int a, string _, int b) => a + b);

        public IRule Expression() => Rule().Symbol(Additive).MapIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
