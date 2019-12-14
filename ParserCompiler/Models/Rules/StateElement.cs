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

        public override string ToString() => Formatting.ToString(this);
    }
}
