namespace EasyParse.LexicalAnalysis.Tokens
{
    public class InvalidInput : Token
    {
        public string Value { get; }

        public InvalidInput(int position, string value) : base(position, value.Length)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"[Invalid input({this.Value})]";
    }
}
