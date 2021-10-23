using System;

namespace EasyParse.ParserGenerator.Models.Rules
{
    internal sealed class GuidReference : RuleReference, IEquatable<GuidReference>
    {
        private Guid Value { get; }

        public GuidReference()
        {
            this.Value = Guid.NewGuid();
        }

        public override bool IsValidAsKey =>
            true;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override bool Equals(object obj) =>
            this.Equals(obj as GuidReference);
        
        public override bool Equals(RuleReference other) =>
            this.Equals(other as GuidReference);

        public bool Equals(GuidReference other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override string ToString() =>
            this.Value.ToString();
    }
}