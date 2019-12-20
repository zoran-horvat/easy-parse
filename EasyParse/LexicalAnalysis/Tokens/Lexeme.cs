namespace EasyParse.LexicalAnalysis.Tokens
{
    public class Lexeme : Token
    {
        public string Label { get; }
        public string Value { get; }

        public Lexeme(string label, int position, string value) : base(position, value.Length)
        {
            this.Label = label;
            this.Value = value;
        }

        public override string ToString() =>
            $"[{this.Label}({this.Value})]";
    }
}
