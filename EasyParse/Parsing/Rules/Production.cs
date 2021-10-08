using System;
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

        public IEnumerable<Production> ChildLines(HashSet<NonTerminal> notIn) =>
            this.Body
                .OfType<NonTerminalSymbol>()
                .Where(symbol => !notIn.Contains(symbol.Rule.Head))
                .SelectMany(symbol => symbol.Rule.ProductionLines);

        public override string ToString() =>
            $"{this.Head} -> {this.BodyToString}";

        internal RuleDefinition ToRuleDefinitionModel() =>
            new RuleDefinition(this.Head.ToNonTerminalModel(), this.Body.Select(symbol => symbol.ToSymbolModel()).ToArray());

        internal IEnumerable<TerminalSymbol> Terminals =>
            this.Body.OfType<TerminalSymbol>();

        internal IEnumerable<RegexSymbol> RegularExpressions =>
            this.Body.OfType<RegexSymbol>();

        internal Grammar AppendToGrammarModel(Grammar grammar)
        {
            Grammar grammar1 = grammar
                .Add(this.ToRuleDefinitionModel())
                .AddRange(this.RegularExpressions.Select(expr => expr.ToLexemeModel()));
            return grammar1;
        }

        private string BodyToString =>
            string.Join(" ", this.Body.Select(x => x.ToString()));
    }
}
