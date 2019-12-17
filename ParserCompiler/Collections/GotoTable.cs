using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler.Collections
{
    public class GotoTable : TransitionTable<int, NonTerminal>, IEnumerable<GotoCommand>
    {
        public GotoTable() { }

        private GotoTable(ImmutableList<Transition<int, NonTerminal>> content) : base(content) { }

        public GotoTable Add(GotoCommand command) =>
            new GotoTable(base.Content.Add(command));

        public GotoTable TryAdd(CoreTransition transition, IDictionary<Set<Progression>, int> coreToIndex) =>
            transition.Symbol is NonTerminal ? this.Add(GotoCommand.Of(transition, coreToIndex)) : this;

        public IEnumerator<GotoCommand> GetEnumerator() =>
            base.Content.OfType<GotoCommand>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
    }
}