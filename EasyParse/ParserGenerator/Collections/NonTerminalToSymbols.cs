using System.Collections.Generic;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Collections
{
    public abstract class NonTerminalToSymbols<TSymbol> : LabeledSet<NonTerminal, TSymbol> where TSymbol : Symbol
    {
        protected NonTerminalToSymbols(NonTerminal label) : base(label) { }

        protected NonTerminalToSymbols(NonTerminal label, IEnumerable<TSymbol> content) : base(label, content.AsSet()) { }

        protected abstract string PrintableName { get; }

        public override string ToString() => Formatter.NamedToString(this, this.PrintableName);
    }
}
