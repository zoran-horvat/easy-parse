﻿using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
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

        public Set<NonTerminalToSymbols> FirstSets => 
            FirstSetsBuilder.From(this.Rules);

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Rules.Select(rule => rule.ToString()));
    }
}