using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public class Rule
    {
        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body { get; }
     
        public Rule(NonTerminal head, IEnumerable<Symbol> body)
        {
            this.Head = head;
            this.Body = body.ToList();
        }

        public static Rule AugmentedGrammarRoot(string startingSymbol) =>
            new Rule(new NonTerminal("S'"), new Symbol[] {new NonTerminal("S")});

        public override string ToString() =>
            $"{this.Head} -> {string.Join(string.Empty, this.Body.Select(x => x.ToString()).ToArray())}";
    }
}