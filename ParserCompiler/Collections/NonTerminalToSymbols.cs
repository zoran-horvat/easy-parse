using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class NonTerminalToSymbols : KeyedSet<NonTerminal, Symbol>
    {
        public NonTerminalToSymbols(NonTerminal key) : base(key) { }

        public NonTerminalToSymbols(NonTerminal key, IEnumerable<Symbol> content) : base(key, content.AsSet()) { }

        private NonTerminalToSymbols(NonTerminal key, Set<Symbol> content) : base(key, content) { }

        public NonTerminalToSymbols Union(NonTerminalToSymbols other) =>
            new NonTerminalToSymbols(this.Key, this.Representation.Union(other));

        public NonTerminalToSymbols PurgeNonTerminals() =>
            new NonTerminalToSymbols(this.Key, this.Representation.OfType<Terminal>());

        public override string ToString() =>
            $"FIRST({base.Key.Value}) = {{{this.ValuesToString()}}}";

        private string ValuesToString() =>
            string.Join(string.Empty, this.Select(value => value.Value).ToArray());
    }
}
