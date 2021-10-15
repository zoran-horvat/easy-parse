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
            .Match(RegexSymbol.Create<int>("number", new Regex(@"\d+"), int.Parse)).ToIdentity<int>()
            .Match("-", Unit).To((string _, int value) => -value)
            .Match("(", Additive, ")").To((string _, int additive, string _) => additive);      

        private NonTerminal Multiplicative => () => Rule()
            .Match(Unit).ToIdentity<int>()
            .Match(Multiply).ToIdentity<int>()
            .Match(Divide).ToIdentity<int>();

        private NonTerminal Multiply => () => Rule()
            .Match(Multiplicative, "*", Unit).To((int a, string _, int b) => a * b);

        private NonTerminal Divide => () => Rule()
            .Match(Multiplicative, "/", Unit).To((int a, string _, int b) => a / b);

        public NonTerminal Additive => () => Rule()
            .Match(Multiplicative).ToIdentity<int>()
            .Match(Add).ToIdentity<int>()
            .Match(Subtract).ToIdentity<int>();

        public NonTerminal Add => () => Rule()
            .Match(Additive, "+", Multiplicative).To((int a, string _, int b) => a + b);

        public NonTerminal Subtract => () => Rule()
            .Match(Additive, "-", Multiplicative).To((int a, string _, int b) => a - b);

        public IRule Expression() => 
            Rule().Match(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { WhiteSpace() };
    }
}
