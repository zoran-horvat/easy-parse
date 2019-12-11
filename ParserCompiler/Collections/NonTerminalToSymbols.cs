using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public abstract class NonTerminalToSymbols : KeyedSet<NonTerminal, Symbol>
    {
        protected NonTerminalToSymbols(NonTerminal key) : base(key) { }

        protected NonTerminalToSymbols(NonTerminal key, IEnumerable<Symbol> content) : base(key, content.AsSet()) { }

        private NonTerminalToSymbols(NonTerminal key, Set<Symbol> content) : base(key, content) { }

        protected abstract string PrintableName { get; }

        public override string ToString() =>
            $"{this.PrintableName}({base.Key.Value}) = {{{this.ValuesToString()}}}";

        private string ValuesToString() =>
            string.Join(string.Empty, this.OrderBy(x => x).Select(value => value.Value).ToArray());
    }
}
