using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class FollowSet : NonTerminalToSymbols<Symbol>
    {
        public FollowSet(NonTerminal key) : base(key)
        {
        }

        public FollowSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content)
        {
        }

        public FollowSet Union(FollowSet other) =>
            new FollowSet(base.Key, base.Representation.Union(other.Representation));

        public FollowSet PurgeNonTerminals() =>
            new FollowSet(base.Key, this.OfType<Terminal>());

        protected override string PrintableName => "FOLLOW";
    }
}
