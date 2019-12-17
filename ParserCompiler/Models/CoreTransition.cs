using System.Collections.Generic;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public class CoreTransition
    {
        public Set<Progression> FromCore { get; }
        public Symbol Symbol { get; }
        public Set<Progression> ToCore { get; }

        public CoreTransition(Set<Progression> fromCore, Symbol symbol, Set<Progression> toCore)
        {
            this.FromCore = fromCore;
            this.Symbol = symbol;
            this.ToCore = toCore;
        }

        public IEnumerable<IndexTransition<T>> ToIndexTransition<T>(IDictionary<Set<Progression>, int> coreToIndex) where T : Symbol =>
            this.Symbol is T symbol ? new[] {new IndexTransition<T>(coreToIndex[this.FromCore], symbol, coreToIndex[this.ToCore])}
            : new IndexTransition<T>[0];
    }
}
