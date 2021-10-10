using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    public class Production
    {
        public Production(NonTerminal head, ImmutableList<Symbol> body, Transform transform)
        {
            this.Head = head;
            this.Body = body;
            this.Transform = transform;
        }

        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body { get; }
        public Transform Transform { get; }

        public IEnumerable<Production> ChildLines(HashSet<NonTerminal> notIn) =>
            this.Body
                .Select(this.ResolveRecursion)
                .OfType<NonTerminalSymbol>()
                .Where(symbol => !notIn.Contains(symbol.Head))
                .SelectMany(symbol => symbol.Productions);

        private Symbol ResolveRecursion(Symbol symbol) =>
            symbol is RecursiveNonTerminalSymbol recursive ? recursive.Materialize()
            : symbol;

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

        public override string ToString() => 
            ToString(this.Head, this.Body);

        public static string ToString(NonTerminal head, IEnumerable<Symbol> body) =>
            $"{head} -> {ToString(body)}";

        public static string ToString(IEnumerable<Symbol> body) =>
            string.Join(" ", body.Select(x => x.ToString()));
    }
}
