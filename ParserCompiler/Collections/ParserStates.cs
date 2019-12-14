using System.Collections.Generic;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class ParserStates
    {
        public IEnumerable<State> States => this.Representation;

        private List<State> Representation { get; }

        public int Length => this.Representation.Count;

        public ParserStates(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Representation = new List<State>()
            {
                new State(rules, followSets)
            };
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
