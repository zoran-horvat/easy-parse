using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public class NonTerminalNode : Node
    {
        public RuleReference ProducedBy { get; }
        public Node[] Children { get; }

        public NonTerminalNode(Location location, string label, RuleReference producedBy, IEnumerable<Node> children) : base(location, label)
        {
            this.ProducedBy = producedBy;
            this.Children = children.ToArray();
        }

        public override string ToString() =>
            $"[{base.Label} -> {this.ChildrenToString()}] at {base.Location}";

        private string ChildrenToString() =>
            string.Join(string.Empty, this.Children.Select(child => $"{child}").ToArray());
    }
}