using System.Linq;
using ParserCompiler.Collections;

namespace ParserCompiler.Models.Rules
{
    public class Progression
    {
        public Rule Rule { get; }
        private int Position { get; }
     
        public Progression(Rule rule)
        {
            this.Rule = rule;
            this.Position = 0;
        }

        public StateElement ToStateElement(Set<FollowSet> followSets) =>
            new StateElement(this, followSets.Find(this.Rule.Head));

        public override string ToString() =>
            $"{this.Rule.Head} -> {this.BodyToString()}";

        private string BodyToString() =>
            string.Join(string.Empty, this.Rule.Body.Select((item, index) => index == this.Position ? $"∘{item}" : $"{item}").ToArray());
    }
}
