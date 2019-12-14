using ParserCompiler.Collections;

namespace ParserCompiler.Models.Rules
{
    public class Progression
    {
        public Rule Rule { get; }
        public int Position { get; }
     
        public Progression(Rule rule)
        {
            this.Rule = rule;
            this.Position = 0;
        }

        public StateElement ToStateElement(Set<FollowSet> followSets) =>
            new StateElement(this, followSets.Find(this.Rule.Head));

        public override string ToString() => Formatting.ToString(this);
    }
}
