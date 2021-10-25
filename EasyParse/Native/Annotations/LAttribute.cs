namespace EasyParse.Native.Annotations
{
    public class LAttribute : LiteralAttribute
    {
        public LAttribute(string value, params string[] otherValues) : base(value, otherValues)
        {
        }
    }
}