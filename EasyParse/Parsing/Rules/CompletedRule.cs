using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class CompletedRule : IRule
    {
        internal CompletedRule(NonTerminalName head, ImmutableList<Production> lines)
        {
            this.Head = head;
            this.Lines = lines;
        }

        private IRule RuleFactory() => this;

        public NonTerminalName Head { get; }
        public IEnumerable<Production> Productions => this.GetProductions(this.Type);
        private ImmutableList<Production> Lines { get; }

        public Type Type =>
            this.Lines
                .Select(line => (type: line.Transform.ReturnType, production: line))
                .Aggregate((a, b) =>
                    a.type.IsAssignableFrom(b.type) ? a
                    : b.type.IsAssignableFrom(a.type) ? b
                    : throw new ArgumentException(
                        $"Could not decide common return type for {a.type.Name} and {b.type.Name} " +
                        $"in rule {b.production}"))
                .type;

        private IEnumerable<Production> GetProductions(Type returnType) =>
            this.Lines.Select(line => line.WithReturnType(returnType));

        public IEnumerable<Production> Expand()
        {
            HashSet<NonTerminalName> produced = new HashSet<NonTerminalName>() { this.Head };
            Queue<Production> pending = this.Productions.ToQueue();

            while (pending.Count > 0)
            {
                Production production = pending.Dequeue();
                IEnumerable<Production> children = production.ChildLines(produced).ToList();
                produced.Add(children.Select(child => child.Head).Distinct());
                pending.Enqueue(children);
                yield return production;
            }
        }

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Lines.Select(x => x.ToString()));

        public IPendingMapping Match(Symbol first, params Symbol[] others) =>
            new IncompleteProductionBuilder(this.Lines, this.Head).Match(first, others);
    }
}
