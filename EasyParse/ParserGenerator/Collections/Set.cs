using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;

namespace EasyParse.ParserGenerator.Collections
{
    public class Set<TValue> : IEquatable<Set<TValue>>, IEnumerable<TValue> where TValue : class
    {
        private ImmutableHashSet<TValue> Representation {get;}

        public Set() : this(ImmutableHashSet<TValue>.Empty) { }

        private Set(ImmutableHashSet<TValue> content)
        {
            this.Representation = content;
        }

        public Set<TValue> Union(IEnumerable<TValue> values) =>
            new Set<TValue>(this.Representation.Union(values));

        public override bool Equals(object obj) => 
            this.Equals(obj as Set<TValue>);

        public bool Equals(Set<TValue> other) =>
            !(other is null) &&
            other.Representation.All(this.Representation.Contains) &&
            this.Representation.All(other.Representation.Contains);

        public override int GetHashCode() => 
            this.Representation.Aggregate(0, (acc, item) => acc ^ item.GetHashCode());

        public IEnumerator<TValue> GetEnumerator() =>
            this.Representation.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => 
            Formatter.ToString(this, ", ");

        public string ToString(string prefix, string separator, string suffix, Func<TValue, int> sortOrder) => 
            Formatter.ToString(this, prefix, separator, suffix, sortOrder);

        public string ToString(Func<TValue, int> sortOrder) => Formatter.ToString(this, sortOrder);
    }
}
