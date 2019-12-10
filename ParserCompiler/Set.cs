using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParserCompiler
{
    public class Set<TValue> : IEquatable<Set<TValue>>, IEnumerable<TValue>
    {
        private HashSet<TValue> Representation {get;}

        public Set() : this(Enumerable.Empty<TValue>()) { }

        public Set(IEnumerable<TValue> content)
        {
            this.Representation = new HashSet<TValue>(content);
        }

        public void Add(TValue value) => 
            this.Representation.Add(value);

        public void Add(IEnumerable<TValue> values)
        {
            foreach (TValue item in values)
                this.Representation.Add(item);
        }

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
            this.ToString("{", ", ", "}");

        public string ToString(string prefix, string separator, string suffix) =>
            $"{prefix}{string.Join(separator, this.Representation.Select(item => item.ToString()))}{suffix}";
    }
}
