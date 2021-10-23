using System;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public abstract class RuleReference : IEquatable<RuleReference>
    {
        public abstract bool Equals(RuleReference other);
        public virtual bool IsValidAsKey => false;

        public static RuleReference Empty() => new EmptyReference();
        public static RuleReference CreateCustom(string value) => new ValueReference(value);
        public static RuleReference CreateCustomOrEmpty(string optionalValue) =>
            string.IsNullOrWhiteSpace(optionalValue) ? new EmptyReference()
            : new ValueReference(optionalValue);
            
        public static RuleReference CreateUnique() => new GuidReference();
        public static RuleReference CreateOrdinal(int ordinal) => new ValueReference($"#{ordinal}");

    }
}