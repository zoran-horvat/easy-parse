using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Builders;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class Grammar
    {
        private NonTerminal StartSymbol { get; }
        public IEnumerable<Rule> Rules => this.AugmentedGrammarRule.Concat(this.RulesRepresentation);
        private IEnumerable<Rule> AugmentedGrammarRule => new[] {Rule.AugmentedGrammarRoot(this.StartSymbol)};
        public IEnumerable<IgnoreLexeme> IgnoreLexemes => this.LexemesRepresentation.OfType<IgnoreLexeme>();
        public IEnumerable<LexemePattern> LexemePatterns => this.LexemesRepresentation.OfType<LexemePattern>();
        public IEnumerable<ConstantLexeme> ConstantLexemes => this.Rules.SelectMany(rule => rule.ConstantLexemes).Distinct();

        private ImmutableList<Rule> RulesRepresentation { get; }

        private ImmutableList<Lexeme> LexemesRepresentation { get; }

        public Grammar(NonTerminal startSymbol, params Rule[] rules) : this(startSymbol, (IEnumerable<Rule>)rules)
        {
        }

        public Grammar(NonTerminal startSymbol, IEnumerable<Rule> rules) :
            this(startSymbol, rules.ToImmutableList(), ImmutableList<Lexeme>.Empty)
        {
        }

        private Grammar(NonTerminal startSymbol, ImmutableList<Rule> rules, ImmutableList<Lexeme> lexemes)
        {
            this.StartSymbol = startSymbol;
            this.RulesRepresentation = rules;
            this.LexemesRepresentation = lexemes;
        }

        public Grammar Add(Rule rule) =>
            new Grammar(this.StartSymbol, this.RulesRepresentation.Add(rule), this.LexemesRepresentation);

        public Grammar AddRange(IEnumerable<Lexeme> lexemes) =>
            lexemes.Aggregate(this, (grammar, lexeme) => grammar.Add(lexeme));

        private Grammar Add(Lexeme lexeme) =>
            this.LexemesRepresentation.Any(existing => existing.Pattern.ToString().Equals(lexeme.Pattern.ToString())) ? this
            : new Grammar(this.StartSymbol, this.RulesRepresentation, this.LexemesRepresentation.Add(lexeme));
        
        public int SortOrderFor(NonTerminal nonTerminal) =>
            Array.IndexOf(this.SortOrder.ToArray(), nonTerminal);

        private IEnumerable<NonTerminal> SortOrder =>
            this.Rules.Select(rule => rule.Head).Distinct();

        public Parser BuildParser() =>
            ParserBuilder.For(this).Build();

        public override string ToString() => Formatter.ToString(this);
    }
}