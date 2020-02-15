using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public Location Location { get; }
        public Location LocationAfter { get; }
        public int Length { get; }

        protected Token(Location location, Location locationAfter, int length)
        {
            this.Location = location;
            this.LocationAfter = locationAfter;
            this.Length = length;
        }
    }
}