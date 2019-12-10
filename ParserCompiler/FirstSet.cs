using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Symbols;

namespace ParserCompiler
{
    public class FirstSet : KeyedSet<NonTerminal, Symbol>
    {
        public FirstSet(NonTerminal key) : base(key)
        {
        }

        public FirstSet(NonTerminal key, IEnumerable<Symbol> content) : base(key, content)
        {
        }

        public override string ToString() =>
            $"FIRST({base.Key.Value}) = {{{this.ValuesToString()}}}";

        private string ValuesToString() =>
            string.Join(string.Empty, this.Select(value => value.Value).ToArray());
    }
}
