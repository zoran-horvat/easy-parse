using System.Collections.Generic;
using System.Text.RegularExpressions;
using EasyParse.Parsing;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.CalculatorDemo
{
    class ArithmeticGrammar : Grammar
    {
        private NonTerminal Unit => () => Rule()
            .Match<int>(RegexSymbol.Create<int>("number", new Regex(@"\d+"), int.Parse))
            .Match("-", Unit).To((string _, int value) => -value)
            .Match("(", Additive, ")").To((string _, int additive, string _) => additive);

        private NonTerminal Multiplicative => () => Rule()
            .Match<int>(Unit)
            .Match<int>(Multiplied)
            .Match<int>(Divided);

        private NonTerminal Multiplied => () => Rule()
            .Match(Multiplicative, "*", Unit).To((int a, string _, int b) => a * b);

        private NonTerminal Divided => () => Rule()
            .Match(Multiplicative, "/", Unit).To((int a, string _, int b) => a / b);

        public NonTerminal Additive => () => Rule()
            .Match<int>(Multiplicative)
            .Match<int>(Added)
            .Match<int>(Subtracted);

        public NonTerminal Added => () => Rule()
            .Match(Additive, "+", Multiplicative).To((int a, string _, int b) => a + b);

        public NonTerminal Subtracted => () => Rule()
            .Match(Additive, "-", Multiplicative).To((int a, string _, int b) => a - b);

        public IRule Expression() => 
            Rule().Match(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
