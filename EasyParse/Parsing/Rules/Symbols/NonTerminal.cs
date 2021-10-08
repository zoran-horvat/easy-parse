using System;

namespace EasyParse.Parsing.Rules.Symbols
{
    public sealed class NonTerminal : IEquatable<NonTerminal>
    {
        public NonTerminal(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public ParserGenerator.Models.Symbols.NonTerminal ToNonTerminalModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Name);

        public bool Equals(NonTerminal other) =>
            other?.Name == this.Name;

        public override bool Equals(object obj) =>
            this.Equals(obj as NonTerminal);

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public static bool operator ==(NonTerminal a, NonTerminal b) =>
            a?.Equals(b) ?? b is null;

        public static bool operator !=(NonTerminal a, NonTerminal b) =>
            !(a == b);

        public override string ToString() => this.Name;
    }
}
