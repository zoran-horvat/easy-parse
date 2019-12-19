using System.Collections.Generic;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public abstract class NonTerminalToSymbols<TSymbol> : LabeledSet<NonTerminal, TSymbol> where TSymbol : Symbol
    {
        protected NonTerminalToSymbols(NonTerminal label) : base(label) { }

        protected NonTerminalToSymbols(NonTerminal label, IEnumerable<TSymbol> content) : base(label, content.AsSet()) { }

        protected abstract string PrintableName { get; }

        public override string ToString() => Formatting.NamedToString(this, this.PrintableName);
    }
}
