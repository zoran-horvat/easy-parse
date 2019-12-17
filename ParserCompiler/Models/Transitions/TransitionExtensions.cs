using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static ParsingTable ToParsingTable(this List<CoreTransition> transitions, List<State> states, List<Rule> rules) =>
            transitions.Aggregate(new ParsingTable(states, rules), (table, transition) => table.Add(transition));
    }
}