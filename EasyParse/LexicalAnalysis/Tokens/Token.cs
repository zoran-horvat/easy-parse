namespace EasyParse.LexicalAnalysis.Tokens
{
    public abstract class Token
    {
        public int Position { get; }
        public int PositionAfter { get; }

        protected Token(int position, int length)
        {
            this.Position = position;
            this.PositionAfter = position + length;
        }
    }
}