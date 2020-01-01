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
        public IEnumerable<Rule> Rules => 
            this.RulesRepresentation
                .SelectMany((rule, index) => 
                    index == 0 ? new[] {Rule.AugmentedGrammarRoot(rule.Head.Value), rule}
                    : new[] {rule});
        
        public IEnumerable<IgnoreLexeme> IgnoreLexemes => this.IgnoreLexemesRepresentation;

        private ImmutableList<Rule> RulesRepresentation { get; }

        private ImmutableList<IgnoreLexeme> IgnoreLexemesRepresentation { get; }

        public Grammar(params Rule[] rules) : this((IEnumerable<Rule>)rules)
        {
        }

        public Grammar(IEnumerable<Rule> rules) :
            this(rules.ToImmutableList(), ImmutableList<IgnoreLexeme>.Empty)
        {
        }

        private Grammar(ImmutableList<Rule> rules, ImmutableList<IgnoreLexeme> ignoreLexemes)
        {
            this.RulesRepresentation = rules;
            this.IgnoreLexemesRepresentation = ignoreLexemes;
        }

        public Grammar AddRange(IEnumerable<Rule> rules) =>
            new Grammar(this.RulesRepresentation.AddRange(rules), this.IgnoreLexemesRepresentation);

        public Grammar Add(Rule rule) =>
            new Grammar(this.RulesRepresentation.Add(rule), this.IgnoreLexemesRepresentation);

        public Grammar AddRange(IEnumerable<IgnoreLexeme> ignores) =>
            new Grammar(this.RulesRepresentation, this.IgnoreLexemesRepresentation.AddRange(ignores));

        public int SortOrderFor(NonTerminal nonTerminal) =>
            Array.IndexOf(this.SortOrder.ToArray(), nonTerminal);

        private IEnumerable<NonTerminal> SortOrder =>
            this.Rules.Select(rule => rule.Head).Distinct();

        public Parser BuildParser() =>
            ParserBuilder.For(this).Build();

        public override string ToString() => Formatter.ToString(this);
    }
}