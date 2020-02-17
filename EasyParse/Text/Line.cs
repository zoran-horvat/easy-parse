namespace EasyParse.Text
{
    internal class Line : Plaintext
    {
        private int Length { get; }
        public Line(string content) : base(content)
        {
            this.Length = content.Length;
        }
        protected override Location LocationFor(int offset) =>
            offset >= this.Length ? EndOfText.Value
            : new LineLocation(offset);
    }
}