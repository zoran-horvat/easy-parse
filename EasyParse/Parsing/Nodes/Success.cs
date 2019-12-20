namespace EasyParse.Parsing.Nodes
{
    public abstract class Success : Node
    {
        public string Value { get; }
     
        protected Success(string value)
        {
            this.Value = value;
        }
    }
}
