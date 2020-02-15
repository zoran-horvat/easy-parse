using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public class EndOfInput : Lexeme
    {
        public EndOfInput(Location location) : base("$", location, "$")
        {
        }

        public override string ToString() =>
            "[End of input]";
    }
}
