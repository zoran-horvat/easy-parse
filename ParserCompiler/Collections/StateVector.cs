using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class StateVector
    {
        public IEnumerable<State> States => this.Representation;

        public IEnumerable<ShiftCommand> ShiftCommands => this.ShiftRepresentation;
        public IEnumerable<GotoCommand> GotoCommands => this.GotoRepresentation;

        private ImmutableArray<State> Representation { get; }

        public int Length => this.Representation.Length;

        private ImmutableList<ShiftCommand> ShiftRepresentation { get; }
        private ImmutableList<GotoCommand> GotoRepresentation { get; }

        public StateVector(IEnumerable<Rule> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets)
        {
            this.Representation = new[] { new State(rules, firstSets, followSets) }.ToImmutableArray();
            this.ShiftRepresentation = ImmutableList<ShiftCommand>.Empty;
            this.GotoRepresentation = ImmutableList<GotoCommand>.Empty;
        }

        private StateVector(ImmutableArray<State> states, ImmutableList<ShiftCommand> shiftRepresentation, ImmutableList<GotoCommand> gotoRepresentation)
        {
            this.Representation = states;
            this.ShiftRepresentation = shiftRepresentation;
            this.GotoRepresentation = gotoRepresentation;
        }

        public StateVector Closure()
        {
            List<int> modifications = Enumerable.Range(0, this.Representation.Length).ToList();
            List<State> states = this.Representation.ToList();
            List<CoreTransition> transitions = new List<CoreTransition>();

            while (modifications.Any())
            {
                var step = this.Advance(modifications, states);
                modifications = step.modifications;
                states = step.states;
                transitions.AddRange(step.transitions);
            }

            (ImmutableList<ShiftCommand> shifts, ImmutableList<GotoCommand> gotos) = transitions.ToIndexTransitions(states);

            return new StateVector(states.ToImmutableArray(), shifts, gotos);
        }

        private (List<int> modifications, List<State> states, List<CoreTransition> transitions) Advance(List<int> modifications, List<State> states)
        {
            List<Transition> pendingTransitions = modifications
                .Select(index => states[index])
                .SelectMany(state => state.Advance())
                .ToList();
            List<State> pendingChanges = pendingTransitions.Select(transition => transition.To).Union().ToList();

            List<State> finalStates = new List<State>();
            List<int> modifiedIndexes = new List<int>();
            List<CoreTransition> transitions = pendingTransitions.Select(transition => transition.ToCore()).ToList();

            foreach ((State state, int index) tuple in states.Select((state, index) => (state, index)))
            {
                var next = this.Advance(tuple.state, tuple.index, new[] {tuple.state}.Concat(pendingChanges).Union().ToList());

                modifiedIndexes.AddRange(next.modifiedIndex);
                finalStates.Add(next.newCurrent);
                pendingChanges = next.pendingChanges;
            }

            modifiedIndexes.AddRange(Enumerable.Range(finalStates.Count, pendingChanges.Count));
            finalStates.AddRange(pendingChanges);

            return (modifiedIndexes, finalStates, transitions);
        }

        private (State newCurrent, IEnumerable<int> modifiedIndex, List<State> pendingChanges) Advance(State current, int index, List<State> changes) =>
            this.Advance(current, changes.First(transition=> transition.Core.Equals(current.Core)), index, changes);

        private (State newCurrent, IEnumerable<int> modifiedIndex, List<State> pendingChanges) Advance(State current, State newCurrent, int index, List<State> changes) =>
            (newCurrent, newCurrent.Equals(current) ? new int[0] : new[] {index}, changes.Where(change => !change.Core.Equals(newCurrent.Core)).ToList());

        public override string ToString() => Formatting.ToString(this);
    }
}
