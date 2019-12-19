using System.Collections.Generic;
using System.Linq;
using EasyParse.Models.Symbols;

namespace EasyParse.Collections
{
    public class IntermediateFollowSet : NonTerminalToSymbols<Symbol>
    {
        public IntermediateFollowSet(NonTerminal label) : base(label)
        {
        }

        public IntermediateFollowSet(NonTerminal label, IEnumerable<Symbol> content) : base(label, content)
        {
        }

        public IntermediateFollowSet Union(IntermediateFollowSet other) =>
            new IntermediateFollowSet(base.Label, base.Values.Union(other.Values));

        public FollowSet PurgeNonTerminals() =>
            new FollowSet(base.Label, this.OfType<Terminal>());

        protected override string PrintableName => "FOLLOW";
    }
}
