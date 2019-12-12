using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models;

namespace ParserCompiler.Collections
{
    public class State
    {
        private ImmutableList<Progression> Progressions { get; }

        public State(IEnumerable<Rule> rules)
        {
            this.Progressions = ImmutableList<Progression>.Empty.AddRange(rules.Select(rule => new Progression(rule)));
        }

        public override string ToString() =>
            string.Join(string.Empty, this.Progressions.Select(line => $"{line}{Environment.NewLine}").ToArray());
    }
}
