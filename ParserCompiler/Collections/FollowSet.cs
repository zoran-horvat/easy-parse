using System.Collections.Generic;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class FollowSet : NonTerminalToSymbols<Terminal>
    {
        public FollowSet(NonTerminal label) : base(label)
        {
        }

        public FollowSet(NonTerminal label, IEnumerable<Terminal> content) : base(label, content)
        {
        }

        protected override string PrintableName => "FOLLOW";
    }
}