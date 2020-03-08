using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public class TerminalNode : Node
    {
        public string Value { get; }
     
        public TerminalNode(Location location, string label, string value) : base(location, label)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"{this.Value} at {base.Location}";
    }
}