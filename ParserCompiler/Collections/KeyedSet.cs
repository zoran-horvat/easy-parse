using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParserCompiler.Collections
{
    public class KeyedSet<TKey, TValue> : IEquatable<KeyedSet<TKey, TValue>>, IEnumerable<TValue>
    {
        public TKey Key { get; }
        public Set<TValue> Values { get; }

        protected KeyedSet(TKey key) : this(key, new Set<TValue>())
        {
        }

        protected KeyedSet(TKey key, Set<TValue> content)
        {
            this.Key = key;
            this.Values = content;
        }

        public KeyedSet<TKey, TValue> Union(IEnumerable<TValue> values) =>
            new KeyedSet<TKey, TValue>(this.Key, this.Values.Union(values));

        public IEnumerator<TValue> GetEnumerator() =>
            this.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override bool Equals(object obj) =>
            this.Equals(obj as KeyedSet<TKey, TValue>);

        public bool Equals(KeyedSet<TKey, TValue> other) =>
            !(other is null) &&
            other.Key.Equals(this.Key) &&
            other.Values.Equals(this.Values);

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TKey>.Default.GetHashCode(Key) * 397) ^ this.Values.GetHashCode();
            }
        }

        public string Format(Func<TValue, string> valueFormat, string valuesSeparator, Func<TKey, string, string> keyValuesFormat) =>
            keyValuesFormat(this.Key, string.Join(valuesSeparator, this.Values.Select(valueFormat).ToArray()));
    }
}
