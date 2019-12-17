using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler.Collections
{
    public class ShiftTable : TransitionTable<int, Terminal>, IEnumerable<ShiftCommand>
    {
        public ShiftTable() { }

        private ShiftTable(ImmutableList<Transition<int, Terminal>> content) : base(content) { }

        public ShiftTable Add(ShiftCommand command) =>
            new ShiftTable(base.Content.Add(command));

        public ShiftTable TryAdd(CoreTransition transition, IDictionary<Set<Progression>, int> coreToIndex) =>
            transition.Symbol is Terminal ? this.Add(ShiftCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<ShiftCommand> GetEnumerator() => 
            base.Content.OfType<ShiftCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        public override string ToString() => Formatting.ToString(this);
    }
}