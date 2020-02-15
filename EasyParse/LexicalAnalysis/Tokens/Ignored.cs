namespace EasyParse.LexicalAnalysis.Tokens
{
    public class Ignored : Token
    {
        public string Value { get; }
     
        public Ignored(string value, Location location) : base(location, value.Length)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"[Ignored({this.Value})]";
    }
}
