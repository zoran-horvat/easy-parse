using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class IncompleteProductionBuilder : IPendingMapping
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, NonTerminal head)
            : this(completedLines, head, ImmutableList<Symbol>.Empty)
        {
        }

        private IncompleteProductionBuilder(
            ImmutableList<Production> completedLines, NonTerminal head, ImmutableList<Symbol> body)
        {
            this.CompletedLines = completedLines;
            this.Head = head;
            this.Body = body;
        }

        protected ImmutableList<Production> CompletedLines { get; }
        private NonTerminal Head { get; }
        private ImmutableList<Symbol> Body { get; }

        public IPendingMapping Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingMapping Regex(string name, string pattern) =>
            this.Append(new RegexSymbol(name, new Regex(pattern)));

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.Append(new RecursiveNonTerminalSymbol(factory));

        private IPendingMapping Append(Symbol symbol) =>
            new IncompleteProductionBuilder(this.CompletedLines, this.Head, this.Body.Add(symbol));

        public IRule To<T1, TResult>(Func<T1, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1));
        public IRule To<T1, T2, TResult>(Func<T1, T2, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2));
        public IRule To<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3));
        public IRule To<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public IRule To<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        public IRule To<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> transform) => this.To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));

        private IRule To<TResult>(Func<object[], object> transform, params Type[] argumentTypes) =>
            this.Body.Count == argumentTypes.Length ? this.ToRule<TResult>(argumentTypes)
            : throw new ArgumentException(this.IncorrectArgumentCountMessage(argumentTypes.Length));

        private IRule ToRule<TResult>(Type[] argumentTypes) =>
            new CompletedRule(this.Head, typeof(TResult), this.AllProductions<TResult>(argumentTypes));

        private ImmutableList<Production> AllProductions<TResult>(Type[] argumentTypes) =>
            this.CompletedLines.Add(this.ToProduction<TResult>(argumentTypes));

        private Production ToProduction<TResult>(Type[] argumentTypes) =>
            new Production(this.Head, this.Body);

        private string IncorrectArgumentCountMessage(int count) =>
            $"Mapping function receives {count} arguments when expecting " + 
            $"{this.Body.Count()} in rule {this.Head}";
    }
}