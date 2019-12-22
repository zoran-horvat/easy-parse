namespace EasyParse.Parsing.Nodes
{
    public class Error : TreeElement
    {
        public string Message { get; }

        public Error(string message)
        {
            this.Message = message;
        }

        public override string ToString() =>
            this.Message;
    }
}
