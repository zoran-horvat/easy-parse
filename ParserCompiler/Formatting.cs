using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Collections;
using ParserCompiler.Models;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler
{
    static class Formatting
    {
        public static string ToString(Parser parser) =>
            $"{parser.Grammar}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.FirstSets.ToString(set => parser.Grammar.SortOrderFor(set.Key))}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.FollowSets.ToString(set => parser.Grammar.SortOrderFor(set.Key))}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.Table}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.States}";

        public static string ToString(Progression progression) =>
            $"{progression.Rule.Head} -> {BodyToString(progression)}";

        private static string BodyToString(Progression progression) =>
            Join(progression.ConsumedSymbols, string.Empty) + "∘" + Join(progression.PendingSymbols, string.Empty);

        public static string NamedToString<TSymbol>(NonTerminalToSymbols<TSymbol> set, string name) where TSymbol : Symbol =>
            $"{name}({set.Key.Value}) = {ToString(set, "{", string.Empty, "}")}";

        public static string ToString(State state) =>
            ToString(state, ProgressionToStringWidth(state));

        private static string ToString(State state, int progressionWidth) =>
            Join(state.Elements.Select(element => ToString(element, progressionWidth)), Environment.NewLine);

        public static string ToString(StateElement element, int progressionWidth) =>
            $"{element.Progression.ToString().PadRight(progressionWidth)} {ToString(element.FollowedBy)}";

        public static string ToString(StateVector states) =>
            ToString(states, ProgressionToStringWidth(states.States));

        private static string ToString(StateVector states, int progressionWidth) =>
            Join(states.States.Select((state, index) => ToString(state, progressionWidth, index)), $"{Environment.NewLine}{Environment.NewLine}");

        private static string ToString(State state, int progressionWidth, int stateIndex) =>
            $"S{stateIndex}{Environment.NewLine}{ToString(state, progressionWidth)}";

        public static string ToString<T>(Set<T> set, Func<T, int> sortOrder) where T : class =>
            set is Set<FirstSet> firstSets ? ToString(firstSets, string.Empty, Environment.NewLine, string.Empty, firstSet => sortOrder(firstSet as T))
            : set is Set<FollowSet> followSets ? ToString(followSets, string.Empty, Environment.NewLine, string.Empty, followSet => sortOrder(followSet as T))
            : ToString(set, string.Empty);

        public static string ToString(Set<FirstSet> firstSets, Func<FirstSet, int> sortOrder) =>
            ToString<FirstSet>(firstSets, String.Empty, Environment.NewLine, string.Empty, sortOrder);

        public static string ToString(Set<FollowSet> followSets, Func<FollowSet, int> sortOrder) =>
            ToString<FollowSet>(followSets, String.Empty, Environment.NewLine, string.Empty, sortOrder);

        public static string ToString(Set<Terminal> set) =>
            ToString(set.OrderBy(x => x), "{", string.Empty, "}");

        private static int ProgressionToStringWidth(State state) =>
            state.Elements.Max(element => element.Progression.ToString().Length);

        public static string ToString(Rule rule) =>
            $"{rule.Head} -> {ToString(rule.Body, string.Empty, string.Empty, string.Empty)}";

        public static string ToString(Grammar grammar) =>
            ToString(grammar.Rules);

        public static string ToString(ParsingTable table) =>
            $"{ToString(table.Shift)}{Environment.NewLine}{Environment.NewLine}{ToString(table.Goto)}";

        public static string ToString(ShiftTable shift) =>
            $"SHIFT{Environment.NewLine}{ToString((TransitionTable<int, Terminal>)shift)}";

        public static string ToString(GotoTable @goto) =>
            $"GOTO{Environment.NewLine}{ToString((TransitionTable<int, NonTerminal>)@goto)}";

        private static string ToString<TState, TSymbol>(TransitionTable<TState, TSymbol> table) where TSymbol : Symbol =>
            string.Join(Environment.NewLine, ToRowStrings(table).ToArray());

        private static IEnumerable<string> ToRowStrings<TState, TSymbol>(TransitionTable<TState, TSymbol> table) where TSymbol : Symbol =>
            table.Items
                .GroupBy(item => item.From)
                .OrderBy(group => group.Key)
                .Select(group => ToRowString(group.Key, group));

        private static string ToRowString<TState, TSymbol>(TState header, IEnumerable<Transition<TState, TSymbol>> row) where TSymbol : Symbol =>
            $"  {header,4}: {ToRowElementsString(row)}";

        private static string ToRowElementsString<TState, TSymbol>(IEnumerable<Transition<TState, TSymbol>> row) where TSymbol : Symbol =>
            string.Join(" ", ToRowElementsStrings(row).ToArray());

        private static IEnumerable<string> ToRowElementsStrings<TState, TSymbol>(IEnumerable<Transition<TState, TSymbol>> row) where TSymbol : Symbol =>
            row.OrderBy(item => item.Symbol)
                .Select(item => $"{item.Symbol}->{item.To}");

        private static string ToString(IEnumerable<Rule> rules) =>
            ToString(rules.Select((rule, index) => $"{index, 3}. {ToString(rule)}"), string.Empty, Environment.NewLine, string.Empty);

        private static int ProgressionToStringWidth(IEnumerable<State> states) =>
            states.Max(ProgressionToStringWidth);

        public static string ToString(StateElement stateElement) =>
            $"{ToString(stateElement.Progression)} {ToString(stateElement.FollowedBy)}";

        public static string SymbolsToString<TSymbol>(IEnumerable<TSymbol> set, string separator) where TSymbol : Symbol =>
            ToString(set.OrderBy(x => x).Select(value => value.Value), "{", separator, "}");

        public static string ToString<T>(IEnumerable<T> set, string separator, Func<T, int> sortOrder) =>
            ToString(set, "{", separator, "}", sortOrder);

        public static string ToString<T>(IEnumerable<T> set, string prefix, string separator, string suffix, Func<T, int> sortOrder) =>
            ToString(set.OrderBy(sortOrder), prefix, separator, suffix);

        public static string ToString<T>(IEnumerable<T> set, string separator) =>
            ToString(set, "{", separator, "}");

        public static string ToString<T>(IEnumerable<T> set, string prefix, string separator, string suffix) =>
            prefix + Join(set, separator) + suffix;

        private static string Join<T>(IEnumerable<T> sequence) => Join(sequence, string.Empty);

        private static string Join<T>(IEnumerable<T> sequence, string separator) =>
            string.Join(separator, sequence.Select(x => $"{x}").ToArray());
    }
}
