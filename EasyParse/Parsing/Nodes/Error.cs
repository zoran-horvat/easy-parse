using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public class Error : TreeElement
    {
        public Location Location { get; }
        public virtual string Message { get; }

        public Error(Location location, string message)
        {
            this.Location = location;
            this.Message = message;
        }

        protected Error()
        {

        }

        public override string ToString() =>
            this.Message;
    }
}
