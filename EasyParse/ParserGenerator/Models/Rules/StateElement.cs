using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class StateElement : IEquatable<StateElement>
    {
        public Progression Progression { get; }
        public Set<Terminal> FollowedBy { get; }

        public StateElement(Progression progression, Set<Terminal> followedBy)
        {
            this.Progression = progression;
            this.FollowedBy = followedBy;
        }

        public IEnumerable<(Symbol consumed, StateElement rest)> Advance() =>
            this.Progression.Advance()
                .Select(tuple => (tuple.consumed, new StateElement(tuple.rest, this.FollowedBy)));

        public IEnumerable<(NonTerminal next, Set<Terminal> follow)> PeekNonTerminal(Set<FirstSet> firstSets) =>
            this.Peek(this.Progression.PeekTwo().ToArray(), firstSets);

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(Symbol[] upcoming, Set<FirstSet> firstSets) =>
            upcoming.Length > 0 && upcoming[0] is NonTerminal first ? this.Peek(first, upcoming.Skip(1).ToArray(), firstSets)
            : Enumerable.Empty<(NonTerminal, Set<Terminal>)>();

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first, Symbol[] next, Set<FirstSet> firstSets) =>
            next.Length == 0 ? this.Peek(first) 
            : next[0] is Terminal terminal ? this.Peek(first, terminal)
            : next[0] is NonTerminal nonTerminal ? this.Peek(first, nonTerminal, firstSets)
            : Enumerable.Empty<(NonTerminal, Set<Terminal>)>();

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first) => 
            new[] {(upcoming: first, this.FollowedBy)};

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first, Terminal follow) =>
            new[] {(upcoming: first, new[] {follow}.AsSet())};

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first, NonTerminal follow, Set<FirstSet> firstSets) =>
            new[] {(upcodming: first, firstSets.Find(follow))};

        public IEnumerable<(Rule reduce, Set<Terminal> terminals)> Reductions =>
            this.Progression.Position < this.Progression.Length ? Enumerable.Empty<(Rule, Set<Terminal>)>()
            : new[] {(this.Progression.Rule, this.FollowedBy)};

        public override string ToString() => Formatter.ToString(this);

        public override bool Equals(object obj) =>
            this.Equals(obj as StateElement);

        public bool Equals(StateElement other) =>
            !(other is null) &&
            other.Progression.Equals(this.Progression) &&
            other.FollowedBy.Equals(this.FollowedBy);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Progression.GetHashCode() * 397) ^ FollowedBy.GetHashCode();
            }
        }
    }
}
