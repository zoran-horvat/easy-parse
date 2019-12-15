using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
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
    }
}