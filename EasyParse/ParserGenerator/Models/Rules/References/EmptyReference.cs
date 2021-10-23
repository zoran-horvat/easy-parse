using System;

namespace EasyParse.ParserGenerator.Models.Rules
{
    internal sealed class EmptyReference : RuleReference, IEquatable<EmptyReference>
    {
        public override int GetHashCode() => 0;

        public override bool Equals(object obj) =>
            this.Equals(obj as EmptyReference);
        
        public override bool Equals(RuleReference other) =>
            this.Equals(other as EmptyReference);

        public bool Equals(EmptyReference other) =>
            other is not null;

        public override string ToString() =>
            "<anonymous>";
    }
}