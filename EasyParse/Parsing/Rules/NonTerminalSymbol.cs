using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    class NonTerminalSymbol : Symbol
    {
        public NonTerminalSymbol(Rule rule)
        {
            this.Rule = rule;
        }

        public Rule Rule { get; }
        public IEnumerable<Production> Lines => this.Rule.ProductionLines;

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Rule.Head.Name);
    }
}
