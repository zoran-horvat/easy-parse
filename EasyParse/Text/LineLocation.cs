namespace EasyParse.Text
{
    public class LineLocation : InnerLocation
    {
        public LineLocation(int offset) : base(offset)
        {
        }

        public override string ToString() =>
            $"{base.Offset + 1}";
    }
}