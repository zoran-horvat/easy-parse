using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.AccessControl;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class StateVector
    {
        public IEnumerable<State> States => this.Representation;

        private ImmutableArray<State> Representation { get; }

        public int Length => this.Representation.Length;

        public StateVector(IEnumerable<Rule> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets)
        {
            this.Representation = new[] { new State(rules, firstSets, followSets) }.ToImmutableArray();
        }

        private StateVector(ImmutableArray<State> states)
        {
            this.Representation = states;
        }

        public StateVector Closure()
        {
            List<int> modifications = Enumerable.Range(0, this.Representation.Length).ToList();
            List<State> states = this.Representation.ToList();

            while (modifications.Any())
            {
                var step = this.Advance(modifications, states);
                modifications = step.modifications;
                states = step.states;
            }

            return new StateVector(states.ToImmutableArray());
        }

        private (List<int> modifications, List<State> states) Advance(List<int> modifications, List<State> states)
        {
            List<State> pendingChanges = modifications
                .Select(index => states[index])
                .SelectMany(state => state.Advance())
                .ToList();

            List<State> finalStates = new List<State>();
            List<int> modifiedIndexes = new List<int>();

            foreach ((State state, int index) tuple in states.Select((state, index) => (state, index)))
            {
                var next = this.Advance(tuple.state, tuple.index, new[] {tuple.state}.Concat(pendingChanges).Union().ToList());

                modifiedIndexes.AddRange(next.modifiedIndex);
                finalStates.Add(next.newCurrent);
                pendingChanges = next.pendingChanges;
            }

            modifiedIndexes.AddRange(Enumerable.Range(finalStates.Count, pendingChanges.Count));
            finalStates.AddRange(pendingChanges);

            return (modifiedIndexes, finalStates);
        }

        private (State newCurrent, IEnumerable<int> modifiedIndex, List<State> pendingChanges) Advance(State current, int index, List<State> changes) =>
            this.Advance(current, changes.First(change => change.Core.Equals(current.Core)), index, changes);

        private (State newCurrent, IEnumerable<int> modifiedIndex, List<State> pendingChanges) Advance(State current, State newCurrent, int index, List<State> changes) =>
            (newCurrent, newCurrent.Equals(current) ? new int[0] : new[] {index}, changes.Where(change => !change.Core.Equals(newCurrent.Core)).ToList());

        public override string ToString() => Formatting.ToString(this);
    }
}
