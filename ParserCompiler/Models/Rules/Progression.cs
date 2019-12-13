using System.Linq;

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

        public override string ToString() =>
            $"{this.Rule.Head} -> {this.BodyToString()}";

        private string BodyToString() =>
            string.Join(string.Empty, this.Rule.Body.Select((item, index) => index == this.Position ? $"∘{item}" : $"{item}").ToArray());
    }
}
