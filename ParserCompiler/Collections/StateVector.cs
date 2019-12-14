using System.Collections.Generic;
using System.Collections.Immutable;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class StateVector
    {
        public IEnumerable<State> States => this.Representation;

        private ImmutableArray<State> Representation { get; }

        public int Length => this.Representation.Length;

        public StateVector(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Representation = new[] { new State(rules, followSets) }.ToImmutableArray();
        }

        private StateVector(ImmutableArray<State> states)
        {
            this.Representation = states;
        }

        public StateVector Closure()
        {
            return new StateVector(this.Representation.AddRange(this.Representation[0].Advance()));
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
