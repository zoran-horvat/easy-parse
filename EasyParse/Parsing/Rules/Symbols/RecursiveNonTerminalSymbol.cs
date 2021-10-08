using System;

namespace EasyParse.Parsing.Rules.Symbols
{
    class RecursiveNonTerminalSymbol : Symbol
    {
        public RecursiveNonTerminalSymbol(Func<IRule> factory)
        {
            this.Factory = factory;
        }

        private Func<IRule> Factory { get; }

        public NonTerminalSymbol Materialize() =>
            new NonTerminalSymbol(this.Factory());

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            this.Materialize().ToSymbolModel();

        public override string ToString() =>
            this.Materialize().Head.Name;
    }
}
