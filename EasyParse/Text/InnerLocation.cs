namespace EasyParse.Text
{
    public abstract class InnerLocation : Location
    {
        public int Offset { get; }
     
        protected InnerLocation(int offset)
        {
            this.Offset = offset;
        }
    }
}
