using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class IntermediateFollowSet : NonTerminalToSymbols<Symbol>
    {
        public IntermediateFollowSet(NonTerminal key) : base(key)
        {
        }

        public IntermediateFollowSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content)
        {
        }

        public IntermediateFollowSet Union(IntermediateFollowSet other) =>
            new IntermediateFollowSet(base.Key, base.Representation.Union(other.Representation));

        public FollowSet PurgeNonTerminals() =>
            new FollowSet(base.Key, this.OfType<Terminal>());

        protected override string PrintableName => "FOLLOW";
    }
}
