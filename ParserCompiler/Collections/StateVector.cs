using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

            StateVector result = new StateVector(states.ToImmutableArray());
            return result;
        }

        private (List<int> modifications, List<State> states) Advance(List<int> modifications, List<State> states)
        {
            List<State> pendingChanges = modifications
                .Select(index => states[index])
                .SelectMany(state => state.Advance())
                .ToList();

            List<State> finalStates = new List<State>();
            List<int> modifiedIndexes = new List<int>();

            foreach (State currentState in states)
            {
                List<State> nextModifiedStates = new List<State>();
                foreach (State modifiedState in new[] {currentState}.Concat(pendingChanges).Union())
                {
                    if (modifiedState.Core.Equals(currentState.Core))
                    {
                        if (!modifiedState.Equals(currentState))
                            modifiedIndexes.Add(finalStates.Count);
                        finalStates.Add(modifiedState);
                    }
                    else
                    {
                        nextModifiedStates.Add(modifiedState);
                    }
                }

                pendingChanges = nextModifiedStates;
            }

            foreach (State modifiedState in pendingChanges)
            {
                modifiedIndexes.Add(finalStates.Count);
                finalStates.Add(modifiedState);
            }

            return (modifiedIndexes, finalStates);
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
