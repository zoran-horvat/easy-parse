using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Parsing.Nodes
{
    public class NonTerminalNode : Node
    {
        public TreeElement[] Children { get; }

        public NonTerminalNode(string label, IEnumerable<TreeElement> children) : base(label)
        {
            this.Children = children.ToArray();
        }
    }
}