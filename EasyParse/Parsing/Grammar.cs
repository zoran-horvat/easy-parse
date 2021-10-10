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
            Parser.From(this.ToGrammarModel().BuildParser());

        public Compiler<T> BuildCompiler<T>() =>
            typeof(T).IsAssignableFrom(this.Start.Type) ? this.CreateCompiler<T>()
            : throw new ArgumentException(
                $"Cannot create compiler for type {typeof(T).Name} " + 
                $"from grammar which produces type {this.Start.Type.Name}");

        private Compiler<T> CreateCompiler<T>() =>
            this.BuildParser().ToCompiler<T>(this.CreateSymbolCompiler());

        private ISymbolCompiler CreateSymbolCompiler() =>
            new DynamicSymbolicCompiler(this.Start.Expand());

        internal ParserGenerator.Models.Rules.Grammar ToGrammarModel() =>
            this.Start.Expand().Aggregate(
                this.ToEmptyGrammarModel().AddRange(this.ToIgnoreLexemeModels()),
                (grammar, production) => production.AppendToGrammarModel(grammar));

        internal ParserGenerator.Models.Rules.Grammar ToEmptyGrammarModel() =>
            new(this.Start.Head.ToNonTerminalModel(), Enumerable.Empty<RuleDefinition>());

        private IEnumerable<Lexeme> ToIgnoreLexemeModels() =>
            this.Ignore.Select(pattern => pattern.ToIgnoreLexemeModel());
    }
}