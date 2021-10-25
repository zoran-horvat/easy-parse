using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Fluent.Rules
{
    internal class RulePlaceholder : IRule
    {
        public RulePlaceholder(NonTerminalName head, Type type)
        {
            this.Head = head;
            this.Type = type;
        }
        public NonTerminalName Head { get; }

        public Type Type { get; }

        public IEnumerable<Production> Productions => Enumerable.Empty<Production>();

        public IEnumerable<Production> Expand() => this.Productions;

        public IPendingMapping Match(Symbol first, params Symbol[] others) =>
            new IncompleteProductionBuilder(ImmutableList<Production>.Empty, this.Head).Match(first, others);
    }
}
