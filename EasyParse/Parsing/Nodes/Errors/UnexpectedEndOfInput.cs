using EasyParse.Text;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class UnexpectedEndOfInput : Error
    {
        public UnexpectedEndOfInput(Location location)
            : base(location, $"Unexpected end of input at {location}")
        {
        }
    }
}
