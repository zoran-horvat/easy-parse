namespace EasyParse.Parsing.Nodes
{
    public abstract class Node : TreeElement
    {
        public string Value { get; }
     
        protected Node(string value)
        {
            this.Value = value;
        }
    }
}
