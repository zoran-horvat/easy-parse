using EasyParse.Text;

namespace EasyParse.LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public Location Location { get; }
        public int Length { get; }

        protected Token(Location location, int length)
        {
            this.Location = location;
            this.Length = length;
        }
    }
}