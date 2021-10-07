using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Rules;

namespace EasyParse.Parsing
{
    public abstract class ParsingRules
    {
        protected RegexSymbol Regex(string name, string expression) =>
            new RegexSymbol(name, new Regex(expression));

        protected RegexSymbol WhiteSpace() =>
            new RegexSymbol("white space", new Regex(@"\s+"));

        protected Rule Rule([CallerMemberName] string nonTerminalName = "") =>
            new Rule(new NonTerminal(nonTerminalName));

        protected abstract Rule Start { get; }
        protected abstract IEnumerable<RegexSymbol> Ignore { get; }

        public Grammar ToGrammarModel() =>
            this.Start.Expand().Aggregate(
                this.ToEmptyGrammarModel().AddRange(this.ToIgnoreLexemeModels()),
                (grammar, production) => production.AppendToGrammarModel(grammar));

        private Grammar ToEmptyGrammarModel() =>
            new Grammar(this.Start.Head.ToNonTerminalModel(), Enumerable.Empty<RuleDefinition>());

        private IEnumerable<Lexeme> ToIgnoreLexemeModels() =>
            this.Ignore.Select(pattern => pattern.ToIgnoreLexemeModel());
    }
}