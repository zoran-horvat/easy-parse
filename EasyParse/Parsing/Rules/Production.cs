using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Parsing.Rules
{
    class Production
    {
        public Production(NonTerminal head, IEnumerable<Symbol> body)
        {
            this.Head = head;
            this.Body = body.ToList();
        }

        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body { get; }

        public IEnumerable<Production> ChildLines =>
            this.Body.OfType<NonTerminalSymbol>().SelectMany(symbol => symbol.Lines);

        public override string ToString() =>
            $"{this.Head} -> {this.BodyToString}";

        internal RuleDefinition ToRuleDefinitionModel() =>
            new RuleDefinition(this.Head.ToNonTerminalModel(), this.Body.Select(symbol => symbol.ToSymbolModel()).ToArray());

        internal IEnumerable<TerminalSymbol> Terminals =>
            this.Body.OfType<TerminalSymbol>();

        internal Grammar AppendToGrammarModel(Grammar grammar) => grammar
            .Add(this.ToRuleDefinitionModel())
            .AddRange(this.Terminals.Select(terminal => terminal.ToLexemeModel()));

        private string BodyToString =>
            string.Join(" ", this.Body.Select(x => x.ToString()));
    }
}
