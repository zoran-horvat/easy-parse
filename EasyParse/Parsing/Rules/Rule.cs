using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class Rule
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
            new Rule(this.Head, this.Lines.Add(new Production(this.Head, symbols)));

        internal IEnumerable<Production> Expand()
        {
            HashSet<NonTerminal> produced = new();
            Queue<Production> pending = this.Lines.ToQueue();

            while (pending.Count > 0)
            {
                Production production = pending.Dequeue();
                if (produced.Contains(production.Head)) continue;
                pending.Enqueue(production.ChildLines);
                produced.Add(production.Head);
                yield return production;
            }
        }

        public static implicit operator Symbol(Rule rule) =>
            new NonTerminalSymbol(rule);

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Lines.Select(x => x.ToString()));
    }
}
