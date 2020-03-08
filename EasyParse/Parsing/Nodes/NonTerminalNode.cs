using System.Collections.Generic;
using System.Linq;
using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public class NonTerminalNode : Node
    {
        public Node[] Children { get; }

        public NonTerminalNode(Location location, string label, IEnumerable<Node> children) : base(location, label)
        {
            this.Children = children.ToArray();
        }

        public override string ToString() =>
            $"[{base.Label} -> {this.ChildrenToString()}] at {base.Location}";

        private string ChildrenToString() =>
            string.Join(string.Empty, this.Children.Select(child => $"{child}").ToArray());
    }
}