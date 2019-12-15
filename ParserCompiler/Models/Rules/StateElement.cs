using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Rules
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

        public IEnumerable<(NonTerminal next, Set<Terminal> follow)> PeekNonTerminal() =>
            this.Peek(this.Progression.PeekTwo().ToArray());

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(Symbol[] upcoming) =>
            upcoming.Length > 0 && upcoming[0] is NonTerminal first ? this.Peek(first, upcoming.Skip(1).ToArray())
            : Enumerable.Empty<(NonTerminal, Set<Terminal>)>();

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first, Symbol[] next) =>
            next.Length == 0 ? this.Peek(first) 
            : next[0] is Terminal terminal ? this.Peek(first, terminal)
            : Enumerable.Empty<(NonTerminal, Set<Terminal>)>();

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first) => 
            new[] {(upcoming: first, this.FollowedBy)};

        private IEnumerable<(NonTerminal next, Set<Terminal> follow)> Peek(NonTerminal first, Terminal follow) =>
            new[] {(upcoming: first, new[] {follow}.AsSet())};

        public override string ToString() => Formatting.ToString(this);

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
