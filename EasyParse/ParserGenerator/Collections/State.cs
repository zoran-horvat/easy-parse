using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class State : IEquatable<State>
    {
        private Set<RuleDefinition> Rules { get; }
        private Set<FirstSet> FirstSets { get; }
        public Set<StateElement> Elements { get; }

        public Core Core { get; }

        public State(IEnumerable<RuleDefinition> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets)
        {
            List<RuleDefinition> rulesList = new List<RuleDefinition>(rules);
            this.Rules = rulesList.AsSet();
            this.FirstSets = firstSets;
            this.Elements = rulesList.Select(rule => rule.ToProgression().ToStateElement(followSets)).AsSet();
            this.Core = new Core(this.Elements.Select(element => element.Progression));
        }

        private State(State copy, IEnumerable<StateElement> elements)
        {
            this.Rules = copy.Rules;
            this.FirstSets = copy.FirstSets;
            this.Elements = elements.AsSet();
            this.Core = new Core(this.Elements.Select(element => element.Progression));
        }

        public IEnumerable<StateTransition> Advance() =>
            this.Elements
                .SelectMany(element => element.Advance())
                .GroupBy(move => move.consumed, move => move.rest)
                .Select(group => new StateTransition(this, group.Key, new State(this, group)))
                .Select(transition => transition.Closure());

        private State Expand() =>
            this.Elements
                .SelectMany(element => element.PeekNonTerminal(this.FirstSets))
                .SelectMany(tuple => this.Rules.Where(rule => rule.Head.Equals(tuple.next)).Select(rule => (rule: rule, follow: tuple.follow)))
                .Select(tuple => new StateElement(tuple.rule.ToProgression(), tuple.follow))
                .Aggregate(this, (state, element) => state.Append(element));

        private State Append(StateElement element) =>
            new State(this, this.Elements.Union(element));

        public State Union(State other) =>
            new State(this, this.Elements.Union(other.Elements));

        public State Closure()
        {
            State result = this;
            while (result.Expand() is State next && !next.Equals(result))
            {
                result = next;
            }
            return result;
        }

        public IEnumerable<(Core core, RuleDefinition reduce, Set<Terminal> terminals)> Reductions =>
            this.Elements.SelectMany(element => element.Reductions).Select(tuple => (this.Core, tuple.reduce, tuple.terminals));

        public override string ToString() => Formatter.ToString(this);

        public override bool Equals(object obj) =>
            this.Equals(obj as State);

        public bool Equals(State other) =>
            !(other is null) &&
            other.Elements.Equals(this.Elements);

        public override int GetHashCode() => 
            this.Elements.GetHashCode();
    }
}
