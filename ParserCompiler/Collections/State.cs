using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class State : IEquatable<State>
    {
        private Set<Rule> Rules { get; }
        private Set<FirstSet> FirstSets { get; }
        public Set<StateElement> Elements { get; }

        public State(IEnumerable<Rule> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets)
        {
            List<Rule> rulesList = new List<Rule>(rules);
            this.Rules = rulesList.AsSet();
            this.FirstSets = firstSets;
            this.Elements = rulesList.Select(rule => rule.ToProgression().ToStateElement(followSets)).AsSet();
        }

        private State(State copy, IEnumerable<StateElement> elements)
        {
            this.Rules = copy.Rules;
            this.FirstSets = copy.FirstSets;
            this.Elements = elements.AsSet();
        }

        public IEnumerable<State> Advance() =>
            this.Elements
                .SelectMany(element => element.Advance())
                .GroupBy(move => move.consumed, move => move.rest)
                .Select(group => new State(this, group))
                .Select(state => state.Closure());

        private State Expand() =>
            this.Elements
                .SelectMany(element => element.PeekNonTerminal(this.FirstSets))
                .SelectMany(tuple => this.Rules.Where(rule => rule.Head.Equals(tuple.next)).Select(rule => (rule: rule, follow: tuple.follow)))
                .Select(tuple => new StateElement(tuple.rule.ToProgression(), tuple.follow))
                .Aggregate(this, (state, element) => state.Append(element));

        private State Append(StateElement element) =>
            new State(this, this.AppendToElements(element));

        private IEnumerable<StateElement> AppendToElements(StateElement element) =>
            this.Elements
                .Select(existing => (rule: existing.Progression, follow: existing.FollowedBy))
                .Concat(new[] {(rule: element.Progression, follow: element.FollowedBy)})
                .GroupBy(row => row.rule, row => row.follow)
                .Select(group => (rule: group.Key, follow: group.Union()))
                .Select(tuple => new StateElement(tuple.rule, tuple.follow));

        private State Closure()
        {
            State result = this;
            while (result.Expand() is State next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        public override string ToString() => Formatting.ToString(this);

        public override bool Equals(object obj) =>
            this.Equals(obj as State);

        public bool Equals(State other) =>
            !(other is null) &&
            other.Elements.Equals(this.Elements);

        public override int GetHashCode() => 
            this.Elements.GetHashCode();
    }
}
