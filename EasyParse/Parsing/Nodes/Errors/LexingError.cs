using EasyParse.Text;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class LexingError : Error
    {
        public LexingError(Location location, string unexpectedInput)
            : base(location, $"Unexpected input: {unexpectedInput} at {location}")
        {
        }
    }
}
