using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParserCompiler
{
    public class KeyedSet<TKey, TValue> : IEquatable<KeyedSet<TKey, TValue>>, IEnumerable<TValue>
    {
        public TKey Key { get; }
        private Set<TValue> Representation { get; }

        public KeyedSet(TKey key) : this(key, Enumerable.Empty<TValue>()) { }

        public KeyedSet(TKey key, IEnumerable<TValue> content)
        {
            this.Key = key;
            this.Representation = new Set<TValue>(content);
        }

        public IEnumerator<TValue> GetEnumerator() =>
            this.Representation.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override bool Equals(object obj) =>
            this.Equals(obj as KeyedSet<TKey, TValue>);

        public bool Equals(KeyedSet<TKey, TValue> other) =>
            !(other is null) &&
            other.Key.Equals(this.Key) &&
            other.Representation.Equals(this.Representation);

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TKey>.Default.GetHashCode(Key) * 397) ^ this.Representation.GetHashCode();
            }
        }

        public string Format(Func<TValue, string> valueFormat, string valuesSeparator, Func<TKey, string, string> keyValuesFormat) =>
            keyValuesFormat(this.Key, string.Join(valuesSeparator, this.Representation.Select(valueFormat).ToArray()));
    }
}
