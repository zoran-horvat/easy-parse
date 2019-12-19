using System;

namespace EasyParse.Models.Symbols
{
    public abstract class Symbol : IEquatable<Symbol>, IComparable<Symbol>
    {
        public string Value { get; }
        
        protected Symbol(string value)
        {
            this.Value = value;
        }

        public static Symbol From(char representation) =>
            char.IsUpper(representation) ? (Symbol)new NonTerminal(representation.ToString()) 
            : new Terminal(representation.ToString());

        public int CompareTo(Symbol other) =>
            this.TypeOrder(this).CompareTo(this.TypeOrder(other)) is int typeComparison && typeComparison != 0 ? typeComparison
            : StringComparer.Ordinal.Compare(this.Value, other.Value);

        private int TypeOrder(Symbol obj) =>
            obj is NonTerminal ? 0
            : obj is EndOfInput ? 2
            : 1;

        public override string ToString() => this.Value;

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
