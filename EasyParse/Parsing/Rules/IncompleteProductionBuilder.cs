using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class IncompleteProductionBuilder : IPendingMapping
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, Production currentLine)
        {
            this.CompletedLines = completedLines;
            this.CurrentLine = currentLine;
        }

        private ImmutableList<Production> CompletedLines { get; }
        private Production CurrentLine { get; }

        public IPendingMapping Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingMapping Regex(string name, string pattern) =>
            this.Append(new RegexSymbol(name, new Regex(pattern)));

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.Append(new RecursiveNonTerminalSymbol(factory));

        private IPendingMapping Append(Symbol symbol) =>
            new IncompleteProductionBuilder(this.CompletedLines, this.CurrentLine.Append(symbol));

        public IRule Map<T1, TResult>(Func<T1, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1));
        public IRule Map<T1, T2, TResult>(Func<T1, T2, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2));
        public IRule Map<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3));
        public IRule Map<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public IRule Map<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        public IRule Map<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        public IRule Map<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        public IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        public IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        public IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> transform) => this.Map(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
        
        private IRule Map(Func<object[], object> transform, params Type[] argumentTypes) =>
            argumentTypes.Length != this.CurrentLine.Body.Count() ? throw new ArgumentException($"Mapping function receives {argumentTypes.Length} arguments when expecting {this.CurrentLine.Body.Count()} in rule {this.CurrentLine}")
            : new CompletedRule(this.CurrentLine.Head, this.CompletedLines.Add(this.CurrentLine));
    }
}