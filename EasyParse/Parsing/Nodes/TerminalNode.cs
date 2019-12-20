namespace EasyParse.Parsing.Nodes
{
    public class TerminalNode : Success
    {
        public string Value { get; }
     
        public TerminalNode(string label, string value) : base(label)
        {
            this.Value = value;
        }
    }
}