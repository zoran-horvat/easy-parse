using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Builders;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Rules
{
    public class Grammar
    {
        private IEnumerable<Rule> Rules { get; }
     
        public Grammar(IEnumerable<Rule> rules)
        {
            this.Rules = rules.SelectMany((rule, index) => index == 0
                    ? new[] {Rule.AugmentedGrammarRoot(rule.Head.Value), rule}
                    : new[] {rule})
                .ToList();
        }

        public int SortOrderFor(NonTerminal nonTerminal) =>
            Array.IndexOf(this.SortOrder.ToArray(), nonTerminal);

        private IEnumerable<NonTerminal> SortOrder =>
            this.Rules.Select(rule => rule.Head).Distinct();

        public Parser BuildParser() =>
            ParserBuilder.For(this.Rules).Build();

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Rules.Select(rule => rule.ToString()));
    }
}