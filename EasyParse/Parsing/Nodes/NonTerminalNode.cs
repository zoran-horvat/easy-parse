using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Nodes
{
    public class NonTerminalNode : Node
    {
        public Node[] Children { get; }

        public NonTerminalNode(string label, IEnumerable<Node> children) : base(label)
        {
            this.Children = children.ToArray();
        }

        public override string ToString() =>
            $"[{base.Value} -> {this.ChildrenToString()}]";

        private string ChildrenToString() =>
            string.Join(string.Empty, this.Children.Select(child => $"{child}").ToArray());
    }
}