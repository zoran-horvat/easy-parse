using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models;

namespace ParserCompiler.Collections
{
    public class State
    {
        private ImmutableList<Rule> Rules { get; }

        public State(IEnumerable<Rule> rules)
        {
            this.Rules = ImmutableList<Rule>.Empty.AddRange(rules);
        }

        public override string ToString() =>
            string.Join(string.Empty, this.Rules.Select(rule => $"{rule}{Environment.NewLine}").ToArray());
    }
}
