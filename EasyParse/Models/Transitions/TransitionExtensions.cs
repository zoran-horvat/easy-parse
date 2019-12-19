using System.Collections.Generic;
using System.Linq;
using EasyParse.Collections;
using EasyParse.Models.Rules;

namespace EasyParse.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static ParsingTable ToParsingTable(this List<CoreTransition> transitions, List<State> states, List<Rule> rules) =>
            transitions.Aggregate(new ParsingTable(states, rules), (table, transition) => table.Add(transition));
    }
}