using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static (ShiftTable shift, GotoTable @goto) ToIndexTransitions(this List<CoreTransition> transitions, List<State> states) =>
            transitions.ToIndexTransitions(states.Select((state, index) => (state, index)).ToDictionary(tuple => tuple.state.Core, tuple => tuple.index));

        private static (ShiftTable shift, GotoTable @goto) ToIndexTransitions(
            this List<CoreTransition> transitions, IDictionary<Set<Progression>, int> coreToIndex) =>
            transitions
                .Aggregate(
                    (shift: new ShiftTable(), @goto: new GotoTable()),
                    (acc, transition) => (acc.shift.TryAdd(transition, coreToIndex), acc.@goto.TryAdd(transition, coreToIndex)));
    }
}