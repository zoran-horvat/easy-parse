using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public abstract class Node : TreeElement
    {
        public Location Location { get; }
        public string Label { get; }
     
        protected Node(Location location, string label)
        {
            this.Location = location;
            this.Label = label;
        }
    }
}
