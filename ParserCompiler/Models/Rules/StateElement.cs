using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Rules
{
    public class StateElement
    {
        public Progression Progression { get; }
        public Set<Terminal> FollowedBy { get; }

        public StateElement(Progression progression, Set<Terminal> followedBy)
        {
            this.Progression = progression;
            this.FollowedBy = followedBy;
        }

        public IEnumerable<(Symbol consumed, StateElement rest)> Advance() =>
            this.Progression.Advance()
                .Select(tuple => (tuple.consumed, new StateElement(tuple.rest, this.FollowedBy)));

        public override string ToString() => Formatting.ToString(this);
    }
}
