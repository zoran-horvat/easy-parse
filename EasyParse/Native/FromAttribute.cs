using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Fluent;

namespace EasyParse.Native
{
    public class FromAttribute : SymbolAttribute
    {
        public FromAttribute(string name, params string[] otherNames)
            : this(new[] {name}.Concat(otherNames))
        {
        }

        internal FromAttribute(IEnumerable<string> names)
            : this(names.Select(nonTerminal => new NonTerminalName(nonTerminal)))
        {
        }

        internal FromAttribute(IEnumerable<NonTerminalName> names)
        {
            this.NonTerminals = names.ToList();
        }

        public IEnumerable<NonTerminalName> NonTerminals { get; }
    }
}