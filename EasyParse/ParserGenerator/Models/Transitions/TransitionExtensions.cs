using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public static class TransitionExtensions
    {
        public static ParsingTable ToParsingTable(this List<CoreTransition> transitions, List<State> states, List<RuleDefinition> rules) =>
            transitions.Aggregate(new ParsingTable(states, rules), (table, transition) => table.Add(transition));
    }
}