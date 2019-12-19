using System.Collections.Generic;
using System.Linq;
using EasyParse.Models.Symbols;

namespace EasyParse.Collections
{
    public class IntermediateFirstSet : NonTerminalToSymbols<Symbol>
    {
        public IntermediateFirstSet(NonTerminal label) : base(label)
        {
        }

        public IntermediateFirstSet(NonTerminal label, IEnumerable<Symbol> content) : base(label, content)
        {
        }

        public IntermediateFirstSet Union(IntermediateFirstSet other) =>
            new IntermediateFirstSet(this.Label, this.Values.Union(other));

        public FirstSet PurgeNonTerminals() =>
            new FirstSet(this.Label, this.Values.OfType<Terminal>());

        protected override string PrintableName => "FIRST";
    }
}