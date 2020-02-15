namespace EasyParse.Text
{
    public class LineLocation : InnerLocation
    {
        public LineLocation(int offset) : base(offset)
        {
        }

        public override string ToString() =>
            $"Pos: {base.Offset + 1}";
    }
}