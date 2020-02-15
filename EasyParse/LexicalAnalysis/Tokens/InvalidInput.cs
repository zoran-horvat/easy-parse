using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public class InvalidInput : Token
    {
        public string Value { get; }

        public InvalidInput(Location location, string value) : base(location, value.Length)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"[Invalid input({this.Value})]";
    }
}
