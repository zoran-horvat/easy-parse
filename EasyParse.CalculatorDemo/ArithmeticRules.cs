using System.Collections.Generic;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticRules : ParsingRules
    {
        public Productions Number => Rule()
            .Match(Regex("number", @"\d+"));

        protected override Productions Start => this.Number;
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
