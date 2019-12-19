using System.Collections.Generic;
using EasyParse.Models.Symbols;

namespace EasyParse.Collections
{
    public class FirstSet : NonTerminalToSymbols<Terminal>
    {
        public FirstSet(NonTerminal label) : base(label) { }

        public FirstSet(NonTerminal label, IEnumerable<Terminal> content) : base(label, content)
        {
        }

        protected override string PrintableName => "FIRST";
    }
}