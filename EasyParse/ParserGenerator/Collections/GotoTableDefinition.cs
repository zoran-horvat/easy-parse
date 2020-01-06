using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class GotoTableDefinition : TransitionTable<int, NonTerminal, int>, IEnumerable<GotoCommand>
    {
        public GotoTableDefinition() { }

        private GotoTableDefinition(Set<Transition<int, NonTerminal, int>> content) : base(content) { }

        public GotoTableDefinition Add(GotoCommand command) =>
            new GotoTableDefinition(base.Content.Union(new[] {command}));

        public GotoTableDefinition TryAdd(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is NonTerminal ? this.Add(GotoCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<GotoCommand> GetEnumerator() =>
            base.Content.OfType<GotoCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => Formatter.ToString(this);
    }
}