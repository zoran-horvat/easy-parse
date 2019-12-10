using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Symbols;

namespace ParserCompiler
{
    public class Rule
    {
        private NonTerminal Head { get; }
        private IEnumerable<Symbol> Body { get; }
     
        public Rule(NonTerminal head, IEnumerable<Symbol> body)
        {
            this.Head = head;
            this.Body = body.ToList();
        }

        public override string ToString() =>
            $"{this.Head} -> {string.Join(string.Empty, this.Body.Select(x => x.ToString()).ToArray())}";
    }
}