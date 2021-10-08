using System;

namespace EasyParse.Parsing.Rules
{
    public interface IPendingMapping : IProductionBuilder
    {
        IRule Map<T1, TResult>(Func<T1, TResult> transform);
        IRule Map<T1, T2, TResult>(Func<T1, T2, TResult> transform);
        IRule Map<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> transform);
        IRule Map<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> transform);
        IRule Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> transform);

        IRule MapIdentity<T>() => Map((T x) => x);
    }
}