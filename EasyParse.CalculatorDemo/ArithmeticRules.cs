using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticRules : ParsingRules
    {
        public Productions Number => Rule()
            .Match(Regex("number", @"\d+"));

        public Productions Multiplicative => Rule()
            .Match(Number);

        protected override Productions Start => this.Multiplicative;
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
