using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class State
    {
        private ImmutableList<StateElement> Progressions { get; }

        public State(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Progressions = ImmutableList<StateElement>.Empty.AddRange(rules.Select(rule => new StateElement(new Progression(rule), followSets.First(set => set.Key.Equals(rule.Head)).OfType<Terminal>().AsSet())));
        }

        public override string ToString() =>
            string.Join(string.Empty, this.Progressions.Select(line => $"{line}{Environment.NewLine}").ToArray());
    }
}
