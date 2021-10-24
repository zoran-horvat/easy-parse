using System;

namespace EasyParse.Native
{
    public class LiteralAttribute : SymbolAttribute
    {
        public LiteralAttribute(string value) 
        {
            this.Value = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException(nameof(value));
        }
        
        public string Value { get; }
    }
}