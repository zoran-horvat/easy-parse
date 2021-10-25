using System.Collections.Generic;
using System.Linq;
using EasyParse.Fluent;

namespace EasyParse.Native.Annotations
{
    public class FromAttribute : SymbolAttribute
    {
        public FromAttribute(string name, params string[] otherNames)
            : this(new[] { name }.Concat(otherNames))
        {
        }

        internal FromAttribute(IEnumerable<string> names)
            : this(names.Select(nonTerminal => new NonTerminalName(nonTerminal)))
        {
        }

        internal FromAttribute(IEnumerable<NonTerminalName> names)
        {
            NonTerminals = names.ToList();
        }

        public IEnumerable<NonTerminalName> NonTerminals { get; }
    }
}