using EasyParse.LexicalAnalysis.Tokens;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class SyntaxError : Error
    {
        public SyntaxError(Lexeme element)
            : base(element.Location, $"Unexpected element at {element.Location}: {Printable(element.Value)}")
        {
        }
    }
}
