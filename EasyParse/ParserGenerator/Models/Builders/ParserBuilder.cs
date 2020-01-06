using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.ParserGenerator.Models.Builders
{
    public class ParserBuilder
    {
        private Grammar Grammar { get; }

        private ParserBuilder(Grammar grammar)
        {
            this.Grammar = grammar;
        }

        public static ParserBuilder For(Grammar grammar) =>
            new ParserBuilder(grammar);

        public ParserDefinition Build()
        {
            Set<FirstSet> firstSets = FirstSetsBuilder.BuildFor(this.Grammar.Rules);
            Set<FollowSet> followSets = FollowSetsBuilder.BuildFor(this.Grammar.Rules, firstSets);
            (StateVector states, ParsingTable table) = new StateVector(this.Grammar.Rules, firstSets, followSets).Closure();

            return new ParserDefinition(this.Grammar, firstSets, followSets, states, table);
        }
    }
}
