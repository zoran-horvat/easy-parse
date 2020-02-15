using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public class EndOfInput : Lexeme
    {
        public EndOfInput(Location location) 
            : base("$", location, EndOfText.Value, "$")
        {
        }

        public override string ToString() =>
            "[End of input]";
    }
}
