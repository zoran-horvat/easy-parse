using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;

namespace ParserCompiler.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static ParsingTable ToParsingTable(this List<CoreTransition> transitions, List<State> states) =>
            transitions.Aggregate(new ParsingTable(states), (table, transition) => table.Add(transition));
    }
}