using System;

namespace EasyParse.Parsing.Rules.Symbols
{
    abstract class RecursiveNonTerminalSymbol : Symbol
    {
        public abstract NonTerminalSymbol Materialize();
    }

    class RecursiveNonTerminalSymbol<T> : RecursiveNonTerminalSymbol
    {
        public RecursiveNonTerminalSymbol(Func<IRule<T>> factory)
        {
            this.Factory = factory;
        }

        private Func<IRule<T>> Factory { get; }

        public override NonTerminalSymbol Materialize() =>
            new NonTerminalSymbol<T>(this.Factory());

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            this.Materialize().ToSymbolModel();

        public override string ToString() =>
            this.Materialize().Head.Name;
    }
}
