using EasyParse.Text;

namespace EasyParse.Parsing.Nodes.Errors
{
    public class InternalError : Error
    {
        public InternalError(Location location)
            : base(location, $"Parser internal error at {location}")
        {
        }

        public InternalError(Location location, string details)
            : base(location, $"Parser internal error at {location}: {details}")
        {
        }
    }
}
