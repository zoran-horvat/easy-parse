using System;
using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class ConstantLexeme : Lexeme, IEquatable<ConstantLexeme>
    {
        public string ConstantValue { get; }

        public ConstantLexeme(string value) : base(Regex.Escape(value))
        {
            this.ConstantValue = value ?? string.Empty;
        }

        public override bool Equals(object obj) =>
            this.Equals(obj as ConstantLexeme);

        public bool Equals(ConstantLexeme other) =>
            other?.ConstantValue.Equals(this.ConstantValue) ?? false;

        public override int GetHashCode() =>
            this.ConstantValue.GetHashCode();

        public override string ToString() =>
            $"Constant \"{this.ConstantValue}\"";
    }
}
