﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class StateVector
    {
        private List<RuleDefinition> Rules { get; }
        public IEnumerable<State> States => this.Representation;

        private ImmutableArray<State> Representation { get; }

        public int Length => this.Representation.Length;

        public StateVector(IEnumerable<RuleDefinition> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets) : 
            this(rules, new[] { new State(rules, firstSets, followSets) }.ToImmutableArray()) 
        {
        }

        private StateVector(IEnumerable<RuleDefinition> rules, ImmutableArray<State> states)
        {
            this.Rules = rules.ToList();
            this.Representation = states;
        }

        public (StateVector vector, ParsingTable table) Closure()
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

            return (new StateVector(this.Rules, states.ToImmutableArray()), transitions.ToParsingTable(states, this.Rules).AddRange(states.Reductions()));
        }

        private (List<int> modifications, List<State> states, List<CoreTransition> transitions) Advance(List<int> modifications, List<State> states)
        {
            List<StateTransition> pendingTransitions = modifications
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

        public override string ToString() => Formatter.ToString(this);
    }
}
