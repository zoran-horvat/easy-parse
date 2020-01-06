using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Formatting
{
    static class Formatter
    {
        public static string ToString(ParserDefinition parser) =>
            $"{parser.Grammar}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.Table}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.FirstSets.ToString(set => parser.Grammar.SortOrderFor(set.Label))}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.FollowSets.ToString(set => parser.Grammar.SortOrderFor(set.Label))}{Environment.NewLine}{Environment.NewLine}" +
            $"{parser.States}";

        public static string ToString(Progression progression) =>
            $"{progression.Rule.Head} -> {BodyToString(progression)}";

        private static string BodyToString(Progression progression) =>
            ProgressionBodyToString(Join(progression.ConsumedSymbols, " "), Join(progression.PendingSymbols, " "));

        private static string ProgressionBodyToString(string consumed, string pending) =>
            consumed + 
            (consumed.Length == 0 || consumed.EndsWith(" ") ? "" : " ") +
            "∘" + 
            pending;

        public static string NamedToString<TSymbol>(NonTerminalToSymbols<TSymbol> set, string name) where TSymbol : Symbol =>
            $"{name}({set.Label.Value}) = {ToString(set, "{", ", ", "}")}";

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

        public static string ToString(Set<Terminal> set) =>
            ToString(set.OrderBy(x => x), "{", ", ", "}");

        private static int ProgressionToStringWidth(State state) =>
            state.Elements.Max(element => element.Progression.ToString().Length);

        public static string ToString(RuleDefinition rule) =>
            $"{rule.Head} -> {ToString(rule.Body, string.Empty, " ", string.Empty)}";

        public static string ToString(Grammar grammar) =>
            ToString(grammar.Rules);

        public static string ToString(ParsingTable table, List<RuleDefinition> rules) =>
            ToString(table, SortedTerminals(rules).ToList(), SortedNonTerminals(rules).ToList());

        private static IEnumerable<Terminal> SortedTerminals(List<RuleDefinition> rules) =>
            rules.AllSymbols().OfType<Terminal>().OrderBy(x => x).Concat(new[] {new EndOfInput()});

        private static IEnumerable<NonTerminal> SortedNonTerminals(List<RuleDefinition> rules) =>
            rules
                .Select((rule, index) => (symbol: rule.Head, index))
                .GroupBy(tuple => tuple.symbol, tuple => tuple.index)
                .Select(group => (symbol: group.Key, index: group.Min()))
                .OrderBy(tuple => tuple.index)
                .Select(tuple => tuple.symbol);

        private static string ToString(ParsingTable table, List<Terminal> terminals, List<NonTerminal> nonTerminals) =>
            new TableFormat<int, Symbol>()
                .AddHeader((string.Empty, 1), ("SHIFT/REDUCE", terminals.Count), ("GOTO", nonTerminals.Count))
                .AddRows(table.StateIndexes.OrderBy(n => n))
                .AddColumns(terminals)
                .AddColumns(nonTerminals)
                .AddContent(table.Shift.Select(shift => (shift.From, (Symbol)shift.Symbol, $"S{shift.To}")))
                .AddContent(table.Reduce.Select(reduce => (reduce.From, (Symbol)reduce.Symbol, $"R{reduce.To}")))
                .AddContent(table.Goto.Select(@goto => (@goto.From, (Symbol)@goto.Symbol, $"{@goto.To}")))
                .AddContent(0, new NonTerminal(RuleDefinition.AugmentedRootNonTerminal), "ACC")
                .ToString();

        public static string ToString(ShiftTableDefinition shift) =>
            $"SHIFT{Environment.NewLine}{ToString((TransitionTable<int, Terminal, int>)shift)}";

        public static string ToString(GotoTableDefinition @goto) =>
            $"GOTO{Environment.NewLine}{ToString((TransitionTable<int, NonTerminal, int>)@goto)}";

        private static string ToString<TState, TSymbol, TResult>(TransitionTable<TState, TSymbol, TResult> table) where TSymbol : Symbol =>
            string.Join(Environment.NewLine, ToRowStrings(table).ToArray());

        private static IEnumerable<string> ToRowStrings<TState, TSymbol, TResult>(TransitionTable<TState, TSymbol, TResult> table) where TSymbol : Symbol =>
            table.Items
                .GroupBy(item => item.From)
                .OrderBy(group => group.Key)
                .Select(group => ToRowString(group.Key, group));

        private static string ToRowString<TState, TSymbol, TResult>(TState header, IEnumerable<Transition<TState, TSymbol, TResult>> row) where TSymbol : Symbol =>
            $"{header,4}: {ToRowElementsString(row)}";

        private static string ToRowElementsString<TState, TSymbol, TResult>(IEnumerable<Transition<TState, TSymbol, TResult>> row) where TSymbol : Symbol =>
            string.Join(" ", ToRowElementsStrings(row).ToArray());

        private static IEnumerable<string> ToRowElementsStrings<TState, TSymbol, TResult>(IEnumerable<Transition<TState, TSymbol, TResult>> row) where TSymbol : Symbol =>
            row.OrderBy(item => item.Symbol)
                .Select(item => $"{item.Symbol}->{item.To}");

        private static string ToString(IEnumerable<RuleDefinition> rules) =>
            ToString(rules.Select((rule, index) => $"{index, 3}. {ToString(rule)}"), string.Empty, Environment.NewLine, string.Empty);

        private static int ProgressionToStringWidth(IEnumerable<State> states) =>
            states.Max(ProgressionToStringWidth);

        public static string ToString(StateElement stateElement) =>
            $"{ToString(stateElement.Progression)} {ToString(stateElement.FollowedBy)}";

        public static string ToString<T>(IEnumerable<T> set, string prefix, string separator, string suffix, Func<T, int> sortOrder) =>
            ToString(set.OrderBy(sortOrder), prefix, separator, suffix);

        public static string ToString<T>(IEnumerable<T> set, string separator) =>
            ToString(set, "{", separator, "}");

        private static string ToString<T>(IEnumerable<T> set, string prefix, string separator, string suffix) =>
            prefix + Join(set, separator) + suffix;

        private static string Join<T>(IEnumerable<T> sequence, string separator) =>
            string.Join(separator, sequence.Select(x => $"{x}").ToArray());
    }
}
