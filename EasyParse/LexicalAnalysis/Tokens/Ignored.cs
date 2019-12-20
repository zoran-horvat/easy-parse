namespace EasyParse.LexicalAnalysis.Tokens
{
    public class Ignored : Token
    {
        public string Value { get; }
     
        public Ignored(string value, int position) : base(position, value.Length)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"[Ignored({this.Value})]";
    }
}
