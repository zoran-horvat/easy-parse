using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static (ImmutableList<ShiftCommand> shifts, ImmutableList<GotoCommand> gotos) ToIndexTransitions(
            this List<CoreTransition> transitions, List<State> states) =>
            transitions.ToIndexTransitions(states.Select((state, index) => (state, index)).ToDictionary(tuple => tuple.state.Core, tuple => tuple.index));

        private static (ImmutableList<ShiftCommand> shifts, ImmutableList<GotoCommand> gotos) ToIndexTransitions(
            this List<CoreTransition> transitions, IDictionary<Set<Progression>, int> coreToIndex) =>
            transitions
                .Aggregate(
                    (shifts: ImmutableList<ShiftCommand>.Empty, gotos: ImmutableList<GotoCommand>.Empty),
                    (acc, transition) => (ShiftCommand.Add(transition, coreToIndex, acc.shifts), GotoCommand.Add(transition, coreToIndex, acc.gotos)));
    }
}