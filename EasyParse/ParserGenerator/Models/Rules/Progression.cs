using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class Progression : IEquatable<Progression>
    {
        public RuleDefinition Rule { get; }
        public int Position { get; }
        public int Length => this.Rule.Body.Count();
     
        public Progression(RuleDefinition rule) : this(rule, 0) { }

        private Progression(RuleDefinition rule, int position)
        {
            this.Rule = rule;
            this.Position = position;
        }

        public IEnumerable<Symbol> ConsumedSymbols =>
            this.Rule.Body.Take(this.Position);

        public IEnumerable<Symbol> PendingSymbols =>
            this.Rule.Body.Skip(this.Position);

        public StateElement ToStateElement(Set<FollowSet> followSets) =>
            new StateElement(this, followSets.Find(this.Rule.Head));

        public IEnumerable<(Symbol consumed, Progression rest)> Advance() => 
            this.Position < this.Length ? new[] {this.Consume()}
            : Enumerable.Empty<(Symbol consumed, Progression rest)>();

        private (Symbol consumed, Progression rest) Consume() =>
            (this.Rule.Body.ElementAt(this.Position), new Progression(this.Rule, this.Position + 1));

        public override string ToString() => Formatter.ToString(this);

        public override bool Equals(object obj) => 
            this.Equals(obj as Progression);

        public bool Equals(Progression other) =>
            !(other is null) &&
            (other.Rule.Equals(this.Rule)) &&
            (other.Position.Equals(this.Position));

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Rule.GetHashCode() * 397) ^ this.Position;
            }
        }

        public IEnumerable<Symbol> PeekTwo() =>
            this.Rule.Body.Skip(this.Position).Take(2);
    }
}
