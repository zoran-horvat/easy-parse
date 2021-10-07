using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyParse.Parsing.Rules
{
    public class Productions
    {
        public Productions(NonTerminal nonTerminal) 
            : this(nonTerminal, ImmutableList<Production>.Empty)
        {
        }

        private Productions(NonTerminal head, ImmutableList<Production> lines)
        {
            this.Head = head;
            this.Lines = lines;
        }

        public NonTerminal Head { get; }
        private ImmutableList<Production> Lines { get; }

        public Productions Match(params Symbol[] symbols) =>
            new Productions(this.Head, this.Lines.Add(new Production(this.Head, symbols)));

        internal IEnumerable<Production> Expand() => this.Lines;

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Lines.Select(x => x.ToString()));
    }
}
