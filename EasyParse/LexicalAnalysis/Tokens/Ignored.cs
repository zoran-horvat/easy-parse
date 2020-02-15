using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public class Ignored : Token
    {
        public string Value { get; }
     
        public Ignored(string value, Location location, Location locationAfter)
            : base(location, locationAfter, value.Length)
        {
            this.Value = value;
        }

        public override string ToString() =>
            $"[Ignored({this.Value})]";
    }
}
