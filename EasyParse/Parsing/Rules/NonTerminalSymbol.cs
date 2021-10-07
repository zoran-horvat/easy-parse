using System.Collections.Generic;

namespace EasyParse.Parsing.Rules
{
    class NonTerminalSymbol : Symbol
    {
        public NonTerminalSymbol(Productions productions)
        {
            this.Productions = productions;
        }

        public Productions Productions { get; }
        public IEnumerable<Production> Lines => this.Productions.ProductionLines;

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Productions.Head.Name);
    }
}
