using System;
using System.Linq;
using System.Collections.Generic;

namespace EasyParse.Native.Annotations
{
    public class LiteralAttribute : SymbolAttribute
    {
        public LiteralAttribute(string value, params string[] otherValues)
            : this(new[] {value}.Concat(otherValues))
        {
        }

        private LiteralAttribute(IEnumerable<string> values)
        {
            this.Values = values.Select(Valid).ToList();
        }

        private static string Valid(string value) =>
            !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException(nameof(value));

        public IEnumerable<string> Values { get; }
    }
}