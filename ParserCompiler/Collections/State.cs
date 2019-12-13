using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class State
    {
        private ImmutableList<StateElement> Elements { get; }

        public State(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Elements = this.ToElements(rules, followSets).ToImmutableList();
        }

        private IEnumerable<StateElement> ToElements(IEnumerable<Rule> rules, Set<FollowSet> followSets) =>
            rules.Select(rule => this.ToElement(rule, followSets));

        private StateElement ToElement(Rule rule, Set<FollowSet> followSets) =>
            new StateElement(new Progression(rule), this.FollowSetFor(rule, followSets));

        private Set<Terminal> FollowSetFor(Rule rule, Set<FollowSet> followSets) =>
            followSets.First(set => set.Key.Equals(rule.Head)).AsSet();

        public override string ToString() =>
            this.ToString(this.ProgressionsToStringWidth);

        private string ToString(int progressionWidth) =>
            string.Join(Environment.NewLine, this.Elements.Select(element => this.ToString(element, progressionWidth)).ToArray());

        private string ToString(StateElement element, int progressionWidth) =>
            $"{element.Progression.ToString().PadRight(progressionWidth)} {{{this.ToString(element.FollowedBy)}}}";

        private string ToString(Set<Terminal> terminals) =>
            string.Join(string.Empty, terminals.OrderBy(x => x).Select(x => $"{x}").ToArray());

        private int ProgressionsToStringWidth =>
            this.Elements.Max(element => element.Progression.ToString().Length);
    }
}
