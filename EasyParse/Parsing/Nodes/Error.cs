﻿using EasyParse.Text;

namespace EasyParse.Parsing.Nodes
{
    public abstract class Error : TreeElement
    {
        public Location Location { get; }
        public virtual string Message { get; }

        public Error(Location location, string message)
        {
            this.Location = location;
            this.Message = message;
        }

        protected static string Printable(string raw) =>
            raw.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");

        public override string ToString() =>
            this.Message;
    }
}
