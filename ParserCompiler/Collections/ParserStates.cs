using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models;

namespace ParserCompiler.Collections
{
    public class ParserStates
    {
        private List<State> Representation { get; }

        public int Length => this.Representation.Count;

        public ParserStates(IEnumerable<Rule> rules)
        {
            this.Representation = new List<State>()
            {
                new State(rules)
            };
        }

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Representation.Select(this.ToString).ToArray());

        private string ToString(State state, int index) =>
            $"S{index}{Environment.NewLine}{state}";
    }
}
