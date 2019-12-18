using System;
using System.Collections.Generic;
using ParserCompiler.Models.Rules;

namespace ParserCompiler.Collections
{
    public class Core : IEquatable<Core>
    {
        private Set<Progression> Progressions { get; }

        public Core(IEnumerable<Progression> progressions)
        {
            this.Progressions = progressions.AsSet();
        }

        public override bool Equals(object obj) => 
            this.Equals(obj as Core);

        public bool Equals(Core other) =>
            other is Core core &&
            core.Progressions.Equals(this.Progressions);

        public override int GetHashCode() =>
            this.Progressions.GetHashCode();
    }
}
