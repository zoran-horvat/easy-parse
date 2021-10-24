using System;

namespace EasyParse.Fluent.Symbols
{
    public class RecursiveNonTerminalSymbol : Symbol
    {
        public RecursiveNonTerminalSymbol(Func<IRule> factory)
        {
            this.Factory = factory;
        }

        public override Type Type => this.Materialize().Type;
        private Func<IRule> Factory { get; }

        public NonTerminalSymbol Materialize() =>
            new NonTerminalSymbol(this.Factory());

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            this.Materialize().ToSymbolModel();

        public override string ToString() =>
            this.Materialize().Head.Name;
    }
}
