using System;

namespace EasyParse.ParserGenerator.Models.Rules
{
    internal sealed class ValueReference : RuleReference, IEquatable<ValueReference>
    {
        private string Value { get; }

        public ValueReference(string value)
        {
            this.Value =
                !string.IsNullOrWhiteSpace(value) ? value
                : throw new ArgumentException(nameof(value));
        }

        public override bool IsValidAsKey => 
            true;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override bool Equals(object obj) =>
            this.Equals(obj as ValueReference);

        public override bool Equals(RuleReference other) =>
            this.Equals(other as ValueReference);

        public bool Equals(ValueReference other) =>
            other?.Value?.Equals(this.Value) ?? false;

        public override string ToString() =>
            this.Value;
    }
}