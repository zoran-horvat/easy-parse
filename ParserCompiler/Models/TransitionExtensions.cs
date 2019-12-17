using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public static class TransitionExtensions
    {
        public static (ImmutableList<IndexTransition<Terminal>> shifts, ImmutableList<IndexTransition<NonTerminal>> gotos) ToIndexTransitions(
            this List<CoreTransition> transitions, List<State> states) =>
            transitions.ToIndexTransitions(states.Select((state, index) => (state, index)).ToDictionary(tuple => tuple.state.Core, tuple => tuple.index));

        private static (ImmutableList<IndexTransition<Terminal>> shifts, ImmutableList<IndexTransition<NonTerminal>> gotos) ToIndexTransitions(
            this List<CoreTransition> transitions, IDictionary<Set<Progression>, int> coreToIndex) =>
            transitions.Select(transition => (shifts: transition.ToIndexTransition<Terminal>(coreToIndex), gotos: transition.ToIndexTransition<NonTerminal>(coreToIndex)))
                .Aggregate(
                    (shifts: ImmutableList<IndexTransition<Terminal>>.Empty, gotos: ImmutableList<IndexTransition<NonTerminal>>.Empty),
                    (acc, tuple) => (acc.shifts.AddRange(tuple.shifts), acc.gotos.AddRange(tuple.gotos)));
    }
}