using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class IntermediateFirstSet : NonTerminalToSymbols<Symbol>
    {
        public IntermediateFirstSet(NonTerminal key) : base(key)
        {
        }

        public IntermediateFirstSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content)
        {
        }

        public IntermediateFirstSet Union(IntermediateFirstSet other) =>
            new IntermediateFirstSet(this.Key, this.Values.Union(other));

        public FirstSet PurgeNonTerminals() =>
            new FirstSet(this.Key, this.Values.OfType<Terminal>());

        protected override string PrintableName => "FIRST";
    }
}