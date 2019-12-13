using System.Collections.Generic;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class FirstSet : NonTerminalToSymbols<Terminal>
    {
        public FirstSet(NonTerminal key) : base(key) { }

        public FirstSet(NonTerminal key, IEnumerable<Terminal> content) : base(key, content)
        {
        }

        protected override string PrintableName => "FIRST";
    }
}