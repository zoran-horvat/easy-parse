using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class Rule : IEmptyRule
    {
        public Rule(NonTerminal nonTerminal) 
            : this(nonTerminal, ImmutableList<Production>.Empty)
        {
        }

        private Rule(NonTerminal head, ImmutableList<Production> lines)
        {
            this.Head = head;
            this.Lines = lines;
        }

        public NonTerminal Head { get; }
        internal IEnumerable<Production> ProductionLines => Lines;
        private ImmutableList<Production> Lines { get; }

        public Rule Match(params Symbol[] symbols) =>
            this.Or(symbols);

        public Rule Or(params Symbol[] symbols) =>
            new Rule(this.Head, this.Lines.Add(new Production(this.Head, symbols)));

        internal IEnumerable<Production> Expand()
        {
            HashSet<NonTerminal> produced = this.Lines.Select(line => line.Head).ToHashSet();
            Queue<Production> pending = this.Lines.ToQueue();

            while (pending.Count > 0)
            {
                Production production = pending.Dequeue();
                IEnumerable<Production> children = production.Body
                    .OfType<NonTerminalSymbol>()
                    .Where(symbol => !produced.Contains(symbol.Rule.Head))
                    .SelectMany(symbol => symbol.Rule.Lines);
                foreach (Production child in children)
                {
                    if (!produced.Contains(child.Head)) produced.Add(child.Head);
                    pending.Enqueue(child);
                }
                yield return production;
            }
        }

        public static implicit operator Symbol(Rule rule) =>
            new NonTerminalSymbol(rule);

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Lines.Select(x => x.ToString()));
    }
}
