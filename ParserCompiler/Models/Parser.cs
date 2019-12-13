using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Models
{
    public class Parser
    {
        public IEnumerable<Rule> Rules => this.RulesRepresentation;
        private ImmutableList<Rule> RulesRepresentation { get; }
        public Set<FirstSet> FirstSets { get; }
        public Set<FollowSet> FollowSets { get; }
        public ParserStates States { get; }

        public Parser(ImmutableList<Rule> rules, Set<FirstSet> firstSets, Set<FollowSet> followSets, ParserStates states)
        {
            this.RulesRepresentation = rules;
            this.FirstSets = firstSets;
            this.FollowSets = followSets;
            this.States = states;
        }
    }
}
