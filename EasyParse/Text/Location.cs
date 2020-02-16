using System;

namespace EasyParse.Text
{
    public abstract class Location : IComparable<Location>
    {
        public abstract int CompareTo(Location other);
    }
}
