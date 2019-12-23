namespace EasyParse.Parsing.Nodes
{
    public abstract class Node : TreeElement
    {
        public string Label { get; }
     
        protected Node(string label)
        {
            this.Label = label;
        }
    }
}
