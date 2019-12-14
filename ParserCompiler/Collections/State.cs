using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Collections
{
    public class State
    {
        public ImmutableList<StateElement> Elements { get; }

        public State(IEnumerable<Rule> rules, Set<FollowSet> followSets)
        {
            this.Elements = rules.Select(rule => rule.ToProgression().ToStateElement(followSets)).ToImmutableList();
        }

        public override string ToString() => Formatting.ToString(this);
    }
}
