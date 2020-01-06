using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class ReduceTableDefinition : TransitionTable<int, Terminal, int>, IEnumerable<ReduceCommand>
    {
        public ReduceTableDefinition() { }

        public ReduceTableDefinition(Set<Transition<int, Terminal, int>> content) : base(content) { }

        public ReduceTableDefinition AddRange(IEnumerable<ReduceCommand> reductions) =>
            new ReduceTableDefinition(base.Content.Union(reductions));

        public IEnumerator<ReduceCommand> GetEnumerator() =>
            base.Content.OfType<ReduceCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
    }
}