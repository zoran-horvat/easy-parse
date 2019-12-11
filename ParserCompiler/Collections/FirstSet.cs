using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class FirstSet : NonTerminalToSymbols
    {
        public FirstSet(NonTerminal key) : base(key)
        {
        }

        public FirstSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content)
        {
        }

        public FirstSet Union(FirstSet other) =>
            new FirstSet(this.Key, this.Representation.Union(other));

        public FirstSet PurgeNonTerminals() =>
            new FirstSet(this.Key, this.Representation.OfType<Terminal>());

        protected override string PrintableName => "FIRST";
    }
}