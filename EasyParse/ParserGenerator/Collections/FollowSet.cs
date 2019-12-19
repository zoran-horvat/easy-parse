using System.Collections.Generic;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Collections
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