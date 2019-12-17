using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Models
{
    public class Parser
    {
        public Grammar Grammar { get; }
        public Set<FirstSet> FirstSets { get; }
        public Set<FollowSet> FollowSets { get; }
        public StateVector States { get; }
        public ParsingTable Table { get; }

        public Parser(Grammar grammar, Set<FirstSet> firstSets, Set<FollowSet> followSets, StateVector states, ParsingTable table)
        {
            this.Grammar = grammar;
            this.FirstSets = firstSets;
            this.FollowSets = followSets;
            this.States = states;
            this.Table = table;
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
