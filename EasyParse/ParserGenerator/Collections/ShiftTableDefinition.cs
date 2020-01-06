using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class ShiftTableDefinition : TransitionTable<int, Terminal, int>, IEnumerable<ShiftCommand>
    {
        public ShiftTableDefinition() { }

        private ShiftTableDefinition(Set<Transition<int, Terminal, int>> content) : base(content) { }

        public ShiftTableDefinition Add(ShiftCommand command) =>
            new ShiftTableDefinition(base.Content.Union(new[] {command}));

        public ShiftTableDefinition TryAdd(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is Terminal ? this.Add(ShiftCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<ShiftCommand> GetEnumerator() => 
            base.Content.OfType<ShiftCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => Formatter.ToString(this);
    }
}