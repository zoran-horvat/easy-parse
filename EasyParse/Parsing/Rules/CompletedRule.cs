using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class CompletedRule : IRule
    {
        internal CompletedRule(NonTerminal head, Type type, ImmutableList<Production> lines)
        {
            this.Head = head;
            this.Type = type;
            this.Lines = lines;
        }

        public NonTerminal Head { get; }
        public Type Type { get; }
        public IEnumerable<Production> Productions => Lines;
        private ImmutableList<Production> Lines { get; }

        public IEnumerable<Production> Expand()
        {
            HashSet<NonTerminal> produced = this.Lines.Select(line => line.Head).ToHashSet();
            Queue<Production> pending = this.Lines.ToQueue();

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

        public IPendingMapping Literal(string value) =>
            this.BeginLine().Literal(value);

        public IPendingMapping Regex(string name, string pattern) =>
            this.BeginLine().Regex(name, pattern);

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.BeginLine().Symbol(factory);

        private IPendingMapping BeginLine() =>
            new IncompleteProductionBuilder(this.Lines, this.Head);
    }
}
