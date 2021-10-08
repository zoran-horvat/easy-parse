using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    public abstract class Grammar<T> : PartialGrammar
    {
        protected abstract IRule<T> Start { get; }
        protected abstract IEnumerable<RegexSymbol> Ignore { get; }

        public Parser BuildParser() =>
            Parser.From(this.ToGrammarModel().BuildParser());

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