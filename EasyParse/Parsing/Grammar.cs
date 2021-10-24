using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public abstract class Grammar : PartialGrammar
    {
        protected abstract IRule Start { get; }
        protected abstract IEnumerable<RegexSymbol> Ignore { get; }

        public Parser BuildParser() =>
            this.BuildParser(this.ExpandedProductions);

        public Compiler<T> BuildCompiler<T>() =>
            typeof(T).IsAssignableFrom(this.Start.Type) ? this.CreateCompiler<T>(this.ExpandedProductions)
            : throw new ArgumentException(
                $"Cannot create compiler for type {typeof(T).Name} " + 
                $"from grammar which produces type {this.Start.Type.Name}");

        private Parser BuildParser(IEnumerable<Production> productions) =>
            Parser.From(this.ToGrammarModel(productions).BuildParser());

        private Compiler<T> CreateCompiler<T>(IEnumerable<Production> productions) =>
            this.BuildParser(productions).ToCompiler<T>(this.CreateSymbolCompiler(productions));

        private ISymbolCompiler CreateSymbolCompiler(IEnumerable<Production> productions) =>
            new DynamicSymbolicCompiler(productions);

        internal ParserGenerator.Models.Rules.Grammar ToGrammarModel(IEnumerable<Production> productions) =>
            productions.Aggregate(
                this.ToEmptyGrammarModel().AddRange(this.ToIgnoreLexemeModels()),
                (grammar, production) => production.AppendToGrammarModel(grammar));

        private IEnumerable<Production> ExpandedProductions => 
            this.Start
                .Expand()
                .Select((production, offset) => production.WithReference(RuleReference.CreateOrdinal(offset + 1)));

        internal ParserGenerator.Models.Rules.Grammar ToEmptyGrammarModel() =>
            new(this.Start.Head.ToNonTerminalModel(), Enumerable.Empty<RuleDefinition>());

        private IEnumerable<Lexeme> ToIgnoreLexemeModels() =>
            this.Ignore.Select(pattern => pattern.ToIgnoreLexemeModel());
    }
}