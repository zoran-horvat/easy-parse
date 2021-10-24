using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.Fluent.Symbols;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Fluent.Rules
{
    public class Production
    {
        public Production(NonTerminalName head, ImmutableList<Symbol> body, Transform transform)
            : this(RuleReference.Empty(), head, (IEnumerable<Symbol>)body, transform)
        {
        }

        private Production(RuleReference reference, NonTerminalName head, IEnumerable<Symbol> body, Transform transform)
        {
            this.Reference = reference;
            this.Head = head;
            this.Body = body;
            this.Transform = transform;
        }

        public RuleReference Reference { get; }
        public NonTerminalName Head { get; }
        public IEnumerable<Symbol> Body { get; }
        public Transform Transform { get; }
        public Type ReturnType => this.Transform.ReturnType;

        public Production WithReturnType(Type type) =>
            this.WithTransform(this.Transform.WithReturnType(type));

        public Production WithReference(RuleReference reference) =>
            new Production(reference, this.Head, this.Body, this.Transform);

        private Production WithTransform(Transform transform) =>
            transform.Equals(this.Transform) ? this
            : new Production(this.Reference, this.Head, this.Body, transform);

        public IEnumerable<Production> ChildLines(HashSet<NonTerminalName> notIn) =>
            this.Body
                .Select(this.ResolveRecursion)
                .OfType<NonTerminalSymbol>()
                .Where(symbol => !notIn.Contains(symbol.Head))
                .SelectMany(symbol => symbol.Productions);

        private Symbol ResolveRecursion(Symbol symbol) =>
            symbol is RecursiveNonTerminalSymbol recursive ? recursive.Materialize()
            : symbol;

        internal RuleDefinition ToRuleDefinitionModel() =>
            new RuleDefinition(this.Head.ToNonTerminalModel(), this.ToSymbolModels().ToArray())
                .WithReference(this.Reference);

        private IEnumerable<EasyParse.ParserGenerator.Models.Symbols.Symbol> ToSymbolModels() =>
            this.Body.Select(symbol => symbol.ToSymbolModel());

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

        public static string ToString(NonTerminalName head, IEnumerable<Symbol> body) =>
            $"{head} -> {ToString(body)}";

        public static string ToString(IEnumerable<Symbol> body) =>
            string.Join(" ", body.Select(x => x.ToString()));
    }
}
