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
        public IEnumerable<Rule> Rules => this.RulesRepresentation;

        private ImmutableList<Rule> RulesRepresentation { get; }

        public Grammar(params Rule[] rules) : this((IEnumerable<Rule>)rules)
        {
        }

        public Grammar(IEnumerable<Rule> rules) :
            this(rules.SelectMany((rule, index) => index == 0
                    ? new[] {Rule.AugmentedGrammarRoot(rule.Head.Value), rule}
                    : new[] {rule})
                .ToImmutableList())
        {
        }

        private Grammar(ImmutableList<Rule> rules)
        {
            this.RulesRepresentation = rules;
        }

        public Grammar AddRange(IEnumerable<Rule> rules) =>
            this.RulesRepresentation.Any() ? new Grammar(this.RulesRepresentation.AddRange(rules))
            : new Grammar(rules);

        public int SortOrderFor(NonTerminal nonTerminal) =>
            Array.IndexOf(this.SortOrder.ToArray(), nonTerminal);

        private IEnumerable<NonTerminal> SortOrder =>
            this.Rules.Select(rule => rule.Head).Distinct();

        public Parser BuildParser() =>
            ParserBuilder.For(this).Build();

        public override string ToString() => Formatter.ToString(this);
    }
}