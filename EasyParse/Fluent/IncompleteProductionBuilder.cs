using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Fluent.Rules;
using EasyParse.Fluent.Symbols;

namespace EasyParse.Fluent
{
    internal class IncompleteProductionBuilder : IPendingMapping
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, NonTerminalName head)
            : this(completedLines, head, ImmutableList<Symbol>.Empty)
        {
        }

        private IncompleteProductionBuilder(
            ImmutableList<Production> completedLines, NonTerminalName head, ImmutableList<Symbol> body)
        {
            CompletedLines = completedLines;
            Head = head;
            Body = body;
        }

        protected ImmutableList<Production> CompletedLines { get; }
        private NonTerminalName Head { get; }
        private ImmutableList<Symbol> Body { get; }

        public IPendingMapping Match(Symbol first, params Symbol[] others) =>
            others.Aggregate(Append(first), (rule, symbol) => rule.Append(symbol));

        public IPendingMapping Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingMapping Regex<T>(string name, string pattern, Func<string, T> transform) =>
            this.Append(RegexSymbol.Create(name, new Regex(pattern), transform));

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.Append(new RecursiveNonTerminalSymbol(factory));

        private IncompleteProductionBuilder Append(Symbol symbol) =>
            new(CompletedLines, Head, Body.Add(symbol));

        public IRule To<T1, TResult>(Func<T1, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1));
        public IRule To<T1, T2, TResult>(Func<T1, T2, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2));
        public IRule To<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3));
        public IRule To<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public IRule To<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        public IRule To<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        public IRule To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> transform) => To<TResult>(transform.DynamicInvoke, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));

        private IRule To<TResult>(Func<object[], object> transform, params Type[] argumentTypes) =>
            NonLiteralsCount != argumentTypes.Length ? ThrowIncorrectArgumentsCount<IRule>(argumentTypes.Length)
            : ContainsLiterals ? NonLiteralsTo<TResult>(transform, argumentTypes)
            : ToRule<TResult>(transform, argumentTypes);

        public IRule ToLiteral<TResult>(Func<string, TResult> transform)
        {
            string SingleString(object[] x)
            {
                if (x.Length != 1)
                {
                    return ThrowIncorrectArgumentsCount<string>(1);
                }

                return (string) x.Take(1).First();
            }

            return ToRule<TResult>(arguments => transform(SingleString(arguments)), new[] { typeof(string) });
        }

        public IRule ToLiteral()
        {
            return ToLiteral(s => s);
        }

        private IRule NonLiteralsTo<TResult>(Func<object[], object> transform, Type[] argumentTypes) =>
            NonLiteralsTo<TResult>(transform, InjectLiteralsTo(argumentTypes).ToArray(), NonLiteralIndices);

        private IRule NonLiteralsTo<TResult>(
            Func<object[], object> nonLiteralsTransform, Type[] argumentTypesWithLiterals, int[] argumentsMap)
        {
            object[] ArgumentsPurge(object[] arguments) =>
                argumentsMap.Select(index => arguments[index]).ToArray();

            object AugmentedTransform(object[] arguments) =>
                nonLiteralsTransform(ArgumentsPurge(arguments));

            return ToRule<TResult>(AugmentedTransform, argumentTypesWithLiterals);
        }

        private IEnumerable<Type> InjectLiteralsTo(IEnumerable<Type> argumentTypes)
        {
            using IEnumerator<Type> argumentType = argumentTypes.GetEnumerator();

            foreach (Symbol symbol in Body)
            {
                if (symbol is LiteralSymbol)
                {
                    yield return typeof(string);
                }
                else
                {
                    argumentType.MoveNext();
                    yield return argumentType.Current;
                }
            }
        }

        private int[] NonLiteralIndices =>
            Body
                .Select((symbol, index) => (symbol, index))
                .Where(tuple => tuple.symbol is not LiteralSymbol)
                .Select(tuple => tuple.index)
                .ToArray();

        private int NonLiteralsCount =>
            Body.Count(symbol => symbol is not LiteralSymbol);

        private int LiteralsCount =>
            Body.Count(symbol => symbol is LiteralSymbol);

        private bool ContainsLiterals =>
            Body.Any(symbol => symbol is LiteralSymbol);

        private IRule ToRule<TResult>(Func<object[], object> function, Type[] argumentTypes) =>
            new CompletedRule(Head, AllProductions<TResult>(function, argumentTypes));

        private ImmutableList<Production> AllProductions<TResult>(Func<object[], object> function, Type[] argumentTypes) =>
            CompletedLines.Add(ToProduction<TResult>(function, argumentTypes));

        private Production ToProduction<TResult>(Func<object[], object> function, Type[] argumentTypes) =>
            new Production(Head, Body, ToTransform<TResult>(function, argumentTypes));

        private Transform ToTransform<TResult>(Func<object[], object> function, Type[] argumentTypes) =>
            Body.Count == argumentTypes.Length ? new Transform(typeof(TResult), argumentTypes, function)
            : throw new ArgumentException(IncorrectArgumentCountMessage(argumentTypes.Length));

        private T ThrowIncorrectArgumentsCount<T>(int count) =>
            throw new ArgumentException(IncorrectArgumentCountMessage(count));

        private string IncorrectArgumentCountMessage(int count) =>
            $"Mapping function receives {count} arguments when expecting " +
            $"{Body.Count()} in rule {Head}";
    }
}