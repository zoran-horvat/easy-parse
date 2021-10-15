using System;

namespace EasyParse.Parsing.Rules
{
    public sealed class NonTerminalName : IEquatable<NonTerminalName>
    {
        public NonTerminalName(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public ParserGenerator.Models.Symbols.NonTerminal ToNonTerminalModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Name);

        public bool Equals(NonTerminalName other) =>
            other?.Name == this.Name;

        public override bool Equals(object obj) =>
            this.Equals(obj as NonTerminalName);

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public static bool operator ==(NonTerminalName a, NonTerminalName b) =>
            a?.Equals(b) ?? b is null;

        public static bool operator !=(NonTerminalName a, NonTerminalName b) =>
            !(a == b);

        public override string ToString() => this.Name;
    }
}
