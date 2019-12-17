using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler.Collections
{
    public class ParsingTable
    {
        private IDictionary<Set<Progression>, int> CoreToIndex { get; }
        public ShiftTable Shift { get; }
        public GotoTable Goto { get; }

        public ParsingTable(List<State> parserStates)
        {
            this.CoreToIndex = parserStates
                .Select((state, index) => (state, index))
                .ToDictionary(tuple => tuple.state.Core, tuple => tuple.index);
            this.Shift = new ShiftTable();
            this.Goto = new GotoTable();
        }

        private ParsingTable(ParsingTable table, ShiftTable shift, GotoTable @goto)
        {
            this.CoreToIndex = table.CoreToIndex;
            this.Shift = shift;
            this.Goto = @goto;
        }

        public ParsingTable Add(CoreTransition transition) =>
            new ParsingTable(this, this.Shift.TryAdd(transition, this.CoreToIndex), this.Goto.TryAdd(transition, this.CoreToIndex));

        public override string ToString() => Formatting.ToString(this);
    }
}
