using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        private NonTerminal Unit => () => Rule()
            .Match<int>(Pattern.Int)
            .Match("-", Unit).To((int x) => -x)
            .Match<int>("(", Additive, ")");
        
        private NonTerminal Multiplicative => () => Rule()
            .Match<int>(Unit)
            .Match(Multiplicative, "*", Unit).To((int a, int b) => a * b)
            .Match(Multiplicative, "/", Unit).To((int a, int b) => a / b);

        public NonTerminal Additive => () => Rule()
            .Match(Multiplicative).ToIdentity<int>()
            .Match(Additive, "+", Multiplicative).To((int a, int b) => a + b)
            .Match(Additive, "-", Multiplicative).To((int a, int b) => a - b);

        public IRule Expression() => 
            Rule().Match(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { Pattern.WhiteSpace };
    }
}
