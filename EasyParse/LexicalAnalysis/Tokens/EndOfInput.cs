namespace EasyParse.LexicalAnalysis.Tokens
{
    public class EndOfInput : Lexeme
    {
        public EndOfInput(int position) : base("$", position, "$")
        {
        }

        public override string ToString() =>
            "[End of input]";
    }
}
