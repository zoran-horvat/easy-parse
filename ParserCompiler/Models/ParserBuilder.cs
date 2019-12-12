using System.Collections.Generic;
using System.Collections.Immutable;
using ParserCompiler.Collections;

namespace ParserCompiler.Models
{
    public class ParserBuilder
    {
        private ImmutableList<Rule> Rules { get; }

        private ParserBuilder(IEnumerable<Rule> rules)
        {
            this.Rules = ImmutableList<Rule>.Empty.AddRange(rules);
        }

        public static ParserBuilder For(IEnumerable<Rule> rules) =>
            new ParserBuilder(rules);

        public Parser Build()
        {
            Set<FirstSet> firstSets = FirstSetsBuilder.BuildFor(this.Rules);
            Set<FollowSet> followSets = FollowSetsBuilder.BuildFor(this.Rules, firstSets);
            ParserStates states = new ParserStates(this.Rules);

            return new Parser(this.Rules, firstSets, followSets, states);
        }
    }
}
