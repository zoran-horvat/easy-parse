using System.Collections.Generic;
using System.Linq;
using ParserCompiler.SlrParserCompiler.Models;
using ParserCompiler.SlrParserCompiler.Models.Symbols;

namespace ParserCompiler.SlrParserCompiler.Collections
{
    public class FirstSet : KeyedSet<NonTerminal, Symbol>
    {
        public FirstSet(NonTerminal key) : base(key) { }

        public FirstSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content.AsSet()) { }

        private FirstSet(NonTerminal key, Set<Symbol> content) : base(key, content) { }

        public FirstSet Union(FirstSet other) =>
            new FirstSet(this.Key, this.Representation.Union(other));

        public FirstSet PurgeNonTerminals() =>
            new FirstSet(this.Key, this.Representation.OfType<Terminal>());

        public override string ToString() =>
            $"FIRST({base.Key.Value}) = {{{this.ValuesToString()}}}";

        private string ValuesToString() =>
            string.Join(string.Empty, this.Select(value => value.Value).ToArray());
    }
}
