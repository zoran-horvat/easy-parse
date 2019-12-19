using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    static class StateExtensions
    {
        public static IEnumerable<StateElement> Union(this IEnumerable<StateElement> elements) =>
            elements
                .GroupBy(element => element.Progression, element => element.FollowedBy)
                .Select(group => (rule: group.Key, follow: group.Union()))
                .Select(tuple => new StateElement(tuple.rule, tuple.follow));

        public static IEnumerable<StateElement> Union(this IEnumerable<StateElement> elements, params StateElement[] append) =>
            elements.Concat(append).Union();

        public static IEnumerable<State> Union(this IEnumerable<State> states) =>
            states
                .GroupBy(state => state.Core)
                .Select(tuple => tuple.Aggregate((acc, cur) => acc.Union(cur)));

        public static IEnumerable<CoreReduction> Reductions(this IEnumerable<State> states) =>
            states.SelectMany(Reductions);

        private static IEnumerable<CoreReduction> Reductions(State state) =>
            state.Reductions.SelectMany(tuple => Reductions(tuple.core, tuple.reduce, tuple.terminals));

        private static IEnumerable<CoreReduction> Reductions(Core core, Rule reduce, Set<Terminal> terminals) =>
            terminals.Select(terminal => new CoreReduction(core, terminal, reduce));
    }
}