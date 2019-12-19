using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class ShiftTable : TransitionTable<int, Terminal, int>, IEnumerable<ShiftCommand>
    {
        public ShiftTable() { }

        private ShiftTable(ImmutableList<Transition<int, Terminal, int>> content) : base(content) { }

        public ShiftTable Add(ShiftCommand command) =>
            new ShiftTable(base.Content.Add(command));

        public ShiftTable TryAdd(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is Terminal ? this.Add(ShiftCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<ShiftCommand> GetEnumerator() => 
            base.Content.OfType<ShiftCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => Formatter.ToString(this);
    }
}