namespace EasyParse.Parsing.Nodes
{
    public class TerminalNode : Node
    {
        public string Value { get; }
     
        public TerminalNode(string label, string value) : base(label)
        {
            this.Value = value;
        }

        public override string ToString() =>
            this.Value;
    }
}