using System;
using ParserCompiler.SlrParserCompiler.Models.Symbols;

namespace ParserCompiler.SlrParserCompiler.Models
{
    public abstract class Symbol : IEquatable<Symbol>
    {
        public string Value { get; }
        
        protected Symbol(string value)
        {
            this.Value = value;
        }

        public static Symbol From(char representation) =>
            char.IsUpper(representation) ? (Symbol)new NonTerminal(representation.ToString()) 
            : new Terminal(representation.ToString());

        public override string ToString() =>
            this.Value;

        public override bool Equals(object obj) =>
            this.Equals(obj as Symbol);

        public bool Equals(Symbol other) =>
            !(other is null) &&
            other.GetType() == this.GetType() &&
            other.Value == this.Value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();
    }
}
