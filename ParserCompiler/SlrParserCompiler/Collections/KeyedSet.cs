using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParserCompiler.SlrParserCompiler.Collections
{
    public class KeyedSet<TKey, TValue> : IEquatable<KeyedSet<TKey, TValue>>, IEnumerable<TValue>
    {
        public TKey Key { get; }
        protected Set<TValue> Representation { get; }

        protected KeyedSet(TKey key) : this(key, new Set<TValue>())
        {
        }

        protected KeyedSet(TKey key, Set<TValue> content)
        {
            this.Key = key;
            this.Representation = content;
        }

        public KeyedSet<TKey, TValue> Union(IEnumerable<TValue> values) =>
            new KeyedSet<TKey, TValue>(this.Key, this.Representation.Union(values));

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
