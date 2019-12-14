using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class State
    {
        public ImmutableList<StateElement> Elements { get; }

        public State(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Elements = rules.Select(rule => rule.ToProgression().ToStateElement(followSets)).ToImmutableList();
        }

        private State(IEnumerable<StateElement> elements)
        {
            this.Elements = elements.ToImmutableList();
        }

        public IEnumerable<State> Advance()
        {
            return this.Elements
                .SelectMany(element => element.Advance())
                .GroupBy(move => move.consumed, move => move.rest)
                .Select(group => new State(group));
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
