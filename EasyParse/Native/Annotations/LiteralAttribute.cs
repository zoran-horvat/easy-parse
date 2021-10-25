using System;
using EasyParse.Native.Annotations;

namespace EasyParse.Native.Annotations
{
    public class LiteralAttribute : SymbolAttribute
    {
        public LiteralAttribute(string value)
        {
            Value = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException(nameof(value));
        }

        public string Value { get; }
    }
}