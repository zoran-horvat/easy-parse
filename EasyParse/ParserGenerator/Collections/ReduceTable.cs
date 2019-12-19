using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class ReduceTable : TransitionTable<int, Terminal, int>, IEnumerable<ReduceCommand>
    {
        public ReduceTable() { }

        public ReduceTable(ImmutableList<Transition<int, Terminal, int>> content) : base(content) { }

        public ReduceTable AddRange(IEnumerable<ReduceCommand> reductions) =>
            new ReduceTable(base.Content.AddRange(reductions));

        public IEnumerator<ReduceCommand> GetEnumerator() =>
            base.Content.OfType<ReduceCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
    }
}