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
            .Match("-", Unit).To((string _, int x) => -x)
            .Match("(", Additive, ")").To((string _, int additive, string _) => additive);
        
        private NonTerminal Multiplicative => () => Rule()
            .MatchOne<int>(Unit, Multiplied, Divided);

        private NonTerminal Multiplied => () => Rule()
            .Match(Multiplicative, "*", Unit).To((int a, string _, int b) => a * b);

        private NonTerminal Divided => () => Rule()
            .Match(Multiplicative, "/", Unit).To((int a, string _, int b) => a / b);

        public NonTerminal Additive => () => Rule()
            .MatchOne<int>(Multiplicative, Added, Subtracted);

        public NonTerminal Added => () => Rule()
            .Match(Additive, "+", Multiplicative).To((int a, string _, int b) => a + b);

        public NonTerminal Subtracted => () => Rule()
            .Match(Additive, "-", Multiplicative).To((int a, string _, int b) => a - b);

        public IRule Expression() => 
            Rule().Match(Additive).ToIdentity<int>();

        protected override IRule Start => this.Expression();
        protected override IEnumerable<RegexSymbol> Ignore => new[] { Pattern.WhiteSpace };
    }
}
