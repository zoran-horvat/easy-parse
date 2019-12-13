using System.Collections.Generic;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class FollowSet : NonTerminalToSymbols<Terminal>
    {
        public FollowSet(NonTerminal key) : base(key)
        {
        }

        public FollowSet(NonTerminal key, IEnumerable<Terminal> content) : base(key, content)
        {
        }

        protected override string PrintableName => "FOLLOW";
    }
}