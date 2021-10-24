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

        private Parser BuildParser(IEnumerable<Production> productions) =>
            Parser.From(ToGrammarModel(productions).BuildParser());

        private Compiler<T> CreateCompiler<T>(IEnumerable<Production> productions) =>
            BuildParser(productions).ToCompiler<T>(CreateSymbolCompiler(productions));

        private ISymbolCompiler CreateSymbolCompiler(IEnumerable<Production> productions) =>
            new DynamicSymbolicCompiler(productions);

        internal Grammar ToGrammarModel(IEnumerable<Production> productions) =>
            productions.Aggregate(
                ToEmptyGrammarModel().AddRange(ToIgnoreLexemeModels()),
                (grammar, production) => production.AppendToGrammarModel(grammar));

        public IEnumerable<string> ToGrammarFileContent() =>
            new GrammarToGrammarFileFormatter().Convert(Ignore, Start.Head, ExpandedProductions);

        private IEnumerable<Production> ExpandedProductions =>
            Start
                .Expand()
                .Select((production, offset) => production.WithReference(RuleReference.CreateOrdinal(offset + 1)));

        internal Grammar ToEmptyGrammarModel() =>
            new(Start.Head.ToNonTerminalModel(), Enumerable.Empty<RuleDefinition>());

        private IEnumerable<Lexeme> ToIgnoreLexemeModels() =>
            Ignore.Select(pattern => pattern.ToIgnoreLexemeModel());
    }
}