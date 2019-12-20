using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Nodes
{
    public class NonTerminalNode : Success
    {
        public Node[] Children { get; }

        public NonTerminalNode(string label, IEnumerable<Node> children) : base(label)
        {
            this.Children = children.ToArray();
        }
    }
}