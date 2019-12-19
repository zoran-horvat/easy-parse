using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyParse.Collections
{
    public class LabeledSet<TLabel, TValue> : IEquatable<LabeledSet<TLabel, TValue>>, IEnumerable<TValue> where TValue : class
    {
        public TLabel Label { get; }
        public Set<TValue> Values { get; }

        protected LabeledSet(TLabel label) : this(label, new Set<TValue>())
        {
        }

        protected LabeledSet(TLabel label, Set<TValue> content)
        {
            this.Label = label;
            this.Values = content;
        }

        public LabeledSet<TLabel, TValue> Union(IEnumerable<TValue> values) =>
            new LabeledSet<TLabel, TValue>(this.Label, this.Values.Union(values));

        public IEnumerator<TValue> GetEnumerator() =>
            this.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override bool Equals(object obj) =>
            this.Equals(obj as LabeledSet<TLabel, TValue>);

        public bool Equals(LabeledSet<TLabel, TValue> other) =>
            !(other is null) &&
            other.Label.Equals(this.Label) &&
            other.Values.Equals(this.Values);

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TLabel>.Default.GetHashCode(Label) * 397) ^ this.Values.GetHashCode();
            }
        }

        public string Format(Func<TValue, string> valueFormat, string valuesSeparator, Func<TLabel, string, string> keyValuesFormat) =>
            keyValuesFormat(this.Label, string.Join(valuesSeparator, this.Values.Select(valueFormat).ToArray()));
    }
}
