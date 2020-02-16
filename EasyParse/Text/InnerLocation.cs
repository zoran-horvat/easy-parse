using System;

namespace EasyParse.Text
{
    public abstract class InnerLocation : Location
    {
        public int Offset { get; }
     
        protected InnerLocation(int offset)
        {
            this.Offset = offset;
        }

        public override int CompareTo(Location other) =>
            other is EndOfText ? -1
            : other is InnerLocation inner ? this.Offset.CompareTo(inner.Offset)
            : throw new ArgumentException(nameof(other));
    }
}
