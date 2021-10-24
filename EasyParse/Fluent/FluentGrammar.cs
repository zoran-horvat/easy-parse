using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.Fluent.Rules;
using EasyParse.Fluent.Symbols;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;

namespace EasyParse.Fluent
{
    public abstract class FluentGrammar : PartialFluentGrammar
    {
        protected abstract IRule Start { get; }
        protected abstract IEnumerable<RegexSymbol> Ignore { get; }

        public Parser BuildParser() =>
            BuildParser(ExpandedProductions);

        public Compiler<T> BuildCompiler<T>() =>
            typeof(T).IsAssignableFrom(Start.Type) ? CreateCompiler<T>(ExpandedProductions)
            : throw new ArgumentException(
                $"Cannot create compiler for type {typeof(T).Name} " +
                $"from grammar which produces type {Start.Type.Name}");

        internal static Parser BuildParser(IEnumerable<RegexSymbol> ignore, NonTerminalName start, IEnumerable<Production> productions) =>
            Parser.From(ToGrammarModel(ignore, start, productions).BuildParser());

        private Parser BuildParser(IEnumerable<Production> productions) =>
            BuildParser(this.Ignore, this.Start.Head, productions);

        private Compiler<T> CreateCompiler<T>(IEnumerable<Production> productions) =>
            BuildParser(productions).ToCompiler<T>(CreateSymbolCompiler(productions));

        private ISymbolCompiler CreateSymbolCompiler(IEnumerable<Production> productions) =>
            new DynamicSymbolicCompiler(productions);

        internal static Grammar ToGrammarModel(IEnumerable<RegexSymbol> ignore, NonTerminalName start, IEnumerable<Production> productions) =>
            productions.Aggregate(
                ToEmptyGrammarModel(start).AddRange(ToIgnoreLexemeModels(ignore)),
                (grammar, production) => production.AppendToGrammarModel(grammar));

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(Ignore, Start.Head, ExpandedProductions);

        private IEnumerable<Production> ExpandedProductions =>
            Start
                .Expand()
                .Select((production, offset) => production.WithReference(RuleReference.CreateOrdinal(offset + 1)));

        private static Grammar ToEmptyGrammarModel(NonTerminalName start) =>
            new(start.ToNonTerminalModel(), Enumerable.Empty<RuleDefinition>());

        private static IEnumerable<Lexeme> ToIgnoreLexemeModels(IEnumerable<RegexSymbol> ignore) =>
            ignore.Select(pattern => pattern.ToIgnoreLexemeModel());
    }
}