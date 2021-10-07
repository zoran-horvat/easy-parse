using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticRules : ParsingRules
    {
        public Rule Number => Rule()
            .Match(Regex("number", @"\d+"));

        public Rule Multiplicative => Rule()
            .Match(Number)
            .Or(Number, "*", Number);

        public Rule Additive => Rule()
            .Match(Multiplicative);

        public Rule Expression => Rule()
            .Match(Additive);

        protected override Rule Start => this.Expression;
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
