using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class GotoTable : TransitionTable<int, NonTerminal, int>, IEnumerable<GotoCommand>
    {
        public GotoTable() { }

        private GotoTable(Set<Transition<int, NonTerminal, int>> content) : base(content) { }

        public GotoTable Add(GotoCommand command) =>
            new GotoTable(base.Content.Union(new[] {command}));

        public GotoTable TryAdd(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is NonTerminal ? this.Add(GotoCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<GotoCommand> GetEnumerator() =>
            base.Content.OfType<GotoCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => Formatter.ToString(this);
    }
}