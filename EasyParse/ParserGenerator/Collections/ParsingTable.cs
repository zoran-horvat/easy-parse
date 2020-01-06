using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public class ParsingTable
    {
        private IDictionary<Core, int> CoreToIndex { get; }
        private IDictionary<RuleDefinition, int> RuleToIndex { get; }
        public ShiftTableDefinition Shift { get; }
        public GotoTableDefinition Goto { get; }
        public ReduceTableDefinition Reduce { get; }
        private ImmutableList<RuleDefinition> Rules { get; }

        public IEnumerable<int> StateIndexes => this.CoreToIndex.Values;

        public ParsingTable(List<State> parserStates, List<RuleDefinition> rules)
        {
            this.Rules = rules.ToImmutableList();

            this.CoreToIndex = parserStates
                .Select((state, index) => (state, index))
                .ToDictionary(tuple => tuple.state.Core, tuple => tuple.index);

            this.RuleToIndex = rules
                .Select((rule, index) => (rule, index))
                .ToDictionary(tuple => tuple.rule, tuple => tuple.index);

            this.Shift = new ShiftTableDefinition();
            this.Goto = new GotoTableDefinition();
            this.Reduce = new ReduceTableDefinition();
        }

        private ParsingTable(ParsingTable table, ShiftTableDefinition shift, GotoTableDefinition @goto, ReduceTableDefinition reduce)
        {
            this.Rules = table.Rules;
            this.CoreToIndex = table.CoreToIndex;
            this.RuleToIndex = table.RuleToIndex;
            this.Shift = shift;
            this.Goto = @goto;
            this.Reduce = reduce;
        }

        public ParsingTable Add(CoreTransition transition) =>
            new ParsingTable(this, this.Shift.TryAdd(transition, this.CoreToIndex), this.Goto.TryAdd(transition, this.CoreToIndex), this.Reduce);

        public ParsingTable AddRange(IEnumerable<CoreReduction> reductions) =>
            this.AddRange(reductions.Select(reduction => new ReduceCommand(this.CoreToIndex[reduction.From], reduction.Symbol, this.RuleToIndex[reduction.To])));

        private ParsingTable AddRange(IEnumerable<ReduceCommand> reductions) =>
            new ParsingTable(this, this.Shift, this.Goto, this.Reduce.AddRange(reductions));

        public override string ToString() => Formatter.ToString(this, this.Rules.ToList());
    }
}
