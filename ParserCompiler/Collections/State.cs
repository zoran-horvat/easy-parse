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
            this.Elements = rules.Select(rule => rule.ToProgression().ToStateElement(followSets)).ToImmutableList();
        }

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
