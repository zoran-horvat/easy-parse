using System.Collections.Generic;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public abstract class NonTerminalToSymbols<TSymbol> : KeyedSet<NonTerminal, TSymbol> where TSymbol : Symbol
    {
        protected NonTerminalToSymbols(NonTerminal key) : base(key) { }

        protected NonTerminalToSymbols(NonTerminal key, IEnumerable<TSymbol> content) : base(key, content.AsSet()) { }

        protected abstract string PrintableName { get; }

        public override string ToString() => Formatting.NamedToString(this, this.PrintableName);
    }
}
