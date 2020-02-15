namespace EasyParse.LexicalAnalysis
{
    public class LineLocation : Location
    {
        public int Offset { get; }
     
        public LineLocation(int offset)
        {
            this.Offset = offset;
        }

        public override string ToString() =>
            $"Pos: {this.Offset + 1}";
    }
}