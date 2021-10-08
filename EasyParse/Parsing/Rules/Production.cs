using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    public class Production
    {
        public Production(NonTerminal head, IEnumerable<Symbol> body)
            : this(head, ImmutableList<Symbol>.Empty.AddRange(body))
        {
        }

        public Production(NonTerminal head) : this(head, ImmutableList<Symbol>.Empty)
        {
        }

        private Production(NonTerminal head, ImmutableList<Symbol> body)
        {
            this.Head = head;
            this.BodyRepresentation = body;
        }

        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body => this.BodyRepresentation;
        private ImmutableList<Symbol> BodyRepresentation { get; }

        public Production Append(Symbol symbol) =>
            new(this.Head, this.BodyRepresentation.Add(symbol));

        public IEnumerable<Production> ChildLines(HashSet<NonTerminal> notIn) =>
            this.Body
                .Select(this.ResolveRecursion)
                .OfType<NonTerminalSymbol>()
                .Where(symbol => !notIn.Contains(symbol.Rule.Head))
                .SelectMany(symbol => symbol.Rule.Productions);

        private Symbol ResolveRecursion(Symbol symbol) =>
            symbol is RecursiveNonTerminalSymbol recursive ? recursive.Materialize()
            : symbol;

        public override string ToString() =>
            $"{this.Head} -> {this.BodyToString}";

        internal RuleDefinition ToRuleDefinitionModel() =>
            new RuleDefinition(this.Head.ToNonTerminalModel(), this.Body.Select(symbol => symbol.ToSymbolModel()).ToArray());

        internal IEnumerable<TerminalSymbol> Terminals =>
            this.Body.OfType<TerminalSymbol>();

        internal IEnumerable<RegexSymbol> RegularExpressions =>
            this.Body.OfType<RegexSymbol>();

        internal ParserGenerator.Models.Rules.Grammar AppendToGrammarModel(ParserGenerator.Models.Rules.Grammar grammar) =>
            grammar
                .Add(this.ToRuleDefinitionModel())
                .AddRange(this.RegularExpressions.Select(expr => expr.ToLexemeModel()));

        private string BodyToString =>
            string.Join(" ", this.Body.Select(x => x.ToString()));
    }
}
